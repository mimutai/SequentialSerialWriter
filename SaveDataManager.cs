using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SequentialSerialWriter
{
    internal class SaveDataManager
    {
        internal static string SAVEDATA_PATH = Environment.CurrentDirectory + @"\mysavedata.xml";
        internal static SaveDataXml SaveData { get; set; } = new SaveDataXml();

        internal static SaveDataXml Load()
        {
            if (!File.Exists(SAVEDATA_PATH)) return SaveData; //ファイルが存在しない場合は初期値を返す

            XmlSerializer serializer = new XmlSerializer(typeof(SaveDataXml));
            StreamReader file = new StreamReader(SAVEDATA_PATH);
            try
            {
                var data = serializer.Deserialize(file); //ファイルから読み込む
                if (data != null) SaveData = (SaveDataXml)data; //Xmlの要素が存在したらキャストして保存
            }
            catch (Exception)
            {
            }
            finally
            {
                if(file != null) file.Close();
            }

            return SaveData;
        }

        internal static void Save()
        {
            Debug.WriteLine(SAVEDATA_PATH);

            //ファイルが存在しなかった場合は作成する
            if (!File.Exists(SAVEDATA_PATH))
            {
                FileStream fs = File.Create(SAVEDATA_PATH);
                fs.Close();
            }

            // シリアライズしてファイルに書き込む
            XmlSerializer serializer = new XmlSerializer(typeof(SaveDataXml));
            using (StreamWriter sw = new StreamWriter(SAVEDATA_PATH, false, new UTF8Encoding()))
            {
                serializer.Serialize(sw, SaveData); //書き込む
            }
        }
    }

    [XmlRoot("savedata")]
    public class SaveDataXml
    {
        [XmlArray("sendtextlist")]
        public List<string> SendTextList = new List<string>();
    }
}
