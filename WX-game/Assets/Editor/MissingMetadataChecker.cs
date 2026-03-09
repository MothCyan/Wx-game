using UnityEditor;
using UnityEngine;
using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.HotUpdate;

public class MissingMetadataTools
{
    [MenuItem("HybridCLR/CheckMissingMetadata")]
    public static void CheckAccessMissingMetadata()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        // aotDir指向 构建主包时生成的裁剪aot dll目录，而不是最新的SettingsUtil.GetAssembliesPostIl2CppStripDir(target)目录。
        // 一般来说，发布热更新包时，由于中间可能调用过generate/all，SettingsUtil.GetAssembliesPostIl2CppStripDir(target)目录中包含了最新的aot dll，
        // 肯定无法检查出类型或者函数裁剪的问题。
        // 需要在构建完主包后，将当时的aot dll保存下来，供后面补充元数据或者裁剪检查。
        string aotDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);

        // 第2个参数hotUpdateAssNames为热更新程序集列表。对于旗舰版本，该列表需要包含DHE程序集，即SettingsUtil.HotUpdateAndDHEAssemblyNamesIncludePreserved。
        var checker = new MissingMetadataChecker(aotDir, SettingsUtil.HotUpdateAssemblyNamesIncludePreserved);

        string hotUpdateDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
        {
            string dllPath = $"{hotUpdateDir}/{dll}";
            bool notAnyMissing = checker.Check(dllPath);
            if (!notAnyMissing)
            {
                // DO SOMETHING
            }
        }
    }
        /*
    public static void CheckAccessMissingMetadata()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        
        Debug.Log("=== 开始检查热更新代码引用 ===");
        
        // 检查 AOT 目录是否存在
        string aotDir = $"HybridCLRData/AssembliesPostIl2CppStrip/{target}";
        if (!Directory.Exists(aotDir))
        {
            Debug.LogError($"AOT 目录不存在: {aotDir}");
            Debug.LogError("请先构建项目生成 AOT DLL！");
            return;
        }
        
        // 检查热更新 DLL 目录是否存在
        string hotUpdateDir = $"HybridCLRData/HotUpdateDlls/{target}";
        if (!Directory.Exists(hotUpdateDir))
        {
            Debug.LogError($"热更新 DLL 目录不存在: {hotUpdateDir}");
            Debug.LogError("请先执行 HybridCLR/Generate/All 生成热更新 DLL！");
            return;
        }
        
        // 列出 AOT DLL
        Debug.Log($"AOT DLL 目录: {aotDir}");
        var aotFiles = Directory.GetFiles(aotDir, "*.dll");
        Debug.Log($"找到 {aotFiles.Length} 个 AOT DLL:");
        foreach (var file in aotFiles)
        {
            Debug.Log($"  - {Path.GetFileName(file)}");
        }
        
        // 列出热更新 DLL
        Debug.Log($"\n热更新 DLL 目录: {hotUpdateDir}");
        var hotUpdateFiles = Directory.GetFiles(hotUpdateDir, "*.dll");
        Debug.Log($"找到 {hotUpdateFiles.Length} 个热更新 DLL:");
        foreach (var file in hotUpdateFiles)
        {
            Debug.Log($"  - {Path.GetFileName(file)}");
        }
        
        // 检查 link.xml
        string linkXmlPath = "Assets/HybridCLRGenerate/link.xml";
        if (File.Exists(linkXmlPath))
        {
            Debug.Log($"\n✓ link.xml 存在: {linkXmlPath}");
        }
        else
        {
            Debug.LogWarning($"⚠ link.xml 不存在: {linkXmlPath}");
        }
        
        Debug.Log("\n=== 检查完成 ===");
        Debug.Log("建议步骤:");
        Debug.Log("1. 手动设置: Edit -> Project Settings -> Player -> Other Settings -> Managed Stripping Level -> Minimal");
        Debug.Log("2. 确保所有热更新类都有 [Preserve] 特性");
        Debug.Log("3. 确保 link.xml 中包含了 Hotupdate 程序集保护");
        Debug.Log("4. 执行 HybridCLR/Generate/All 重新生成热更新 DLL");
        Debug.Log("5. 重新构建项目测试");
    }*/
}

