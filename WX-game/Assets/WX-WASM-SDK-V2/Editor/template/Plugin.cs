using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WeChatWASM;

public class Plugin : LifeCycleBase
{
    public override void exportDone()
    {
        Debug.Log("当前进度完全导出，执行预下载资源操作");

        string path = BuildTemplateHelper.DstMinigameDir;
        //替换game.js的启动增加预下载
        string gameJsPath = path + "\\game.js";
        string gamejsText = System.IO.File.ReadAllText(gameJsPath);
        gamejsText = gamejsText.Replace(
            "gameManager.startGame();",
            "//预下载启动流程\r\n" +
            "        wx.request({\r\n" +
            "          url: GameGlobal.managerConfig.DATA_CDN + '/StreamingAssets/PreloadData.json',\r\n" +
            "          success(res) {\r\n" +
            "            console.log('获取预下载资源状态 ' + res.statusCode);\r\n" +
            "            if (res.statusCode === 200) {\r\n" +
            "              console.log('预下载资源配置 ' + res.data);\r\n" +
            "              GameGlobal.manager.setPreloadList(res.data);\r\n" +
            "            }\r\n" +
            "          },\r\n" +
            "          complete()\r\n" +
            "          {\r\n" +
            "            // 开始执行游戏启动流程\r\n" +
            "            gameManager.startGame();\r\n" +
            "          }\r\n" +
            "        });");
        File.WriteAllText(gameJsPath, gamejsText);
        Debug.Log("完成GameJS代码自动替换");

        WXPreloadJsonGenrate.GenrateJson(Directory.GetParent(path).ToString());
    }
}
