# YooAsset 多平台自动适配使用说明

## 🎯 问题解决

您之前遇到的问题：
- ❌ 在 Unity Editor 中无法使用，因为 `WechatFileSystemCreater` 不可用
- ❌ 在微信开发者工具中也无法加载

**现在的解决方案：**
- ✅ Unity Editor 自动使用 `EditorSimulateMode`
- ✅ 微信小游戏自动使用 `OnCustomMode`（微信文件系统）
- ✅ 普通 WebGL 自动使用 `OnWebPlayMode`
- ✅ 移动端/PC端自动使用 `HostPlayMode`

## 📋 配置步骤

### 步骤 1: 配置 Scripting Define Symbols

**必须配置，否则微信小游戏无法工作！**

1. 打开 `Edit` > `Project Settings` > `Player`
2. 选择 `WebGL` 平台标签
3. 展开 `Other Settings` 部分
4. 找到 `Scripting Define Symbols`
5. 添加以下宏定义：
   ```
   WEIXINMINIGAME
   ```
6. 点击 Apply 或者按 Enter 确认

### 步骤 2: 验证配置

在 Unity Console 中查看日志，确认环境检测正确：

```
=== 开始初始化 YooAsset ===
检测到 Unity Editor 环境
```

或在 WebGL 构建后：
```
=== 开始初始化 YooAsset ===
检测到 WebGL 平台
是否在微信小游戏环境: True
使用微信小游戏专用模式
```

## 🚀 工作原理

### 自动适配流程

```
启动游戏
    ↓
检测运行环境
    ↓
┌──────────────────┬─────────────────┬──────────────────┐
│  Unity Editor    │  WebGL 平台     │  其他平台        │
│                  │                 │                  │
│ EditorSimulate   │ 检测微信环境?   │  HostPlayMode    │
│     Mode         │    ↙    ↘      │   或              │
│                  │  是      否     │  OfflinePlayMode │
│                  │  ↓       ↓      │                  │
│                  │ Custom WebPlay  │                  │
│                  │  Mode    Mode   │                  │
└──────────────────┴─────────────────┴──────────────────┘
```

### 代码逻辑

```csharp
void Awake()
{
    // 自动检测并选择合适的加载模式
    StartCoroutine(InitializeYooAsset());
}

IEnumerator InitializeYooAsset()
{
#if UNITY_EDITOR
    // 编辑器模式
    yield return OnEditorSimulsteMode();
    
#elif UNITY_WEBGL
    // WebGL 平台
    bool isWeixinMiniGame = IsWeixinMiniGame();
    
    if (isWeixinMiniGame)
    {
        // 微信小游戏
        yield return OnCustomMode();
    }
    else
    {
        // 普通 WebGL
        yield return OnWebPlayMode();
    }
    
#else
    // 移动端/PC端
    yield return OnHostPlayMode();
#endif
}
```

## 📱 不同环境的行为

### 1. Unity Editor
- ✅ 使用 `EditorSimulateMode`
- ✅ 不需要构建资源包
- ✅ 直接从项目中加载资源
- ✅ 快速迭代开发

**日志输出：**
```
=== 开始初始化 YooAsset ===
检测到 Unity Editor 环境
initializationOperation
加载模式EditorSimulateMode资源包初始化成功！
```

### 2. 微信小游戏
- ✅ 自动检测 `WEIXINMINIGAME` 宏
- ✅ 使用 `WechatFileSystemCreater`
- ✅ 缓存路径：`{USER_DATA_PATH}/__GAME_FILE_CACHE/yoo`
- ✅ 支持微信文件系统优化

**日志输出：**
```
=== 开始初始化 YooAsset ===
检测到 WebGL 平台
是否在微信小游戏环境: True
使用微信小游戏专用模式
=== 微信小游戏专用模式 ===
微信缓存路径: /data/__GAME_FILE_CACHE/yoo
✓ 微信小游戏资源包初始化成功
```

### 3. 普通 WebGL（浏览器）
- ✅ 使用 `WebPlayMode`
- ✅ 标准 WebGL 文件系统
- ✅ 支持跨域下载

**日志输出：**
```
=== 开始初始化 YooAsset ===
检测到 WebGL 平台
是否在微信小游戏环境: False
使用标准 WebGL 模式
defaultHostServer: http://192.168.100.121/ServerFile/
资源包初始化成功
```

### 4. 移动端/PC端
- ✅ 使用 `HostPlayMode`
- ✅ 支持本地缓存
- ✅ 支持热更新

## ⚙️ 高级配置

### 手动指定加载模式（不推荐）

如果您需要在某个平台强制使用特定模式：

```csharp
void Awake()
{
    YooAssets.Initialize();
    package = YooAssets.CreatePackage(PackageName);
    
    // 手动指定模式
    if (LoadMode == EPlayMode.EditorSimulateMode)
    {
        StartCoroutine(OnEditorSimulsteMode());
    }
    else if (LoadMode == EPlayMode.WebPlayMode)
    {
        StartCoroutine(OnWebPlayMode());
    }
    // ... 其他模式
}
```

### 调试模式

添加详细的日志输出：

