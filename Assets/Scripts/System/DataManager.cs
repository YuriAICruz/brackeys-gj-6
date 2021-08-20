using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace System
{
    public static class DataManager
    {
        private static string _path;
        private static readonly string _extension = ".dat";

        private static void ConstructPath()
        {
#if UNITY_EDITOR
            _path = Application.dataPath + "/../local_data";
#else
            _path = Application.persistentDataPath + "/local_data";
#endif
        }

        public static void Save<T>(T data, string fileName)
        {
            ConstructPath();

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            File.WriteAllText(_path + "/" + fileName + _extension, JsonConvert.SerializeObject(data));
        }

        public static T Load<T>(string fileName)
        {
            ConstructPath();

            var path = _path + "/" + fileName + _extension;
            if (!File.Exists(path))
                return default;

            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return default;
            }
        }
    }
}