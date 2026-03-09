using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HybridCLR;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;
using System.IO;

public class HotUpdateLoad : MonoBehaviour
{
    public string GameRoot = "GameRoot";
    public ResourcePackage Package;
    // public ResourcePackage ScenePackage;
    public string HostHttp = "http://192.168.100.121/ServerFile/";
    public EPlayMode LoadMode = EPlayMode.EditorSimulateMode;

    [Header("下载进度UI")]
    public Image ProgressBarFill;   // Image 的 Type 设为 Filled，Fill Method 设为 Horizontal
    public Text ProgressText;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(Init());
    }
    void Start()
    {
        // 注意：不要在这里调用 Load()，因为 package 还没初始化完成
        // StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        
        var hotUpdateAss = Package.LoadAssetAsync<TextAsset>("Hotupdate.dll");
        yield return hotUpdateAss;
        byte[] dllBytes = (hotUpdateAss.AssetObject as TextAsset).bytes;
        Assembly hotUpdateAsss = Assembly.Load(dllBytes);


        
        yield return LoadScene("TestScene");
        yield return null;
        var handle = Package.LoadAssetAsync<GameObject>(GameRoot);
        yield return handle;
        Debug.Log($"加载对象完毕。状态{handle.Status}");
        if (handle.Status == EOperationStatus.Succeed)
        {
            handle.InstantiateSync();
            
        }
        Debug.Log("Load方法执行完毕"); Debug.Log("Load方法执行完毕"); Debug.Log("Load方法执行完毕"); Debug.Log("Load方法执行完毕");
    }

    // Start is called before the first frame update
    IEnumerator Init()
    {
        // 初始化资源系统
        YooAssets.Initialize();

        // 创建并初始化 Package
        Package = YooAssets.TryGetPackage("MyPackage");
        if (Package == null)
            Package = YooAssets.CreatePackage("MyPackage");

        YooAssets.SetDefaultPackage(Package);
        // 创建并初始化 ScenePackage
        // ScenePackage = YooAssets.TryGetPackage("ScenePackage");
        // if (ScenePackage == null)
        //     ScenePackage = YooAssets.CreatePackage("ScenePackage");

        // 开始初始化流程
        yield return LoadPackage(Package, "MyPackage");

        var operation = Package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        yield return operation;
        if (operation.Status == EOperationStatus.Succeed)
        {
            Debug.Log("清理缓存文件成功");
        }
        else
        {
            Debug.LogError($"清理缓存文件失败：{operation.Error}");
        }
        yield return LoadMetadataForAOTAssemblies(Package);

        // yield return LoadPackage(ScenePackage, "ScenePackage");
        yield return Load();
        
    }

    public IEnumerator LoadPackage(ResourcePackage package, string packageName)
    {
        // 注意：package 已经在 Awake 中创建好了，这里直接使用

        if (LoadMode == EPlayMode.EditorSimulateMode)
        {
            yield return OnEditorSimulsteMode(package, packageName);
        }
        else if (LoadMode == EPlayMode.OfflinePlayMode)
        {
            yield return OnOfflinePlayMode(package, packageName);
        }
        else if (LoadMode == EPlayMode.HostPlayMode)
        {
            yield return OnHostPlayMode(package, packageName);
        }
        else if (LoadMode == EPlayMode.WebPlayMode)
        {
            yield return OnWebPlayMode(package, packageName);
        }
        else if (LoadMode == EPlayMode.CustomPlayMode)
        {
            yield return OnCustomMode(package, packageName);
        }
        yield return null;

    }
    public IEnumerator LoadScene(string sceneName)
    {
        // YooAssets.RemovePackage(Package);
        // Package.DestroyAsync();

        // var allPackage = YooAssets.GetAllPackages();
        // foreach(var package in allPackage)
        // {
        //     Debug.Log($"包1：{package.PackageName}");
        //     Debug.Log($"包2：{package.GetPackageDetails()}");
        //     Debug.Log($"包3：{package.GetPackageNote()}");
        //     Debug.Log($"包4：{package.GetPackageVersion()}");

        //     var packageAsset = package.GetAllAssetInfos();
        //     foreach(var asset in packageAsset)
        //     {
        //         Debug.Log($"资源：{asset.AssetType} - {asset.AssetPath}");
        //     }
        // }

        SceneHandle handle2 = Package.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        yield return handle2;
    }


    public IEnumerator OnEditorSimulsteMode(ResourcePackage package, string PackageName)
    {
        YooAssets.Initialize();
        var buildResult = EditorSimulateModeHelper.SimulateBuild(PackageName);
        var createParameters = new EditorSimulateModeParameters();
        createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(buildResult.PackageRootDirectory);
        var initOp = package.InitializeAsync(createParameters);
        yield return initOp;
        if (initOp.Status != EOperationStatus.Succeed) { Debug.LogError($"初始化失败：{initOp.Error}"); yield break; }
        Debug.Log($"加载模式{LoadMode}资源包初始化成功");

        yield return RequestAndUpdateManifest(package);
        yield return DownloadAssets(package);
    }

    public IEnumerator OnOfflinePlayMode(ResourcePackage package, string PackageName)
    {
        Debug.Log("开始初始化单机模式");
        var createParameters = new OfflinePlayModeParameters();
        createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        var initOp = package.InitializeAsync(createParameters);
        yield return initOp;
        if (initOp.Status != EOperationStatus.Succeed) { Debug.LogError($"初始化失败：{initOp.Error}"); yield break; }
        Debug.Log($"加载模式{LoadMode}资源包初始化成功");

        yield return RequestAndUpdateManifest(package);
        yield return DownloadAssets(package);
    }

    public IEnumerator OnHostPlayMode(ResourcePackage package, string PackageName)
    {
        IRemoteServices remoteServices = new RemoteServices(HostHttp, HostHttp);
        var createParameters = new HostPlayModeParameters();
        createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        createParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
        var initOp = package.InitializeAsync(createParameters);
        yield return initOp;
        if (initOp.Status != EOperationStatus.Succeed) { Debug.LogError($"资源包初始化失败：{initOp.Error}"); yield break; }
        Debug.Log("资源包初始化成功！");

        yield return RequestAndUpdateManifest(package);
        yield return DownloadAssets(package);
    }

    public IEnumerator OnWebPlayMode(ResourcePackage package, string PackageName)
    {
        string hostServer = $"{HostHttp}WebGL/{PackageName}";
        IRemoteServices remoteServices = new RemoteServices(hostServer, hostServer);
        var createParameters = new WebPlayModeParameters();
        createParameters.WebServerFileSystemParameters = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
        createParameters.WebRemoteFileSystemParameters = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices);
        var initOp = package.InitializeAsync(createParameters);
        yield return initOp;
        if (initOp.Status != EOperationStatus.Succeed) { Debug.LogError($"资源包初始化失败：{initOp.Error}"); yield break; }
        Debug.Log("资源包初始化成功");

        yield return RequestAndUpdateManifest(package);
        yield return DownloadAssets(package);
    }

    /// <summary>
    /// 微信小游戏专用模式 - 使用微信文件系统
    /// 注意：此方法在编辑器中会显示部分API错误，但不影响构建和运行
    /// </summary>
    public IEnumerator OnCustomMode(ResourcePackage package, string packageName)
    {
        Debug.Log("=== 开始微信小游戏模式初始化 ===");
        var createParameters = new WebPlayModeParameters();

#if UNITY_WEBGL && WEIXINMINIGAME && !UNITY_EDITOR
        string defaultHostServer = HostHttp + "/StreamingAssets/yoo/MyPackage";
        string fallbackHostServer = defaultHostServer;
        string packageRoot = $"{WeChatWASM.WX.env.USER_DATA_PATH}/__GAME_FILE_CACHE/StreamingAssets/yoo/MyPackage";
        Debug.Log($"CDN服务器: {defaultHostServer}");
        Debug.Log($"本地缓存路径: {packageRoot}");
        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
        createParameters.WebServerFileSystemParameters = WechatFileSystemCreater.CreateFileSystemParameters(packageRoot, remoteServices);
#else
        createParameters.WebServerFileSystemParameters = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
#endif

        var initOp = package.InitializeAsync(createParameters);
        yield return initOp;
        if (initOp.Status != EOperationStatus.Succeed) { Debug.LogError($"资源包初始化失败：{initOp.Error}"); yield break; }
        Debug.Log("✓ 资源包初始化成功");

        yield return RequestAndUpdateManifest(package);
        yield return DownloadAssets(package);

        Debug.Log("=== 开始加载游戏资源 ===");
    }

    /// <summary>请求版本号并更新清单</summary>
    private IEnumerator RequestAndUpdateManifest(ResourcePackage package)
    {
        var versionOp = package.RequestPackageVersionAsync();
        yield return versionOp;
        if (versionOp.Status != EOperationStatus.Succeed) { Debug.LogError($"请求版本号失败：{versionOp.Error}"); yield break; }
        Debug.Log($"✓ 版本号: {versionOp.PackageVersion}");

        var manifestOp = package.UpdatePackageManifestAsync(versionOp.PackageVersion);
        yield return manifestOp;
        if (manifestOp.Status != EOperationStatus.Succeed) { Debug.LogError($"更新清单失败：{manifestOp.Error}"); yield break; }
        Debug.Log("✓ 清单更新成功");
    }

    /// <summary>创建下载器并执行下载</summary>
    private IEnumerator DownloadAssets(ResourcePackage package)
    {
        var downloader = package.CreateResourceDownloader(10, 3);
        Debug.Log($"需要更新文件数量: {downloader.TotalDownloadCount}");
        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("✓ 无需下载资源");
            SetProgressUI(1f, "资源已是最新");
            yield break;
        }

        SetProgressUI(0f, "开始下载资源...");

        downloader.DownloadFinishCallback = r => Debug.Log($"下载完成 {r.PackageName} -> {r.Succeed}");
        downloader.DownloadErrorCallback = r => Debug.LogError($"下载错误 {r.FileName} -> {r.ErrorInfo}");
        downloader.DownloadUpdateCallback = r =>
        {
            Debug.Log($"下载进度: {r.Progress:P2} | {r.CurrentDownloadCount}/{r.TotalDownloadCount}");
            SetProgressUI(r.Progress, $"下载中 {r.CurrentDownloadCount}/{r.TotalDownloadCount}  ({FormatBytes(r.CurrentDownloadBytes)}/{FormatBytes(r.TotalDownloadBytes)})");
        };
        downloader.DownloadFileBeginCallback = r =>
        {
            Debug.Log($"开始下载: {r.FileName} ({r.FileSize} bytes)");
            SetProgressUI(ProgressBarFill != null ? ProgressBarFill.fillAmount : 0f, $"正在下载: {r.FileName}");
        };

        downloader.BeginDownload();
        yield return downloader;

        if (downloader.Status != EOperationStatus.Succeed)
        {
            Debug.LogError("下载失败");
            SetProgressUI(0f, "下载失败！");
            yield break;
        }
        SetProgressUI(1f, "下载完成 ✓");
        Debug.Log("✓ 资源下载完成");
    }

    private void SetProgressUI(float progress, string message)
    {
        if (ProgressBarFill != null)
            ProgressBarFill.fillAmount = progress;
        if (ProgressText != null)
            ProgressText.text = message;
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes >= 1024 * 1024) return $"{bytes / (1024f * 1024f):F1} MB";
        if (bytes >= 1024) return $"{bytes / 1024f:F1} KB";
        return $"{bytes} B";
    }

    private static IEnumerator LoadMetadataForAOTAssemblies(ResourcePackage package)
    {

        // AOTGenericReferences.PatchedAOTAssemblyList
        // List<string> aotDllList = new List<string>
        // {
        //     "UnityEngine.CoreModule.dll",
        //     "mscorlib.dll",
        //     "Cinemachine.dll",
            // // 核心运行时库
            // "mscorlib.dll",
            // "System.dll",
            // "System.Core.dll",
            
            // // Unity 核心模块（修复 Transform.Translate 等 API）
            // "UnityEngine.CoreModule.dll",
            // "UnityEngine.dll",
            
            // // Unity 常用模块
            // "UnityEngine.UI.dll",                    // UGUI
            // "UnityEngine.PhysicsModule.dll",         // 物理系统
            // "UnityEngine.Physics2DModule.dll",       // 2D 物理
            // //"UnityEngine.AnimationModule.dll",       // 动画系统
            // //"UnityEngine.AudioModule.dll",           // 音频系统
            // //"UnityEngine.ParticleSystemModule.dll",  // 粒子系统
            // //"UnityEngine.InputLegacyModule.dll",     // 输入系统
            // //"UnityEngine.TextRenderingModule.dll",   // 文本渲染
            // //"UnityEngine.IMGUIModule.dll",           // IMGUI
            
            // // .NET 扩展库
            // //"System.Xml.dll",
            // //"System.Numerics.dll",
            
            // // 第三方库（如果使用）
            // // "Newtonsoft.Json.dll",                // JSON 序列化
            // // "protobuf-net.dll",                   // Protobuf
            // // "DOTween.dll",                        // DOTween 动画库
        // };

        var aotDllList = AOTGenericReferences.PatchedAOTAssemblyList;
        foreach (var aotDllName in aotDllList)
        {
            var dllHandle = package.LoadAssetAsync<TextAsset>(aotDllName);
            yield return dllHandle;

            if(dllHandle.Status == EOperationStatus.Succeed)
            {
                byte[] dllBytes = (dllHandle.AssetObject as TextAsset).bytes;   
                LoadImageErrorCode err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
            }
            else
            {
                Debug.LogError($"加载 {aotDllName} 失败 {dllHandle.LastError}");
            }
        }
    }



    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }

    }


}