```csharp
void Start()
{
    Debug.Log("=== 环境信息 ===");
    Debug.Log($"Application.platform: {Application.platform}");
    Debug.Log($"Application.isEditor: {Application.isEditor}");
    
#if UNITY_EDITOR
    Debug.Log("✓ UNITY_EDITOR 已定义");
#endif

#if UNITY_WEBGL
    Debug.Log("✓ UNITY_WEBGL 已定义");
#endif

#if WEIXINMINIGAME
    Debug.Log("✓ WEIXINMINIGAME 已定义");
#endif
}
```

## 🔍 常见问题

### Q1: Unity Editor 中显示红色波浪线

**现象：**
```csharp
WechatFileSystemCreater.CreateFileSystemParameters(...)
// 显示：当前上下文中不存在名称"WechatFileSystemCreater"
```

**原因：** 
这是正常的！因为 `OnCustomMode()` 内部的微信相关代码被 `#if UNITY_WEBGL && WEIXINMINIGAME && !UNITY_EDITOR` 包裹，在编辑器中不会编译。

**解决方案：**
- ✅ 这不会影响编辑器运行
- ✅ 编辑器会自动使用 `EditorSimulateMode`
- ✅ 构建到 WebGL 时会正确编译

### Q2: 微信小游戏中资源加载失败

**检查清单：**

1. **是否定义了 WEIXINMINIGAME 宏？**
   ```
   Edit > Project Settings > Player > WebGL > Other Settings
   Scripting Define Symbols: WEIXINMINIGAME
   ```

2. **查看日志输出**
   ```
   是否在微信小游戏环境: True
   使用微信小游戏专用模式
   ```

3. **检查服务器文件**
   - 确认服务器上有正确的资源文件
   - 验证 URL 可以访问

4. **检查缓存路径**
   ```
   微信缓存路径: /data/__GAME_FILE_CACHE/yoo
   ```

### Q3: 普通浏览器中无法加载

**检查：**

1. 确认 `HostHttp` 配置正确
   ```csharp
   public string HostHttp = "http://192.168.100.121/ServerFile/";
   ```

2. 检查服务器 CORS 配置（跨域）

3. 查看浏览器控制台的网络请求

### Q4: 如何在不同环境测试？

**Unity Editor 测试：**
```
直接点击 Play 按钮
→ 自动使用 EditorSimulateMode
```

**微信小游戏测试：**
```
1. 配置 WEIXINMINIGAME 宏
2. Build 到 WebGL
3. 使用微信开发者工具打开
→ 自动使用 OnCustomMode
```

**浏览器测试：**
```
1. 不配置 WEIXINMINIGAME 宏（或配置但在非微信环境）
2. Build 到 WebGL
3. 在浏览器中打开
→ 自动使用 OnWebPlayMode
```

## 📊 决策树

```
游戏启动
    │
    ├─ 是 Unity Editor？
    │   └─ 是 → EditorSimulateMode ✓
    │
    ├─ 是 WebGL 平台？
    │   │
    │   ├─ 定义了 WEIXINMINIGAME 宏？
    │   │   ├─ 是 → OnCustomMode（微信文件系统） ✓
    │   │   └─ 否 → OnWebPlayMode（标准 WebGL） ✓
    │   │
    │   └─ OnWebPlayMode 作为后备 ✓
    │
    └─ 其他平台
        └─ HostPlayMode 或 OfflinePlayMode ✓
```

## ✅ 验证步骤

### 1. 验证编辑器模式

```
1. 在 Unity 中打开项目
2. 点击 Play
3. 查看 Console：
   ✓ 检测到 Unity Editor 环境
   ✓ 资源包初始化成功
```

### 2. 验证微信小游戏模式

```
1. Edit > Project Settings > Player > WebGL
2. 添加 WEIXINMINIGAME 到 Scripting Define Symbols
3. Build Settings > Platform: WebGL > Build
4. 使用微信开发者工具打开构建输出
5. 查看日志：
   ✓ 是否在微信小游戏环境: True
   ✓ 使用微信小游戏专用模式
   ✓ 微信小游戏资源包初始化成功
```

### 3. 验证浏览器模式

```
1. 确保 WEIXINMINIGAME 未定义（或移除）
2. Build 到 WebGL
3. 在浏览器中打开 index.html
4. 查看日志：
   ✓ 是否在微信小游戏环境: False
   ✓ 使用标准 WebGL 模式
   ✓ 资源包初始化成功
```

## 🎉 总结

**现在的代码可以：**
- ✅ 在 Unity Editor 中正常开发和测试
- ✅ 在微信小游戏中使用微信文件系统
- ✅ 在普通浏览器中使用标准 WebGL 模式
- ✅ 自动检测环境，无需手动切换
- ✅ 统一的代码库，多平台兼容

**您只需要：**
1. 配置 `WEIXINMINIGAME` 宏（仅微信小游戏需要）
2. 配置服务器地址 `HostHttp`
3. 运行即可，代码会自动适配！

---

**需要更多帮助？**
- 检查 Unity Console 的日志输出
- 验证 Scripting Define Symbols 配置
- 确认服务器文件部署正确
