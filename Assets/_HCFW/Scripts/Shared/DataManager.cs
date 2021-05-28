using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Linq;

/*
 * Data Manager Class, used to handle JSON I/O for application 
 * Nothing to change here, please.
 * Developed by Romano Zagorscak, romano@zagorscak.com
 */

namespace HCFW
{
    public static class DataManager
    {
        static string saveKey = "GameData";
        static bool usePrefs = true;

        #region Public Methods

        // I haven't actually utilized these as it's just one profile class type, not many
        // So, in an essence - PlayerGameProfileData works just fine for these small games, regardless of it having awkward dependency on Hc_GameManager
        public static void SaveData<T>(T data, string baseDir, string fileName = "Dummy_Data.json")
        {
            //SaveDataToDisk(data, baseDir, fileName);
        }

        public static T LoadData<T>(string baseDir, string fileName = "Dummy_Data.json")
        {
            //return LoadDataFromDisk<T>(baseDir, fileName);
            return default;
        }

        #endregion

        public static string GetFullPath(string dir)
        {
            string path = "";
            path = Path.Combine(Application.persistentDataPath, dir);
            return path;
        }

        public static UnityEngine.Object ReadJsonToObject(string fileName)
        {
            string fullPath = Path.Combine(GetFullPath(saveKey), fileName);
            if (File.Exists(fullPath))
            {
                Debug.Log("Reading from file: " + fullPath);
                StreamReader sr = new StreamReader(fullPath);
                string contents = sr.ReadToEnd();
                sr.Close();
                UnityEngine.Object myObject = JsonUtility.FromJson<UnityEngine.Object>(contents);
                return myObject;
            }
            else
            {
                return null;
            }

        }

        public static string ReadJsonToString(string directory, string fileName)
        {
            string fullPath = Path.Combine(GetFullPath(directory), fileName);
            if (File.Exists(fullPath))
            {
                Debug.Log("Reading from file: " + fullPath);
                StreamReader sr = new StreamReader(fullPath);
                string contents = sr.ReadToEnd();
                sr.Close();
                return contents;
            }
            else
            {
                return null;
            }
        }

        public static void WriteJson(string directory, string fileName, string jSON)
        {
            StreamWriter file;
            if (!Directory.Exists(GetFullPath(saveKey)))
            {
                Directory.CreateDirectory(GetFullPath(saveKey));
                Debug.Log("Directory.CreateDirectory: " + GetFullPath(saveKey));
            }
            string fullPath = Path.Combine(GetFullPath(directory), fileName);

            try
            {
                if (!File.Exists(fullPath))
                {
                    file = File.CreateText(fullPath);
                    file.WriteLine(jSON);
                    file.Close();
#if UNITY_EDITOR
                    Debug.Log("Writing file to: " + fullPath);
#endif

                }
                else
                {
                    File.Delete(fullPath);
                    file = File.CreateText(fullPath);
                    file.WriteLine(jSON);
                    file.Close();
#if UNITY_EDITOR
                    Debug.Log("Writing file to: " + fullPath);
#endif

                }

            }
            catch (Exception e)
            {
                Debug.LogError(e);

            }


        }

        public static void DeleteProcessedDebrisFile(string fileName)
        {

            string fullPath = Path.Combine(GetFullPath(saveKey), fileName);
            if (fullPath != null)
            {
                File.Delete(fullPath);
            }
        }

        public static void DeleteFile(string directory, string fileName)
        {

            string fullPath = Path.Combine(GetFullPath(directory), fileName);
            if (fullPath != null)
            {
                File.Delete(fullPath);
            }
        }

        public static void DeleteAllFilesInDir(string directory)
        {
            DirectoryInfo info = new DirectoryInfo(GetFullPath(directory));
            foreach (FileInfo fi in info.GetFiles())
            {
                File.Delete(fi.FullName);
            }
        }

        public static FileInfo[] GetAllFilesContent(string directory)
        {

            if (!Directory.Exists(GetFullPath(saveKey)))
            {
                Directory.CreateDirectory(GetFullPath(saveKey));
                Debug.Log("Directory.CreateDirectory: " + GetFullPath(saveKey));
            }
            DirectoryInfo info = new DirectoryInfo(GetFullPath(directory));
            FileInfo[] fileInfo = info.GetFiles().OrderBy(prop => prop.CreationTime).ToArray();
            return fileInfo;

        }

        public static List<string> GetJsonStringsFromAllFiles(string directory)
        {

            List<string> jsons = new List<string>();
            foreach (FileInfo fi in GetAllFilesContent(directory))
            {
                if (fi.Extension.Contains("json"))
                {
                    jsons.Add(ReadJsonToString(directory, fi.Name));
                }
            }
            return jsons;

        }

        /* HEX/Byte converter methods in case of need, not used now */
        public static string ByteArrayToHex(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        // Basically, whole JSON object is written to disk with PlayerGameProfile object that is activeObject in GameManager of any game
        // Also, when GET is called, last PlayerGameProfile data from disk is loaded into GameManager player object
        // It uses PlayerGameProfile class which can be expanded easily if need be but for basic versions of games it will most likely not be necessary
        public static PlayerGameProfile PlayerGameProfileData
        {
            get
            {
                if (!usePrefs)
                {
                    return JsonUtility.FromJson<PlayerGameProfile>(ReadJsonToString(saveKey, "_playerGameProfile.json"));
                } else
                {
                    Debug.Log("Reading from pref: " + saveKey);
                    return JsonUtility.FromJson<PlayerGameProfile>(PlayerPrefs.GetString(saveKey));
                }
                
            }
            set
            {
                if (!usePrefs)
                {
                    WriteJson(saveKey, "_playerGameProfile.json", JsonUtility.ToJson(value));
                } else
                {
                    Debug.Log("Saving to pref: " + saveKey);
                    PlayerPrefs.SetString(saveKey, JsonUtility.ToJson(value));
                }
            }
        }
    }
}

