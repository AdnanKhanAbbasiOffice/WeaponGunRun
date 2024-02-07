using System;
using System.IO;
using Falcon.FalconCore.Scripts.Utils.Logs;
using Newtonsoft.Json;
using UnityEngine;
using Object = System.Object;

namespace Falcon.FalconCore.Scripts.Utils.Data
{
    /// <summary>
    ///     Saves, loads and deletes all data in the game
    /// </summary>
    
    public class FFile
    {
        private static string _persistentDataPath;

        public static string PersistentDataPath
        {
            get
            {
                if (_persistentDataPath == null)
                {
                    OnStart();
                }
        
                return _persistentDataPath;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnStart()
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            _persistentDataPath = Application.streamingAssetsPath ?? "";
#else
            _persistentDataPath = Application.persistentDataPath ?? "";
#endif
        }

        
        private readonly string fileName;

        public FFile(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        ///     Save data to a file (overwrite completely)
        /// </summary>
        public void Save(object data)
        {
            Save(JsonConvert.SerializeObject(data));
        }

        public void Save(String data)
        {
            try
            {
                using (var writer = File.CreateText(GetFilePath()))
                {
                    writer.Write(data);
                }
            }
            catch (Exception e)
            {
                // write out error here
                CoreLogger.Instance.Error("Failed to save data to: " + GetFilePath());
                CoreLogger.Instance.Error(e);
            }
        }
        
        public void Append(Object data)
        {
            Append(JsonConvert.SerializeObject(data));
        }

        public void Append(String data)
        {
            try
            {
                using (var writer = File.AppendText(GetFilePath()))
                {
                    writer.Write(data);
                }
            }
            catch (Exception e)
            {
                // write out error here
                CoreLogger.Instance.Error("Failed to save data to: " + GetFilePath());
                CoreLogger.Instance.Error(e);
            }
        }

        /// <summary>
        ///     Load all data at a specified file and folder location
        /// </summary>
        /// <returns></returns>
        public T Load<T>()
        {
            try
            {
                var jsonData = File.ReadAllText(GetFilePath());

                // convert to the specified object type
                var returnedData = JsonConvert.DeserializeObject<T>(jsonData);

                // return the casted json object to use
                return returnedData;
            }
            catch (Exception e)
            {
                CoreLogger.Instance.Warning("Failed to load file from: " + GetFilePath() + ". If this is the first time you run game with SDK, ignore this exception. If this is not the first time, something is definitely wrong (._.|||)");
                CoreLogger.Instance.Warning(e);
                return default(T);
            }
        }
        
        /// <summary>
        ///     Load all data at a specified file and folder location
        /// </summary>
        /// <returns></returns>
        public String Load()
        {
            try
            {
                return File.ReadAllText(GetFilePath());
            }
            catch (Exception e)
            {
                CoreLogger.Instance.Warning("Failed to load file from: " + GetFilePath());
                CoreLogger.Instance.Warning(e);
                return null;
            }
        }

        /// <summary>
        ///     Create file path for where a file is stored on the specific platform given a folder name and file name
        /// </summary>
        /// <returns></returns>
        private string GetFilePath()
        {
            String result;
            if (fileName.EndsWith(".txt")) result = Path.Combine(PersistentDataPath, "data", fileName);
            else result = Path.Combine(PersistentDataPath, "data", fileName + ".txt");
            string directory = Path.GetDirectoryName(result);
            if (!Directory.Exists(directory) && directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            return result;
        }
        
        public void Delete()
        {
            File.Delete(GetFilePath());
        }
    }
}