# 微信小游戏 YooAsset 部署完整方案

## 🎯 问题分析

### 当前错误
```
GET http://192.168.100.121/ServerFile/WebGL/MyPackage/MyPackage.version
404 (Not Found)
```

### 配置对比

**微信小游戏配置（WXEditorScriptObject）：**
- CDN: `http://192.168.100.121/ServerFile/`
- Bundle Path Identifier: `yoo;`

**代码配置（HotUpdateLoad.cs）：**
- HostHttp: `http://192.168.100.121/ServerFile/`
- PackageName: `MyPackage`

### 问题根源

YooAsset 会自动拼接路径：
```
{HostHttp} + {自动生成的相对路径}
```

对于微信小游戏，路径结构应该是：
```
http://192.168.100.121/ServerFile/yoo/{文件相对路径}
```

但实际请求的是：
```
http://192.168.100.121/ServerFile/WebGL/MyPackage/MyPackage.version
```

## ✅ 正确的解决方案

### 方案 A：修改服务器文件结构（推荐）

#### 1. 确定当前构建输出
```
d:\hub\WX-game\Bundles\WebGL\MyPackage\
├── 2026-02-20-607\
│   ├── MyPackage.version
│   ├── MyPackage_2026-02-20-607.bytes
│   ├── MyPackage_2026-02-20-607.hash
│   └── MyPackage_2026-02-20-607.json
└── OutputCache\
    ├── assets_dll.bundle
    └── assets_prefs.bundle
```

#### 2. 部署到服务器

**目标结构（根据微信配置）：**
```
服务器根目录\ServerFile\yoo\
├── 2026-02-20-607\
│   ├── MyPackage.version
│   ├── MyPackage_2026-02-20-607.bytes
│   ├── MyPackage_2026-02-20-607.hash
│   └── MyPackage_2026-02-20-607.json
└── OutputCache\
    ├── assets_dll.bundle
    └── assets_prefs.bundle
```

**PowerShell 部署命令：**
```powershell
# 设置路径
$sourcePath = "d:\hub\WX-game\Bundles\WebGL\MyPackage"
$targetPath = "\\192.168.100.121\ServerFile\yoo"

# 创建目录
New-Item -Path $targetPath -ItemType Directory -Force

# 复制所有文件
Copy-Item -Path "$sourcePath\*" -Destination $targetPath -Recurse -Force

Write-Host "部署完成！" -ForegroundColor Green
Write-Host "测试 URL: http://192.168.100.121/ServerFile/yoo/2026-02-20-607/MyPackage.version"
```

#### 3. 修改代码以匹配微信配置

保持 `HostHttp` 不变，微信小游戏会自动处理路径：
```csharp
public string HostHttp = "http://192.168.100.121/ServerFile/";
```

### 方案 B：修改微信小游戏配置

如果您想使用 `WebGL/MyPackage` 路径结构：

#### 1. 修改 CDN 配置

在微信小游戏配置中：
```
CDN: http://192.168.100.121/ServerFile/WebGL/MyPackage/
```

#### 2. 部署文件

```
服务器根目录\ServerFile\WebGL\MyPackage\
├── 2026-02-20-607\
│   ├── MyPackage.version
│   └── ...
└── OutputCache\
    └── ...
```

#### 3. 修改 Bundle Path Identifier（可选）

如果需要特殊标识，可以改为：
```
Bundle Path Identifier: WebGL/MyPackage;
```

## 🚀 推荐方案：统一路径配置

### 步骤 1：确定统一的路径策略

**选择 A（推荐）：使用 yoo 作为资源根目录**
```
CDN: http://192.168.100.121/ServerFile/
Bundle Path: yoo;
代码 HostHttp: http://192.168.100.121/ServerFile/
```

**选择 B：使用平台特定目录**
```
CDN: http://192.168.100.121/ServerFile/
Bundle Path: WebGL/MyPackage;
代码 HostHttp: http://192.168.100.121/ServerFile/
```

### 步骤 2：创建自动化部署脚本

保存为 `Deploy-WX-YooAsset.ps1`：

