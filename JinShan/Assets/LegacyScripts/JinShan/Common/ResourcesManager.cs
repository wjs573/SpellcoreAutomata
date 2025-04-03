using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
namespace JinShan
{
    /// <summary>
    /// 资源管理器——管理资源映射表（资源名称-资源完整路径）
    /// 技能系统中，我们要把所有技能都放在一个目录下，用字符串拼接处文件路径，
    /// 然后用resource.load进行加载。但是，我们不可能把所有技能资源都放在一个
    /// 固定的地方。因此，我们需要有一个根据资源名字加载资源的管理类。
    /// </summary>
    public class ResourcesManager
    {
        private static Dictionary<string, string> configMap;
        //初始构造函数，初始化类的静态数据成员，类被加载时执行一次
        static ResourcesManager()
        {
            string fileContent = GetConfigFile("mapConfig.txt");
            BuildMap(fileContent);
        }

        //加载文件
        public static string GetConfigFile(string fileName)
        {
            string url;
#if UNITY_EDITOR
            url = "file://" + Application.dataPath + "/StreamingAssets/" + fileName;
#elif UNITY_IPHONE
            url = "file://"+Application.dataPath + "/Raw/"+fileName;
#elif UNITY_ANDROID
            url = "jar:file://" + Application.dataPath + "!/assets/"+fileName;
#endif
            //Debug.Log(url);
            UnityWebRequest webRequest = new UnityWebRequest(url);
            //Debug.Log(webRequest.downloadedBytes.ToString());
            //return null;
            webRequest.SendWebRequest();
            while (true)
            {
                if (webRequest.isDone)
                {

                    return webRequest.downloadHandler.text;
                }
            }

        }


        //解析文件
        private static void BuildMap(string fileContent)
        {
            configMap = new Dictionary<string, string>();
            //程序退出using代码块 自动调用dispose()
            using (StringReader reader = new StringReader(fileContent))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    string[] KV = line.Split('=');
                    configMap.Add(KV[0], KV[1]);
                    line = reader.ReadLine();
                }
            }
            //以下是我写的解析代码
            /*            string[] dataArray = fileContent.Split("\r\n".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < dataArray.Length; i++)
                        {
                            string[] name_path = dataArray[i].Split('=');
                            configMap.Add(name_path[0], name_path[1]);
                        }*/
        }

        /// <summary>
        /// 输入预制件的名字，返回预制件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public static T Load<T>(string prefabName) where T : Object
        {
            string prefabPath = configMap[prefabName];
            return Resources.Load<T>(prefabName);
        }
    }
}

