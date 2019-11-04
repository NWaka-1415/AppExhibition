using System;
using System.IO;
using UnityEngine;

namespace Controllers
{
    public static class DataController
    {
        private const string FileName = "appEx.json";

        private const string FileDirectory = "data/";

        private const string UnityEditorDirectory = "Assets/StreamingAssets/";

        /// <summary>
        /// 与えられたデータをjsonに変換します
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>jsonデータ:string</returns>
        public static string ChangeJsonFromData<T>(T data)
        {
            if (data == null) return null;
            return JsonUtility.ToJson(data);
        }

        /// <summary>
        /// jsonデータをT型(任意のオブジェクト)オブジェクトに変換します
        /// </summary>
        /// <param name="jsonData"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>データの内容:T型(任意のオブジェクト)</returns>
        public static T ChangeDataFromJson<T>(string jsonData)
        {
            T data;
            try
            {
                data = JsonUtility.FromJson<T>(jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }

            return data;
        }

        /// <summary>
        /// json形式のデータを書き込みます。
        /// </summary>
        /// <param name="jsonData"></param>
        public static void SaveJson(string jsonData)
        {
            if (!Directory.Exists(FileDirectory)) Directory.CreateDirectory(FileDirectory);
            string saveDataPath = Path.Combine(FileDirectory, FileName);
#if UNITY_EDITOR
            saveDataPath = Path.Combine(UnityEditorDirectory, saveDataPath);
#endif
            //append引数はtrue追記，false上書き
            StreamWriter writer = new StreamWriter(saveDataPath, false);
            writer.Write(jsonData);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// json形式のデータを読み込みます。
        /// </summary>
        /// <returns>if(Exists data) then json形式のデータ:string else then null</returns>
        public static string LoadJson()
        {
            string saveDataPath = Path.Combine(FileDirectory, FileName);
#if UNITY_EDITOR
            saveDataPath = Path.Combine(UnityEditorDirectory, saveDataPath);
#endif
            if (File.Exists(saveDataPath))
            {
                FileStream fileStream = File.Open(saveDataPath, FileMode.Open);
                StreamReader reader = new StreamReader(fileStream);
                string jsonData = reader.ReadToEnd();
                reader.Close();
                fileStream.Close();
                return jsonData;
            }

            return null;
        }
    }
}