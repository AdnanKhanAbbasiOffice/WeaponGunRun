using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Falcon.FalconCore.Scripts.Entities;
using Falcon.FalconCore.Scripts.Interfaces;
using Falcon.FalconCore.Scripts.Utils.Logs;
using Falcon.FalconCore.Scripts.Utils.Singletons;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconCore.Scripts.Utils.Data
{
    public class FData : FSingleton<FData>, IFEssential
    {
        public static readonly string DataFile = Path.Combine("Sdk", "Data");

        private readonly FConcurrentDict<string, string> cache;

        private readonly FFile file = new FFile(DataFile);

        [Preserve]
        public FData()
        {
            var fileData = file.Load<Dictionary<string, string>>() ?? new Dictionary<string, string>();

            fileData = fileData
                .Where(f => f.Value != null)
                .ToDictionary(x => x.Key, x => x.Value);
            cache = new FConcurrentDict<string, string>(fileData);
        }

        public void OnPreContinue()
        {
        }

        public void OnPostStop()
        {
            Instance.SaveData();
        }

        public T GetOrDefault<T>(string key, T defaultValue)
        {
            string valStr;
            if (cache.TryGetValue(key, out valStr))
                try
                {
                    return JsonConvert.DeserializeObject<T>(valStr);
                }
                catch (Exception e)
                {
                    CoreLogger.Instance.Warning(e);
                    return defaultValue;
                }

            return defaultValue;
        }

        public T GetOrSet<T>(string key, T valueIfNotExist)
        {

            var result = valueIfNotExist;
            cache.Compute(key, (hasKey, valStr) =>
            {
                if (!hasKey) return JsonConvert.SerializeObject(valueIfNotExist);
                try
                {
                    result = JsonConvert.DeserializeObject<T>(valStr);
                    return valStr;
                }
                catch (Exception e)
                {
                    CoreLogger.Instance.Warning(e);
                    return JsonConvert.SerializeObject(valueIfNotExist);
                }
            });
            return result;
        }

        public bool HasKey(string key)
        {
            return cache.ContainsKey(key);
        }

        public void ComputeIfPresent<T>(string key, Action<T> ifPresent)
        {

            cache.ComputeIfPresent(key, valStr =>
            {
                try
                {
                    ifPresent.Invoke(JsonConvert.DeserializeObject<T>(valStr));
                }
                catch (Exception e)
                {
                    CoreLogger.Instance.Warning(e);
                    cache.Remove(key);
                }
            });
        }

        public KeyValuePair<bool, T> ComputeIfPresent<T>(string key, Func<T, T> ifPresent)
        {
            var result = default(T);

            return new KeyValuePair<bool, T>(cache.ComputeIfPresent(key, valStr =>
            {
                try
                {
                    result = JsonConvert.DeserializeObject<T>(valStr);
                }
                catch (Exception e)
                {
                    CoreLogger.Instance.Warning(e);
                    return null;
                }

                return JsonConvert.SerializeObject(ifPresent.Invoke(result));
            }).Key, result);
        }

        public void ComputeIfAbsent(string key, Action computation)
        {
            cache.ComputeIfAbsent(key, computation);
        }

        public KeyValuePair<bool, T> ComputeIfAbsent<T>(string key, Func<T> ifAbsent)
        {
            var result = default(T);
            return new KeyValuePair<bool, T>(cache.ComputeIfAbsent(key, () =>
            {
                result = ifAbsent.Invoke();
                return JsonConvert.SerializeObject(result);
            }).Key, result);
        }

        public T Compute<T>(string key, Func<bool, T, T> function)
        {

            var result = default(T);
            cache.Compute(key, (hasVal, valStr) =>
            {
                if (hasVal)
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<T>(valStr);
                    }
                    catch (Exception e)
                    {
                        CoreLogger.Instance.Warning(e);
                        hasVal = false;
                    }
                }

                if (function != null) result = function(hasVal, result);

                if (result != null) return JsonConvert.SerializeObject(result);

                return null;
            });
            return result;
        }

        public void Save<T>(string key, T value)
        {
        var json = JsonConvert.SerializeObject(value);
        cache[key] = json;
        }

        public void Delete(string key)
        {
            cache.Remove(key);
        }

        private void SaveData()
        {
            file.Save(Instance.cache);
            CoreLogger.Instance.Info("Save Data finished");
        }
    }
}