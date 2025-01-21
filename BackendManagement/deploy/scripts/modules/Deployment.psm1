using module "./Constants.psm1"
using module "./Validation.psm1"

function Initialize-Deployment {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Environment,
        [Parameter(Mandatory)]
        [string]$ConfigPath
    )

    # 驗證前置條件
    Test-Prerequisites
    $config = Test-Configuration -ConfigPath $ConfigPath

    # 建立命名空間
    foreach ($ns in $NAMESPACES) {
        Write-Host "建立命名空間: $ns"
        kubectl create namespace $ns --dry-run=client -o yaml | kubectl apply -f -
    }

    # 設定密鑰
    Set-DeploymentSecrets -Config $config -Namespace "backend-management"

    return $config
}

function Set-DeploymentSecrets {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [PSCustomObject]$Config,
        [Parameter(Mandatory)]
        [string]$Namespace
    )

    # 從 Key Vault 取得密鑰
    $secrets = @{
        "db-password" = Get-KeyVaultSecret -VaultName $Config.keyVault.name -SecretName "db-password"
        "redis-password" = Get-KeyVaultSecret -VaultName $Config.keyVault.name -SecretName "redis-password"
        "jwt-secret" = Get-KeyVaultSecret -VaultName $Config.keyVault.name -SecretName "jwt-secret"
    }

    # 建立 Kubernetes Secrets
    $secretsYaml = @"
apiVersion: v1
kind: Secret
metadata:
  name: backend-secrets
  namespace: $Namespace
type: Opaque
data:
  db-password: $([Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($secrets["db-password"])))
  redis-password: $([Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($secrets["redis-password"])))
  jwt-secret: $([Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($secrets["jwt-secret"])))
"@

    $secretsYaml | kubectl apply -f -
}

function Install-Dependencies {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Namespace,
        [Parameter(Mandatory)]
        [PSCustomObject]$Config
    )

    # 部署資料庫
    Write-Host "部署 PostgreSQL..."
    helm upgrade --install backend-db bitnami/postgresql `
        --namespace $Namespace `
        --values deploy/helm/postgresql-values.yaml `
        --wait

    # 部署 Redis
    Write-Host "部署 Redis..."
    helm upgrade --install backend-redis bitnami/redis `
        --namespace $Namespace `
        --values deploy/helm/redis-values.yaml `
        --wait

    # 驗證服務健康狀態
    $services = @("backend-db", "backend-redis")
    foreach ($service in $services) {
        if (-not (Test-ServiceHealth -Namespace $Namespace -ServiceName $service)) {
            throw ($ERROR_MESSAGES.DEPLOYMENT_FAILED -f $service)
        }
    }
}

function Deploy-Application {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Namespace,
        [Parameter(Mandatory)]
        [string]$Version,
        [Parameter(Mandatory)]
        [PSCustomObject]$Config
    )

    # 替換部署檔案中的變數
    $deploymentYaml = Get-Content "deploy/k8s/backend-management.yaml" -Raw
    $deploymentYaml = $deploymentYaml.Replace('${REGISTRY_URL}', $Config.registry.url)
    $deploymentYaml = $deploymentYaml.Replace('${VERSION}', $Version)

    # 部署應用程式
    $deploymentYaml | kubectl apply -f - -n $Namespace

    # 等待部署完成
    kubectl rollout status deployment/backend-management -n $Namespace

    # 驗證應用程式健康狀態
    if (-not (Test-ServiceHealth -Namespace $Namespace -ServiceName "backend-management")) {
        throw ($ERROR_MESSAGES.DEPLOYMENT_FAILED -f "backend-management")
    }
}

Export-ModuleMember -Function Initialize-Deployment, Install-Dependencies, Deploy-Application 