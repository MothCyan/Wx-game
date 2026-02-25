using HybridCLR.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WeChatWASM;

public class WXPreloadJsonGenrate : EditorWindow
{
    private static readonly string subPath = "/StreamingAssets";
    [MenuItem("热更新辅助工具/微信小游戏/生成预下载资源文件Json", priority = 200)]
    private static void Genrate()
    {
        // 加载插件配置
        string path = "Assets/WX-WASM-SDK-V2/Editor/MiniGameConfig.asset";
        WXEditorScriptObject config = AssetDatabase.LoadAssetAtPath<WXEditorScriptObject>(path);
        if(config == null )
        {
            Debug.LogError("加载wx小游戏配置失败，请检查目录:" + path);
            return;
        }
        Debug.Log("path " + config.ProjectConf.DST);
        GenrateJson(config.ProjectConf.DST, true);
    }


    public static void GenrateJson(string path, bool openJsonDirectories = false)
    {
        Debug.Log($"开始生成预下载资源文件Json,目标路径:{path}");
        string webglPath = path + "/webgl";
        string streamPath = webglPath + subPath;
        string yooPath = streamPath + "/yoo";
        string jsonPath = streamPath + "/PreloadData.json";

        string[] yooSubDirs = Directory.GetDirectories(yooPath);
        string json = "[\n";
        for (int i = 0; i < yooSubDirs.Length; i++)
        {
            string subDir = yooSubDirs[i];
            string dirName = Path.GetRelativePath(streamPath, subDir);

            //只需要bundle，其他文件没必要
            string[] yooSubFiles = Directory.GetFiles(subDir, "*.bundle");
            for (int j = 0; j < yooSubFiles.Length; j++)
            {
                string fileName = Path.GetFileName(yooSubFiles[j]);
                //提取名字组装json
                string relativePath = $"\\{dirName}\\{fileName}";
                json += $"  \"{relativePath.Replace("\\","/")}\"";
                //添加后缀，如果不是最后一个需要添加","
                if (j != yooSubFiles.Length - 1)
                {
                    json += ",";
                }
                //换行给人看
                json += "\n";
            }
        }
        json += "]";

        File.WriteAllText(jsonPath, json);
        Debug.Log("预下载Json：" + json);
        Debug.Log("导出预下载Json：" + jsonPath);

        if(openJsonDirectories)
        {
            System.Diagnostics.Process.Start(streamPath);
        }
    }
}
