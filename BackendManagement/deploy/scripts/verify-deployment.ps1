param(
    [string]$Namespace = "backend-management"
)

# 檢查服務狀態
$services = @(
    "backend-management",
    "backend-db",
    "backend-redis"
)

foreach ($service in $services) {
    Write-Host "檢查服務: $service"
    
    # 檢查 Pod 狀態
    $status = kubectl get pods -l app=$service -n $Namespace -o jsonpath="{.items[*].status.phase}"
    if ($status -ne "Running") {
        Write-Warning "警告: 服務 $service 狀態異常"
        kubectl describe pods -l app=$service -n $Namespace
    }
    
    # 檢查資源使用量
    kubectl top pods -l app=$service -n $Namespace
}

# 檢查健康狀態
$endpoint = kubectl get service backend-management -n $Namespace -o jsonpath="{.status.loadBalancer.ingress[0].ip}"
try {
    $health = Invoke-RestMethod "http://$endpoint/health"
    Write-Host "健康檢查結果: $($health.status)"
} catch {
    Write-Error "健康檢查失敗: $_"
} 