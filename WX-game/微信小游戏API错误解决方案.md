# 微信小游戏API错误解决方案

## 问题描述
在Unity WebGL构建中运行微信小游戏SDK时，出现以下错误：
- `[worker] getNetworkType:fail not support`
- `[worker] getPermissionBytes:fail not support`
- `[worker] getABTestConfig:fail not support`
- `[worker] Uncaught (in promise) Error: not support`

## 根本原因
这些错误是因为微信小游戏的API（如`wx.getNetworkType`、`wx.getPermissionBytes`等）只在**微信小游戏环境**中可用。当您在浏览器中直接运行WebGL构建时，这些API不存在，导致调用失败。

## 解决方案

### 方案1：在微信开发者工具中运行（推荐）

**这是正确的开发流程：**

1. **使用Unity导出微信小游戏**
   - 在Unity中，Build Settings选择微信小游戏平台
   - 点击Build导出项目

2. **在微信开发者工具中打开**
   - 打开微信开发者工具
   - 导入导出的小游戏项目
   - 在微信开发者工具中预览和调试

3. **真机测试**
   - 使用微信开发者工具的真机调试功能
   - 或生成体验版进行测试

### 方案2：在浏览器中测试（需要Mock API）

如果您需要在浏览器中快速测试（不推荐用于正式开发），需要添加API兼容层：

1. **已创建Mock适配器**
   文件位置：`Assets/WebGLTemplates/WX-Mock-Adapter.js`
   
   这个文件提供了微信API的Mock实现，让代码在浏览器中不会报错。

2. **修改WebGL模板**
   需要在WebGL模板的HTML文件中引入这个适配器：

   在`Assets/WebGLTemplates/WXTemplate/index.html`（或您使用的模板）的`<head>`标签中添加：
   
   ```html
   <!-- 微信API Mock适配器（仅用于浏览器测试） -->
   <script src="WX-Mock-Adapter.js"></script>
   ```

   **重要：** 确保这个脚本在所有其他微信SDK脚本之前加载！

3. **重新构建项目**
   - 在Unity中重新Build WebGL项目
   - Mock适配器会自动检测环境：
     - 在微信环境中：不会执行Mock，使用真实API
     - 在浏览器环境中：提供Mock API，避免报错

### 方案3：条件编译（推荐用于生产环境）

在Unity C#代码中，使用条件编译来避免在非微信环境调用API：

```csharp
#if WEIXINMINIGAME
    // 只在微信小游戏平台编译和执行
    WX.GetNetworkType(new GetNetworkTypeOption()
    {
        success = (result) =>
        {
            Debug.Log("Network Type: " + result.networkType);
        },
        fail = (result) =>
        {
            Debug.LogWarning("GetNetworkType failed: " + result.errMsg);
        }
    });
#else
    Debug.Log("Not in WeChat MiniGame environment");
#endif
```

## 建议的开发流程

1. **开发阶段**
   - 在Unity Editor中开发和测试基本功能
   - 使用条件编译隔离平台特定代码

2. **测试阶段**
   - 构建为微信小游戏平台
   - 在微信开发者工具中测试
   - 使用真机调试验证功能

3. **发布阶段**
   - 通过微信开发者工具提交审核
   - 发布正式版本

## 注意事项

1. **不要在生产环境使用Mock适配器**
   - Mock适配器仅用于开发测试
   - 返回的是假数据，不能用于实际业务逻辑

2. **API版本兼容性**
   - 检查微信小游戏SDK版本
   - 确保使用的API在目标版本中支持

3. **错误处理**
   - 所有微信API调用都应该有fail回调
   - 妥善处理API调用失败的情况

## 常见问题

**Q: 为什么在浏览器中看到这些错误？**
A: 因为浏览器中没有微信小游戏的运行环境，wx对象和相关API不存在。

**Q: 这些错误会影响微信小游戏正式运行吗？**
A: 不会。在微信环境中，这些API都是可用的，不会出现这些错误。

**Q: 可以忽略这些错误吗？**
A: 如果您确定最终在微信环境中运行，可以暂时忽略。但建议使用上述方案进行处理，以便更好地进行本地开发和调试。

## 相关文档

