using System.IO;
using Directory = UnityEngine.Windows.Directory;

namespace Controllers
{
    public static class DataController
    {
        private const string FileName = ".json";

        private const string FileDirectory = "Assets/StreamingAssets/data/";

        /// <summary>
        /// json形式のデータを書き込みます。
        /// </summary>
        /// <param name="jsonData"></param>
        public static void SaveJson(string jsonData)
        {
            if (!Directory.Exists(FileDirectory)) Directory.CreateDirectory(FileDirectory);
            string saveDataPath = Path.Combine(FileDirectory, FileName);
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
            if (File.Exists(saveDataPath))
            {
                FileStream fileStream = File.Open(saveDataPath, FileMode.Open);
                StreamReader reader=new StreamReader(fileStream);
                string jsonData = reader.ReadToEnd();
                reader.Close();
                fileStream.Close();
                return jsonData;
            }

            return null;
        }
    }
}