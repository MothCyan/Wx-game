using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HybridCLR;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using System.IO;

public class HotUpdateLoad : MonoBehaviour
{
    public string GameRoot = "GameRoot";
    public ResourcePackage Package;
    // public ResourcePackage ScenePackage;
    public string HostHttp = "http://192.168.100.121/ServerFile/";
    public EPlayMode LoadMode = EPlayMode.EditorSimulateMode;
    void Awake()
    {
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


        var handle = Package.LoadAssetAsync<GameObject>(GameRoot);
        yield return handle;
        Debug.Log($"加载对象完毕。状态{handle.Status}");
        if (handle.Status == EOperationStatus.Succeed)
        {
            Instantiate(handle.InstantiateSync());
            
        }
        yield return LoadScene("TestScene");
        yield return null;
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
        // 初始化资源系统
        YooAssets.Initialize();

        // 创建包容器

        // 根据 LoadMode 选择不同的加载流程
        if (LoadMode == EPlayMode.EditorSimulateMode)
        {
            // ========== 编辑器模拟模式流程 ==========
            // 配置包容器
            Debug.Log($"initializationOperation");
            InitializationOperation initializationOperation = null;
            var buildResult = EditorSimulateModeHelper.SimulateBuild(PackageName);
            var packageRoot = buildResult.PackageRootDirectory;
            var createParameters = new EditorSimulateModeParameters();
            createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            initializationOperation = package.InitializeAsync(createParameters);
            yield return initializationOperation;
            if (initializationOperation.Status == EOperationStatus.Succeed)
                Debug.Log($"加载模式{LoadMode}资源包初始化成功！{package.PackageValid}");
            else
                Debug.LogError($"加载模式{LoadMode}资源包初始化失败：{initializationOperation.Error}");

            // 请求包体版本号
            var operation = package.RequestPackageVersionAsync();
            yield return operation;
            var packageVersion = operation.PackageVersion;

            // 更新包体清单
            Debug.Log($"UpdatePackageManifestAsync");
            var operation2 = package.UpdatePackageManifestAsync(packageVersion);
            yield return operation2;

            //创建下载器
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            // 初始化完成后，开始加载资源
            Debug.Log("Package 初始化完成，开始加载资源");


        }
    }
    public IEnumerator OnOfflinePlayMode(ResourcePackage package, string PackageName)

