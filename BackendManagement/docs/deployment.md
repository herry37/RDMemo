# 後台管理系統部署指南

## 1. 部署準備

### 1.1 環境檢查清單

```powershell
# 檢查必要工具版本
$requiredVersions = @{
    "dotnet" = "8.0"
    "docker" = "24.0"
    "kubectl" = "1.28"
    "helm" = "3.0"
}

foreach ($tool in $requiredVersions.Keys) {
    $version = & $tool --version
    if (-not $version -match $requiredVersions[$tool]) {
        Write-Error "錯誤: $tool 版本不符合要求 (需要 $($requiredVersions[$tool]))"
        exit 1
    }
}

# 檢查網路連線
$endpoints = @(
    "registry.azurecr.io",
    "kubernetes.default.svc",
    "postgresql-service",
    "redis-service"
)

foreach ($endpoint in $endpoints) {
    if (-not (Test-NetConnection $endpoint -Port 443)) {
        Write-Error "錯誤: 無法連線到 $endpoint"
        exit 1
    }
}
```

### 1.2 配置管理

```powershell
# 載入配置檔案
$config = Get-Content deploy/config/deployment.json | ConvertFrom-Json

# 設定環境變數
$envVars = @{
    "ASPNETCORE_ENVIRONMENT" = $config.environment
    "REGISTRY_URL" = $config.registry.url
    "DB_CONNECTION" = $config.database.connectionString
    "REDIS_CONNECTION" = $config.redis.connectionString
    "JWT_SECRET" = $config.security.jwtSecret
}

# 驗證必要變數
foreach ($var in $envVars.Keys) {
    if ([string]::IsNullOrEmpty($envVars[$var])) {
        Write-Error "錯誤: 環境變數 $var 未設定"
        exit 1
    }
    [Environment]::SetEnvironmentVariable($var, $envVars[$var])
}
```

## 2. 基礎設施部署

### 2.1 命名空間管理

```powershell
# 定義命名空間清單
$namespaces = @(
    "monitoring",
    "logging",
    "cert-manager",
    "backend-management"
)

# 建立命名空間
foreach ($ns in $namespaces) {
    kubectl create namespace $ns --dry-run=client -o yaml | kubectl apply -f -
}
```

### 2.2 密鑰管理

```powershell
# 從 Azure Key Vault 取得密鑰
$secrets = @{
    "db-password" = az keyvault secret show --name db-password --vault-name $config.keyVault.name --query value -o tsv
    "redis-password" = az keyvault secret show --name redis-password --vault-name $config.keyVault.name --query value -o tsv
    "jwt-secret" = az keyvault secret show --name jwt-secret --vault-name $config.keyVault.name --query value -o tsv
}

# 建立 Kubernetes Secrets
kubectl create secret generic backend-secrets `
    --namespace backend-management `
    --from-literal=db-password=$secrets["db-password"] `
    --from-literal=redis-password=$secrets["redis-password"] `
    --from-literal=jwt-secret=$secrets["jwt-secret"] `
    --dry-run=client -o yaml | kubectl apply -f -
```

## 3. 應用程式部署

### 3.1 資料庫部署

```powershell
# 部署 PostgreSQL
$dbValues = Get-Content deploy/helm/postgresql-values.yaml
helm upgrade --install backend-db bitnami/postgresql `
    --namespace backend-management `
    --values $dbValues `
    --set auth.existingSecret=backend-secrets `
    --wait

# 驗證資料庫連線
$podName = kubectl get pods -l app=postgresql -n backend-management -o jsonpath="{.items[0].metadata.name}"
kubectl exec $podName -n backend-management -- pg_isready
```

### 3.2 快取服務部署

```powershell
# 部署 Redis
$redisValues = Get-Content deploy/helm/redis-values.yaml
helm upgrade --install backend-redis bitnami/redis `
    --namespace backend-management `
    --values $redisValues `
    --set auth.existingSecret=backend-secrets `
    --wait

# 驗證 Redis 連線
$podName = kubectl get pods -l app=redis -n backend-management -o jsonpath="{.items[0].metadata.name}"
kubectl exec $podName -n backend-management -- redis-cli ping
```

### 3.3 應用程式部署

```powershell
# 部署應用程式
kubectl apply -f deploy/k8s/backend-management.yaml -n backend-management

