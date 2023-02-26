using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Raffle
{
    public class SaveSystem
    {
        public static void SaveData(string fileName, object data)
        {
            XmlSerializer serializer = new XmlSerializer(data.GetType());
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            EnsureDirectoryExists(filePath);

            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Create);

                serializer.Serialize(fileStream, data);

                fileStream.Close();
            }
            catch (Exception ex)
            {
                // Handle the exception
                Debug.LogError($"Error saving data to {fileName}: {ex.Message}");
            }
        }

        public static T LoadData<T>(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            EnsureDirectoryExists(filePath);

            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open);

                T data = (T)serializer.Deserialize(fileStream);

                fileStream.Close();

                return data;
            }
            catch (FileNotFoundException)
            {
                Debug.LogWarning($"File {fileName} not found, returning default value for type {typeof(T)}");

                return default(T);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading data from {fileName}: {ex.Message}");

                return default(T);
            }
        }

        private static void EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Debug.LogWarning($"Unknown Node: {e.Name}\t{e.Text}");
        }

        private static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Debug.LogWarning($"Unknown attribute {attr.Name}='{attr.Value}'");
        }
    }
}