- [微信小游戏开发文档](https://developers.weixin.qq.com/minigame/dev/guide/)
- [Unity微信小游戏适配方案](https://github.com/wechat-miniprogram/minigame-unity-webgl-transform)
- [微信开发者工具下载](https://developers.weixin.qq.com/miniprogram/dev/devtools/download.html)

---

## 🆕 WechatFileSystemCreater 无法识别问题

### 问题描述
在 `HotUpdateLoad.cs` 中使用 `WechatFileSystemCreater` 时，Unity Editor 显示错误：
```
当前上下文中不存在名称"WechatFileSystemCreater"
```

### 根本原因
`WechatFileSystemCreater` 类在 `Assets/WechatFileSystem/WechatFileSystem.cs` 中被预编译指令包裹：

```csharp
#if UNITY_WEBGL && WEIXINMINIGAME
public static class WechatFileSystemCreater { ... }
#endif
```

**这意味着：**
- ✅ 只有在 **WebGL 平台** + **定义了 WEIXINMINIGAME 宏** 时才会编译
- ❌ 在 Unity Editor 中默认不会编译，所以 IDE 会显示错误提示

### 解决方案

#### 1. 添加预编译指令（已修复）

在 `HotUpdateLoad.cs` 顶部添加命名空间引用：
```csharp
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HybridCLR;
using Unity.VisualScripting;
using UnityEngine;
using YooAsset;
#if UNITY_WEBGL && WEIXINMINIGAME
using WeChatWASM;
#endif
```

在 `OnCustomMode` 方法上添加预编译指令：
```csharp
#if UNITY_WEBGL && WEIXINMINIGAME
/// <summary>
/// 微信小游戏专用模式 - 使用微信文件系统
/// </summary>
public IEnumerator OnCustomMode()
{
    string defaultHostServer = HostHttp;
    string fallbackHostServer = HostHttp;
    var remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);

    // 小游戏缓存根目录
    string packageRoot = $"{WeChatWASM.WX.env.USER_DATA_PATH}/__GAME_FILE_CACHE/yoo";

    // 创建初始化参数
    var createParameters = new WebPlayModeParameters();
    createParameters.WebServerFileSystemParameters = WechatFileSystemCreater.CreateFileSystemParameters(packageRoot, remoteServices, null);

    // 初始化ResourcePackage
    var initOperation = package.InitializeAsync(createParameters);
    yield return initOperation;
    
    // ... 后续代码
}
#endif
```

#### 2. 配置平台宏定义

**步骤：**
1. 打开 `Edit` > `Project Settings` > `Player`
2. 选择 `WebGL` 平台
3. 展开 `Other Settings`
4. 在 `Scripting Define Symbols` 中添加：
   ```
   WEIXINMINIGAME
   ```
5. 点击 Apply

#### 3. 理解为什么编辑器中显示错误

**这是正常的！** 因为：
- Unity Editor 默认不是 WebGL 平台
- 代码用 `#if UNITY_WEBGL && WEIXINMINIGAME` 包裹
- 在非目标平台中，这段代码不会编译
- IDE 无法解析未编译的代码，所以显示错误

**但是：**
- ✅ 不影响编辑器中运行其他模式
- ✅ 构建到 WebGL 平台时会正确编译
- ✅ 在微信小游戏中会正常工作

### 使用建议

#### 开发时自动选择模式

修改 `Awake()` 方法，让代码自动选择正确的加载模式：

```csharp
void Awake()
{
    YooAssets.Initialize();
    package = YooAssets.TryGetPackage(PackageName);
    if (package == null)
        package = YooAssets.CreatePackage(PackageName);

    // 根据平台自动选择加载模式
#if UNITY_EDITOR
    StartCoroutine(OnEditorSimulsteMode());
#elif UNITY_WEBGL && WEIXINMINIGAME
    StartCoroutine(OnCustomMode()); // 微信小游戏专用
#elif UNITY_WEBGL
    StartCoroutine(OnWebPlayMode()); // 标准 WebGL
#else
    StartCoroutine(OnHostPlayMode()); // 移动端/PC端
#endif

    LoadMetadataForAOTAssemblies();
}
```

### 调试技巧

检查当前平台和宏定义：

```csharp
void Start()
{
    Debug.Log("=== 平台信息 ===");
    
#if UNITY_EDITOR
    Debug.Log("✓ 当前在 Unity Editor 中");
#endif

#if UNITY_WEBGL
    Debug.Log("✓ UNITY_WEBGL 已定义");
#else
    Debug.Log("✗ UNITY_WEBGL 未定义");
#endif

#if WEIXINMINIGAME
    Debug.Log("✓ WEIXINMINIGAME 已定义");
    Debug.Log($"微信用户数据路径: {WeChatWASM.WX.env.USER_DATA_PATH}");
#else
    Debug.Log("✗ WEIXINMINIGAME 未定义");
#endif

#if UNITY_WEBGL && WEIXINMINIGAME
    Debug.Log("✓✓ 微信小游戏环境完全启用");
#endif
}
```

### 总结

✅ **已修复的问题：**
- 添加了 `#if UNITY_WEBGL && WEIXINMINIGAME` 预编译指令
- `OnCustomMode()` 方法仅在微信小游戏平台编译
- 编辑器中的错误提示不影响实际运行

✅ **正常现象：**
- Unity Editor 中看到错误提示是正常的
- 这不会影响其他平台的开发和构建
- 构建到 WebGL + 微信平台时会自动使用正确的代码

✅ **使用流程：**
1. 开发时在编辑器中使用 `EditorSimulateMode`
2. 构建 WebGL 时自动切换到 `WebPlayMode`
3. 构建微信小游戏时自动使用 `OnCustomMode`

---

**需要更多帮助？**
- 检查 Platform 是否设置为 WebGL
- 验证 Scripting Define Symbols 包含 WEIXINMINIGAME
- 确认微信小游戏插件已正确安装