    {
        // 3. 配置本地 "包容器" - 单机模式
        Debug.Log("开始初始化单机模式");
        InitializationOperation initializationOperation = null;
        var createParameters = new OfflinePlayModeParameters();
        createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        initializationOperation = package.InitializeAsync(createParameters);
        yield return initializationOperation;

        if (initializationOperation.Status == EOperationStatus.Succeed)
            Debug.Log($"加载模式{LoadMode}资源包初始化成功！PackageValid: {package.PackageValid}");
        else
        {
            Debug.LogError($"加载模式{LoadMode}资源包初始化失败：{initializationOperation.Error}");
            yield break;
        }
        // 请求包体版本号
        var operation = package.RequestPackageVersionAsync();
        yield return operation;
        var packageVersion = operation.PackageVersion;

        // 更新包体清单
        var operation2 = package.UpdatePackageManifestAsync(packageVersion);
        yield return operation2;

        //创建下载器
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

    }
    public IEnumerator OnHostPlayMode(ResourcePackage package, string PackageName)
    {
        //这个翻译看起来 就是默认服务器，就是访问包清单时，优先选择的服务器
        string defaultHostServer = HostHttp;
        //这个fallback，即失败时返回。就是访问默认服务器失败时，用这个链接。
        string fallbackHostServer = HostHttp;
        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
        var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
        var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

        var createParameters = new HostPlayModeParameters();
        createParameters.BuildinFileSystemParameters = buildinFileSystemParams;
        createParameters.CacheFileSystemParameters = cacheFileSystemParams;

        var initOperation = package.InitializeAsync(createParameters);
        yield return initOperation;

        if (initOperation.Status == EOperationStatus.Succeed)
            Debug.Log("资源包初始化成功！");
        else
        {
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
            yield break;
        }


        //这里，Status应该成功才走
        // 请求包体版本号
        var operation = package.RequestPackageVersionAsync();
        yield return operation; //等待请求包体版本号完成

        var packageVersion = operation.PackageVersion;
        Debug.Log($"RequestPackageVersionAsync -> {operation.Status} 版本号{packageVersion} ");
        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"请求包体版本号失败：{operation.Error}");
            yield break;
        }

        // 更新包体清单
        var operation2 = package.UpdatePackageManifestAsync(packageVersion);
        yield return operation2; //等待更新包体清单完成

        Debug.Log($"UpdatePackageManifestAsync -> {operation2.Status} 清单更新完成");

        if (operation2.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"更新包体清单失败：{operation2.Error}");
            yield break;
        }

        //创建下载器
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        Debug.Log($"当前需要更新文件数量 -> {downloader.TotalDownloadCount}");
        if (downloader.TotalDownloadCount != 0)
        {
            //需要下载的文件总数和总大小
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;
            //注册回调方法
            downloader.DownloadFinishCallback = (result) =>
            {

                Debug.Log($"DownloadFinishCallback callback {result.PackageName} -> {result.Succeed}");
            }; //当下载器结束（无论成功或失败）
            downloader.DownloadErrorCallback = (result) =>
            {
                string Error = $"DownloadErrorCallback callback {result.PackageName} -> {result.ErrorInfo}->{result.FileName}";
                Debug.Log(Error);
            }; //当下载器发生错误
            downloader.DownloadUpdateCallback = (result) =>
            {
                //获取下载的字节
                string bytesMsg = $"{result.CurrentDownloadBytes} / {result.TotalDownloadBytes}";
                //获取下载数量
                string countMsg = $"{result.CurrentDownloadCount} / {result.TotalDownloadCount}";
                string msg = $"DownloadUpdateCallback callback";
                msg += $"\n{result.PackageName} -> {result.Progress}";
                msg += "\n" + countMsg;
                msg += "\n" + bytesMsg;
                Debug.Log(msg);
                //这样就能得到所有回调给的数据了。也许玩家不需要看，但是开发者你需要。
            }; //当下载进度发生变化
            downloader.DownloadFileBeginCallback = (result) =>
            {

                string PackageName = $"DownloadFileBeginCallback callback {result.PackageName} -> {result.FileName} ->{result.FileSize}";
                Debug.Log(PackageName);
            }; //当开始下载某个文件

            //开启下载
            downloader.BeginDownload();
            yield return downloader;

            //检测下载结果
            Debug.Log($"BeginDownload。状态{downloader.Status}");

            if (downloader.Status != EOperationStatus.Succeed)
            {
                yield break;
            }
            Debug.Log("完成");


        }

    }
    public IEnumerator OnWebPlayMode(ResourcePackage package, string PackageName)
    {
        // WebPlayMode 需要完整的包路径
        // 服务器上应该有: ServerFile/WebGL/MyPackage/ 目录
        string defaultHostServer = $"{HostHttp}WebGL/{PackageName}";
        string fallbackHostServer = $"{HostHttp}WebGL/{PackageName}";

        Debug.Log($"=== WebPlayMode 初始化 ===");
        Debug.Log($"defaultHostServer: {defaultHostServer}");
        Debug.Log($"fallbackHostServer: {fallbackHostServer}");

        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
        var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
        var webRemoteFileSystemParams = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); //支持跨域下载

        var createParameters = new WebPlayModeParameters();
        createParameters.WebServerFileSystemParameters = webServerFileSystemParams;
        createParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;

        var initOperation = package.InitializeAsync(createParameters);
        yield return initOperation;

        if (initOperation.Status == EOperationStatus.Succeed)
        {
            Debug.Log("资源包初始化成功");
        }
        else
        {
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
            yield break;
        }
        //这里，Status应该成功才走
        // 请求包体版本号
        var operation = package.RequestPackageVersionAsync();
        yield return operation; //等待请求包体版本号完成

        var packageVersion = operation.PackageVersion;
        Debug.Log($"RequestPackageVersionAsync -> {operation.Status} 版本号{packageVersion} ");
        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"请求包体版本号失败：{operation.Error}");
            yield break;
        }

        // 更新包体清单
        var operation2 = package.UpdatePackageManifestAsync(packageVersion);
        yield return operation2; //等待更新包体清单完成

        Debug.Log($"UpdatePackageManifestAsync -> {operation2.Status} 清单更新完成");

        if (operation2.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"更新包体清单失败：{operation2.Error}");
            yield break;
        }

        //创建下载器
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        Debug.Log($"当前需要更新文件数量 -> {downloader.TotalDownloadCount}");
        if (downloader.TotalDownloadCount != 0)
        {
            //需要下载的文件总数和总大小
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;
            //注册回调方法
            downloader.DownloadFinishCallback = (result) =>
            {

                Debug.Log($"DownloadFinishCallback callback {result.PackageName} -> {result.Succeed}");
            }; //当下载器结束（无论成功或失败）
            downloader.DownloadErrorCallback = (result) =>
            {
                string Error = $"DownloadErrorCallback callback {result.PackageName} -> {result.ErrorInfo}->{result.FileName}";
                Debug.Log(Error);
            }; //当下载器发生错误
            downloader.DownloadUpdateCallback = (result) =>
            {
                //获取下载的字节
                string bytesMsg = $"{result.CurrentDownloadBytes} / {result.TotalDownloadBytes}";
                //获取下载数量
                string countMsg = $"{result.CurrentDownloadCount} / {result.TotalDownloadCount}";
                string msg = $"DownloadUpdateCallback callback";
                msg += $"\n{result.PackageName} -> {result.Progress}";
                msg += "\n" + countMsg;
                msg += "\n" + bytesMsg;
                Debug.Log(msg);
                //这样就能得到所有回调给的数据了。也许玩家不需要看，但是开发者你需要。
            }; //当下载进度发生变化
            downloader.DownloadFileBeginCallback = (result) =>
            {

                string PackageName = $"DownloadFileBeginCallback callback {result.PackageName} -> {result.FileName} ->{result.FileSize}";
                Debug.Log(PackageName);
            }; //当开始下载某个文件

            //开启下载
            downloader.BeginDownload();
            yield return downloader;

            //检测下载结果
            Debug.Log($"BeginDownload。状态{downloader.Status}");

            if (downloader.Status != EOperationStatus.Succeed)
            {
                yield break;
            }
            Debug.Log("完成");


        }


    }

    /// <summary>
    /// 微信小游戏专用模式 - 使用微信文件系统
    /// 注意：此方法在编辑器中会显示部分API错误，但不影响构建和运行
    /// </summary>
    public IEnumerator OnCustomMode(ResourcePackage package, string packageName)
    {
        Debug.Log("=== 开始微信小游戏模式初始化 ===");

        InitializationOperation initializationOperation = null;
        var createParameters = new WebPlayModeParameters();

#if UNITY_WEBGL && WEIXINMINIGAME && !UNITY_EDITOR
        // 微信小游戏环境 - 使用微信文件系统
        Debug.Log("使用微信文件系统");
        
        //这里是CDN目录
        string defaultHostServer = HostHttp;
        string fallbackHostServer = HostHttp;

        //这里是文件缓冲目录
        string packageRoot =
            $"{WeChatWASM.WX.env.USER_DATA_PATH}/__GAME_FILE_CACHE";
        // 重要：添加 /yoo 子目录以匹配 Bundle Path Identifier

        //这里是热更文件的目录，拼接一下
        defaultHostServer += "/StreamingAssets/yoo/MyPackage"; //注意尾部不能带/
        fallbackHostServer += "/StreamingAssets/yoo/MyPackage"; //注意尾部不能带/
        packageRoot += "/StreamingAssets/yoo/MyPackage"; //注意尾部不能带/
        
        //实际上就是一个目录位置而已，可以不拼，
        // 但是转为小游戏时候已经有一个目录结构了。
        // 那么直接使用转换的目录接口即可
        //转换的webgl目录就是放置到CDN的目录；
        //直接丢过去方便。而加载资源就需要进入到这个目录下。的子目录。
        //
        //这里比较讲究
        //CDN抬头 http://192.168.100.121/ServerFile;
        //下面是完整路径，那么host路径需要拼接/StreamingAssets/yoo/MyPackage
        //http://192.168.100.121/ServerFile/StreamingAssets/yoo/MyPackage
        //此处的root也需要拼接 一模一样的路径/StreamingAssets/yoo/MyPackage
        //末尾不能带/
        // 后续路径拼接需要相同
        



        Debug.Log($"CDN服务器: {defaultHostServer}");
        Debug.Log($"本地缓存路径: {packageRoot}");
        
        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
        createParameters.WebServerFileSystemParameters = WechatFileSystemCreater.CreateFileSystemParameters(packageRoot, remoteServices);
#else
        // 编辑器或其他环境 - 使用标准文件系统
        Debug.Log("使用标准 WebGL 文件系统");
        createParameters.WebServerFileSystemParameters = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
#endif

        // 1. 初始化包
        initializationOperation = package.InitializeAsync(createParameters);
        yield return initializationOperation;

        if (initializationOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"资源包初始化失败：{initializationOperation.Error}");
            yield break;
        }
        Debug.Log("✓ 资源包初始化成功");

        // 2. 请求版本号
        var versionOperation = package.RequestPackageVersionAsync();
        yield return versionOperation;

        if (versionOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"请求版本号失败：{versionOperation.Error}");
            yield break;
        }

        string packageVersion = versionOperation.PackageVersion;
        Debug.Log($"✓ 获取版本号成功: {packageVersion}");

        // 3. 更新清单
        var updateOperation = package.UpdatePackageManifestAsync(packageVersion);
        yield return updateOperation;

        if (updateOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"更新清单失败：{updateOperation.Error}");
            yield break;
        }
        Debug.Log("✓ 清单更新成功");

        // 4. 创建下载器检查是否需要下载
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        Debug.Log($"当前需要更新文件数量: {downloader.TotalDownloadCount}");

        if (downloader.TotalDownloadCount > 0)
        {
            Debug.Log($"需要下载文件数量: {downloader.TotalDownloadCount}");
            Debug.Log($"需要下载大小: {downloader.TotalDownloadBytes} bytes");

            // 注册回调
            downloader.DownloadFinishCallback = (result) =>
            {
                Debug.Log($"下载完成 {result.PackageName} -> {result.Succeed}");
            };

            downloader.DownloadErrorCallback = (result) =>
            {
                Debug.LogError($"下载错误 {result.PackageName} -> {result.ErrorInfo} -> {result.FileName}");
            };

            downloader.DownloadUpdateCallback = (result) =>
            {
                string bytesMsg = $"{result.CurrentDownloadBytes} / {result.TotalDownloadBytes}";
                string countMsg = $"{result.CurrentDownloadCount} / {result.TotalDownloadCount}";
                string msg = $"下载进度: {result.Progress:P2}";
                msg += $"\n文件数: {countMsg}";
                msg += $"\n大小: {bytesMsg}";
                Debug.Log(msg);
            };

            downloader.DownloadFileBeginCallback = (result) =>
            {
                Debug.Log($"开始下载: {result.FileName} ({result.FileSize} bytes)");
            };

            // 开始下载
            downloader.BeginDownload();
            yield return downloader;

            if (downloader.Status != EOperationStatus.Succeed)
            {
                Debug.LogError("下载失败");
                yield break;
            }
            Debug.Log("✓ 资源下载完成");
        }
        else
        {
            Debug.Log("✓ 无需下载资源");
        }

        // 5. 所有准备工作完成，开始加载资源
        Debug.Log("=== 开始加载游戏资源 ===");

    }

    private static IEnumerator LoadMetadataForAOTAssemblies(ResourcePackage package)
    {


        List<string> aotDllList = new List<string>
        {
            "UnityEngine.CoreModule.dll",
            "mscorlib.dll",
            "Cinemachine.dll",
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
        };


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
