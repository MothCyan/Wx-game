# YooAsset WebGL 资源部署脚本

<#
.SYNOPSIS
    自动部署 YooAsset WebGL 构建输出到服务器

.DESCRIPTION
    此脚本将 Unity 构建的 WebGL 资源包复制到服务器指定目录
    确保服务器文件结构与 YooAsset 期望的路径一致

.PARAMETER SourcePath
    Unity 构建输出路径（默认: Bundles/WebGL）

.PARAMETER ServerPath
    服务器目标路径（需要修改为您的实际服务器路径）

.EXAMPLE
    .\Deploy-YooAsset.ps1
    使用默认路径部署资源

.EXAMPLE
    .\Deploy-YooAsset.ps1 -ServerPath "\\192.168.100.121\www\ServerFile"
    部署到指定的网络共享路径
#>

param(
    [string]$SourcePath = "d:\hub\WX-game\Bundles\WebGL",
    [string]$ServerPath = "C:\inetpub\wwwroot\ServerFile",  # 修改为您的实际服务器路径
    [switch]$WhatIf = $false  # 如果设置，只显示操作但不实际执行
)

# 颜色输出函数
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

# 检查源路径
if (-not (Test-Path $SourcePath)) {
    Write-ColorOutput "❌ 错误: 源路径不存在: $SourcePath" "Red"
    Write-ColorOutput "请先在 Unity 中构建 WebGL 资源包" "Yellow"
    exit 1
}

Write-ColorOutput "`n========================================" "Cyan"
Write-ColorOutput "YooAsset WebGL 资源部署工具" "Cyan"
Write-ColorOutput "========================================`n" "Cyan"

Write-ColorOutput "📁 源路径: $SourcePath" "Gray"
Write-ColorOutput "📁 目标路径: $ServerPath" "Gray"
Write-ColorOutput ""

# 检查是否存在 MyPackage 目录
$packagePath = Join-Path $SourcePath "MyPackage"
if (-not (Test-Path $packagePath)) {
    Write-ColorOutput "❌ 错误: 找不到 MyPackage 目录" "Red"
    Write-ColorOutput "预期路径: $packagePath" "Yellow"
    exit 1
}

# 创建服务器目录结构
$targetWebGLPath = Join-Path $ServerPath "WebGL"
$targetPackagePath = Join-Path $targetWebGLPath "MyPackage"

if ($WhatIf) {
    Write-ColorOutput "🔍 [预览模式] 将要执行的操作:" "Yellow"
    Write-ColorOutput "   创建目录: $targetPackagePath" "Gray"
    Write-ColorOutput "   复制文件: $packagePath -> $targetPackagePath" "Gray"
} else {
    try {
        # 创建目标目录
        Write-ColorOutput "📂 创建目标目录..." "Yellow"
        if (-not (Test-Path $targetPackagePath)) {
            New-Item -Path $targetPackagePath -ItemType Directory -Force | Out-Null
            Write-ColorOutput "✓ 创建目录: $targetPackagePath" "Green"
        } else {
            Write-ColorOutput "✓ 目录已存在: $targetPackagePath" "Green"
        }

        # 复制文件
        Write-ColorOutput "`n📦 开始复制文件..." "Yellow"
        Copy-Item -Path $packagePath -Destination $targetWebGLPath -Recurse -Force
        Write-ColorOutput "✓ 文件复制完成" "Green"

        # 验证复制结果
        Write-ColorOutput "`n🔍 验证文件..." "Yellow"
        
        $versionFiles = Get-ChildItem -Path $targetPackagePath -Filter "*.version" -Recurse
        $bytesFiles = Get-ChildItem -Path $targetPackagePath -Filter "*.bytes" -Recurse
        $bundleFiles = Get-ChildItem -Path $targetPackagePath -Filter "*.bundle" -Recurse

        Write-ColorOutput "   版本文件: $($versionFiles.Count) 个" "Gray"
        Write-ColorOutput "   清单文件: $($bytesFiles.Count) 个" "Gray"
        Write-ColorOutput "   资源包: $($bundleFiles.Count) 个" "Gray"

        if ($versionFiles.Count -gt 0) {
            Write-ColorOutput "`n📋 版本信息:" "Cyan"
            foreach ($versionFile in $versionFiles) {
                $version = Get-Content $versionFile.FullName -Raw
                $relativePath = $versionFile.FullName.Replace($targetPackagePath, "").TrimStart("\")
                Write-ColorOutput "   $relativePath => 版本: $($version.Trim())" "Gray"
            }
        }

    } catch {
        Write-ColorOutput "`n❌ 部署失败: $_" "Red"
        exit 1
    }
}

# 显示访问URL
Write-ColorOutput "`n========================================" "Cyan"
Write-ColorOutput "🎉 部署完成！" "Green"
Write-ColorOutput "========================================`n" "Cyan"

Write-ColorOutput "📝 服务器文件结构:" "Yellow"
Write-ColorOutput "   ServerFile/" "Gray"
Write-ColorOutput "   └── WebGL/" "Gray"
Write-ColorOutput "       └── MyPackage/" "Gray"
Write-ColorOutput "           ├── 2026-xx-xx-xxx/" "Gray"
Write-ColorOutput "           │   ├── MyPackage.version" "Gray"
Write-ColorOutput "           │   ├── MyPackage_xxx.bytes" "Gray"
Write-ColorOutput "           │   └── MyPackage_xxx.hash" "Gray"
Write-ColorOutput "           └── OutputCache/" "Gray"
Write-ColorOutput "               └── *.bundle" "Gray"

Write-ColorOutput "`n🌐 测试访问URL:" "Yellow"
Write-ColorOutput "   http://192.168.100.121/ServerFile/WebGL/MyPackage/2026-02-20-607/MyPackage.version" "Cyan"

Write-ColorOutput "`n💡 提示:" "Yellow"
Write-ColorOutput "   1. 确保 IIS/Apache/Nginx 已正确配置" "Gray"
Write-ColorOutput "   2. 确保防火墙允许访问" "Gray"
Write-ColorOutput "   3. 如果需要跨域，请配置 CORS" "Gray"
Write-ColorOutput "   4. 在浏览器中测试上面的 URL 是否可访问" "Gray"

Write-ColorOutput "`n========================================`n" "Cyan"

# 可选：在浏览器中打开测试页面
$openBrowser = Read-Host "是否在浏览器中测试访问? (Y/N)"
if ($openBrowser -eq "Y" -or $openBrowser -eq "y") {
    # 查找最新的版本目录
    $latestVersion = Get-ChildItem -Path $targetPackagePath -Directory | 
                     Where-Object { $_.Name -match "^\d{4}-\d{2}-\d{2}-\d+" } |
                     Sort-Object Name -Descending |
                     Select-Object -First 1

    if ($latestVersion) {
        $testUrl = "http://192.168.100.121/ServerFile/WebGL/MyPackage/$($latestVersion.Name)/MyPackage.version"
        Write-ColorOutput "🌐 打开浏览器测试: $testUrl" "Cyan"
        Start-Process $testUrl
    }
}
