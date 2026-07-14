param(
    [string]$BaseUrl = "http://localhost:5193"
)

$passed = 0
$failed = 0
$results = @()

function Check-Url {
    param($Name, $Url, $ExpectedStatus = 200)
    try {
        $response = Invoke-WebRequest -Uri "$BaseUrl$Url" -UseBasicParsing -TimeoutSec 10
        if ($response.StatusCode -eq $ExpectedStatus) {
            $results += [PSCustomObject]@{ Name = $Name; Status = "PASS"; Detail = "$($response.StatusCode) $($response.StatusDescription)" }
            $global:passed++
        } else {
            $results += [PSCustomObject]@{ Name = $Name; Status = "FAIL"; Detail = "Expected $ExpectedStatus, got $($response.StatusCode)" }
            $global:failed++
        }
    } catch {
        $results += [PSCustomObject]@{ Name = $Name; Status = "FAIL"; Detail = "Error: $_" }
        $global:failed++
    }
}

Write-Host "=== 烟雾测试 - 图书馆座位预约系统 ===" -ForegroundColor Cyan
Write-Host "Base URL: $BaseUrl`n" -ForegroundColor Gray

# 1. 首页
Check-Url -Name "首页 (GET /)" -Url "/"

# 2. 首页可包含关键文字
try {
    $response = Invoke-WebRequest -Uri "$BaseUrl/" -UseBasicParsing -TimeoutSec 10
    if ($response.Content -match "图书馆座位预约系统") {
        $results += [PSCustomObject]@{ Name = "首页包含标题文字"; Status = "PASS"; Detail = "关键字匹配" }
        $passed++
    } else {
        $results += [PSCustomObject]@{ Name = "首页包含标题文字"; Status = "FAIL"; Detail = "未找到关键字" }
        $failed++
    }
} catch {
    $results += [PSCustomObject]@{ Name = "首页包含标题文字"; Status = "FAIL"; Detail = "Error: $_" }
    $failed++
}

# 3. 座位列表 (需要先登录, 直接检查302跳转)
Check-Url -Name "座位列表 (GET /Seats - 未登录)" -Url "/Seats" -ExpectedStatus 302

# 4. 管理端登录页
Check-Url -Name "管理端登录 (GET /Admin/Login)" -Url "/Admin/Login"

# 5. 管理端预约管理 (未登录应302)
Check-Url -Name "管理端预约管理 (GET /Admin/Reservations - 未登录)" -Url "/Admin/Reservations" -ExpectedStatus 302

# 6. 管理端统计 (未登录应302)
Check-Url -Name "管理端统计 (GET /Admin/Statistics - 未登录)" -Url "/Admin/Statistics" -ExpectedStatus 302

# 7. 管理端座位管理 (未登录应302)
Check-Url -Name "管理端座位管理 (GET /Admin/Seats - 未登录)" -Url "/Admin/Seats" -ExpectedStatus 302

# 8. 静态资源可访问
Check-Url -Name "Bootstrap CSS (静态资源)" -Url "/lib/bootstrap/dist/css/bootstrap.min.css"

# 9. 我的预约 (未登录应302)
Check-Url -Name "我的预约 (GET /Reservations/My - 未登录)" -Url "/Reservations/My" -ExpectedStatus 302

Write-Host "`n=== 测试结果 ===" -ForegroundColor Cyan
$results | Format-Table Name, Status, Detail -AutoSize

Write-Host "`n总计: $($passed+$failed) | 通过: $passed | 失败: $failed" -ForegroundColor $(if ($failed -eq 0) { "Green" } else { "Red" })

if ($failed -gt 0) {
    exit 1
}