```powershell
<#
.SYNOPSIS
    微信小游戏 YooAsset 资源部署脚本
#>

param(
    [string]$SourcePath = "d:\hub\WX-game\Bundles\WebGL\MyPackage",
    [string]$ServerPath = "\\192.168.100.121\ServerFile",
    [string]$BundlePath = "yoo",  # 与微信配置中的 Bundle Path Identifier 对应
    [switch]$WhatIf = $false
)

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "微信小游戏 YooAsset 资源部署工具" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# 检查源路径
if (-not (Test-Path $SourcePath)) {
    Write-Host "❌ 错误: 源路径不存在: $SourcePath" -ForegroundColor Red
    exit 1
}

# 目标路径
$targetPath = Join-Path $ServerPath $BundlePath

Write-Host "📁 源路径: $SourcePath" -ForegroundColor Gray
Write-Host "📁 目标路径: $targetPath" -ForegroundColor Gray
Write-Host ""

if ($WhatIf) {
    Write-Host "🔍 [预览模式] 将要执行的操作:" -ForegroundColor Yellow
    Write-Host "   创建目录: $targetPath" -ForegroundColor Gray
    Write-Host "   复制文件: $SourcePath\* -> $targetPath\" -ForegroundColor Gray
} else {
    try {
        # 创建目标目录
        Write-Host "📂 创建目标目录..." -ForegroundColor Yellow
        New-Item -Path $targetPath -ItemType Directory -Force | Out-Null
        Write-Host "✓ 目录就绪" -ForegroundColor Green

        # 复制文件
        Write-Host "`n📦 开始复制文件..." -ForegroundColor Yellow
        Copy-Item -Path "$SourcePath\*" -Destination $targetPath -Recurse -Force
        Write-Host "✓ 文件复制完成" -ForegroundColor Green

        # 验证文件
        Write-Host "`n🔍 验证文件..." -ForegroundColor Yellow
        $versionFiles = Get-ChildItem -Path $targetPath -Filter "*.version" -Recurse
        $bytesFiles = Get-ChildItem -Path $targetPath -Filter "*.bytes" -Recurse
        $bundleFiles = Get-ChildItem -Path $targetPath -Filter "*.bundle" -Recurse

        Write-Host "   版本文件: $($versionFiles.Count) 个" -ForegroundColor Gray
        Write-Host "   清单文件: $($bytesFiles.Count) 个" -ForegroundColor Gray
        Write-Host "   资源包: $($bundleFiles.Count) 个" -ForegroundColor Gray

        if ($versionFiles.Count -gt 0) {
            Write-Host "`n📋 版本信息:" -ForegroundColor Cyan
            foreach ($versionFile in $versionFiles) {
                $version = Get-Content $versionFile.FullName -Raw
                $relativePath = $versionFile.FullName.Replace($targetPath, "").TrimStart("\")
                Write-Host "   $relativePath => 版本: $($version.Trim())" -ForegroundColor Gray
                
                # 构建测试 URL
                $testUrl = "http://192.168.100.121/ServerFile/$BundlePath/$relativePath"
                Write-Host "   测试 URL: $testUrl" -ForegroundColor Cyan
            }
        }

        Write-Host "`n========================================" -ForegroundColor Cyan
        Write-Host "🎉 部署完成！" -ForegroundColor Green
        Write-Host "========================================`n" -ForegroundColor Cyan

    } catch {
        Write-Host "`n❌ 部署失败: $_" -ForegroundColor Red
        exit 1
    }
}
```

### 步骤 3：执行部署

```powershell
# 进入项目目录
cd d:\hub\WX-game

# 使用 yoo 路径（推荐）
.\Deploy-WX-YooAsset.ps1 -BundlePath "yoo"

# 或使用 WebGL/MyPackage 路径
.\Deploy-WX-YooAsset.ps1 -BundlePath "WebGL/MyPackage"
```

### 步骤 4：验证部署

在浏览器中测试以下 URL（根据您选择的方案）：

**方案 A（yoo）：**
```
http://192.168.100.121/ServerFile/yoo/2026-02-20-607/MyPackage.version
```

**方案 B（WebGL/MyPackage）：**
```
http://192.168.100.121/ServerFile/WebGL/MyPackage/2026-02-20-607/MyPackage.version
```

## 📝 配置对照表

### 方案 A：使用 yoo 目录（推荐）

| 配置项 | 值 |
|--------|-----|
| 微信 CDN | `http://192.168.100.121/ServerFile/` |
| 微信 Bundle Path | `yoo;` |
| 代码 HostHttp | `http://192.168.100.121/ServerFile/` |
| 服务器路径 | `ServerFile/yoo/` |
| 访问 URL | `http://192.168.100.121/ServerFile/yoo/{version}/MyPackage.version` |

### 方案 B：使用 WebGL/MyPackage 目录

| 配置项 | 值 |
|--------|-----|
| 微信 CDN | `http://192.168.100.121/ServerFile/WebGL/MyPackage/` |
| 微信 Bundle Path | `yoo;` 或留空 |
| 代码 HostHttp | `http://192.168.100.121/ServerFile/WebGL/MyPackage/` |
| 服务器路径 | `ServerFile/WebGL/MyPackage/` |
| 访问 URL | `http://192.168.100.121/ServerFile/WebGL/MyPackage/{version}/MyPackage.version` |

## 🔧 当前问题的快速修复

根据您的截图，最快的修复方法：

### 1. 不修改微信配置

保持微信配置不变：
```
CDN: http://192.168.100.121/ServerFile/
Bundle Path: yoo;
```

### 2. 部署文件到 yoo 目录

```powershell
# 复制文件
Copy-Item -Path "d:\hub\WX-game\Bundles\WebGL\MyPackage\*" `
          -Destination "\\192.168.100.121\ServerFile\yoo\" `
          -Recurse -Force
```

### 3. 验证

访问：`http://192.168.100.121/ServerFile/yoo/2026-02-20-607/MyPackage.version`

## ⚠️ 重要提醒

1. **路径一致性**：确保微信配置、代码配置、服务器文件路径三者一致
2. **不要包含包名两次**：如果 CDN 已经有包名，代码中就不要再加
3. **Bundle Path Identifier**：这个字段在微信小游戏中很重要，要与服务器目录对应
4. **测试验证**：部署后务必在浏览器中测试 URL 是否可访问

## 📞 调试检查清单

- [ ] 确认 Unity 构建输出目录
- [ ] 确认微信 CDN 配置
- [ ] 确认 Bundle Path Identifier
- [ ] 确认代码中的 HostHttp
- [ ] 确认服务器文件路径
- [ ] 在浏览器测试版本文件 URL
- [ ] 在微信开发者工具测试加载

---

**需要帮助？** 请告诉我：
1. 您想使用哪个方案（A 或 B）
2. 您的服务器是什么类型（IIS/Apache/Nginx）
3. 是否能访问服务器文件系统