# 等待部署完成
kubectl rollout status deployment/backend-management -n backend-management

# 驗證應用程式健康狀態
$endpoint = kubectl get service backend-management -n backend-management -o jsonpath="{.status.loadBalancer.ingress[0].ip}"
Invoke-RestMethod "http://$endpoint/health"
```

## 4. 監控與日誌

### 4.1 監控設定

```powershell
# 部署 Prometheus 和 Grafana
helm upgrade --install monitoring prometheus-community/kube-prometheus-stack `
    --namespace monitoring `
    --values deploy/helm/monitoring-values.yaml `
    --wait

# 設定 Grafana 儀表板
kubectl apply -f deploy/k8s/grafana-dashboards.yaml -n monitoring
```

### 4.2 日誌收集

```powershell
# 部署 EFK Stack
helm upgrade --install logging elastic/eck-operator `
    --namespace logging `
    --values deploy/helm/logging-values.yaml `
    --wait

# 驗證日誌收集
kubectl logs -l app=fluent-bit -n logging --tail=1
```

## 5. 故障排除指南

### 5.1 健康狀態檢查

```powershell
# 檢查所有服務狀態
$services = @(
    "backend-management",
    "backend-db",
    "backend-redis"
)

foreach ($service in $services) {
    $status = kubectl get pods -l app=$service -n backend-management -o jsonpath="{.items[*].status.phase}"
    if ($status -ne "Running") {
        Write-Warning "警告: 服務 $service 狀態異常"
        kubectl describe pods -l app=$service -n backend-management
    }
}
```

### 5.2 效能診斷

```powershell
# 收集效能指標
kubectl top pods -n backend-management

# 檢查資源使用量
kubectl describe nodes | Select-String -Pattern "Allocated resources"
```

## 6. 安全性設定

### 6.1 網路安全

- 啟用 HTTPS
- 設定 CORS
- 啟用 Rate Limiting
- 設定網路策略

```powershell
# 安裝 cert-manager
helm install cert-manager jetstack/cert-manager `
    --namespace cert-manager `
    --create-namespace `
    --set installCRDs=true

# 設定 TLS 憑證
kubectl apply -f - <<EOF
apiVersion: cert-manager.io/v1
kind: Certificate
metadata:
  name: backend-tls
  namespace: backend-management
spec:
  secretName: backend-tls
  issuerRef:
    name: letsencrypt-prod
    kind: ClusterIssuer
  dnsNames:
  - api.example.com
EOF

# 設定網路策略
kubectl apply -f - <<EOF
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: backend-network-policy
  namespace: backend-management
spec:
  podSelector:
    matchLabels:
      app: backend-management
  policyTypes:
  - Ingress
  - Egress
  ingress:
  - from:
    - namespaceSelector:
        matchLabels:
          name: frontend
    ports:
    - protocol: TCP
      port: 80
EOF
```

### 6.2 存取控制

- JWT 驗證
- 角色權限
- IP 白名單
- 資源限制

```powershell
# 設定 RBAC
kubectl apply -f - <<EOF
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  name: backend-role
  namespace: backend-management
rules:
- apiGroups: [""]
  resources: ["pods", "services"]
  verbs: ["get", "list", "watch"]
---
apiVersion: rbac.authorization.k8s.io/v1
kind: RoleBinding
metadata:
  name: backend-role-binding
  namespace: backend-management
subjects:
- kind: ServiceAccount
  name: backend-service-account
  namespace: backend-management
roleRef:
  kind: Role
  name: backend-role
  apiGroup: rbac.authorization.k8s.io
EOF

# 設定資源限制
kubectl apply -f - <<EOF
apiVersion: v1
kind: ResourceQuota
metadata:
  name: backend-quota
  namespace: backend-management
spec:
  hard:
    requests.cpu: "4"
    requests.memory: 8Gi
    limits.cpu: "8"
    limits.memory: 16Gi
EOF
```

## 7. 聯絡支援

- 技術支援: support@example.com
- 緊急聯絡: (02) 1234-5678
- 文件網站: https://docs.example.com

## 8. 更新歷史

- 2024-03-20: 初始版本
- 2024-03-21: 新增災難復原程序
- 2024-03-22: 更新監控設定
