using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Android;

[Serializable]
public class LogData
{
    public List<DiceDatum> diceData;
}

[Serializable]
public class DiceDatum
{
    public int index;
    public string title;
    public int range;
    public int result;
}

public static class DataHelper
{
    private const string fileName = "/logfile.json";

    public static bool TrySaveData(LogData logData)
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
#endif

        try
        {
            var dataPath = Application.persistentDataPath + fileName;
            var jsonData = JsonUtility.ToJson(logData);

#if UNITY_EDITOR
            // Debug log to verify the path and data
            Debug.Log($"Saving data to: {dataPath}");
            Debug.Log($"JSON Data: {jsonData}");
#endif

            File.WriteAllText(dataPath, jsonData);
            return true;
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError($"Failed to save data: {e.Message}");
#endif
        }
        return false;
    }

    public static bool TryLoadData(out LogData LogData)
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
#endif

        try
        {
            var dataPath = Application.persistentDataPath + fileName;

            // Debug log to verify the path
#if UNITY_EDITOR
            Debug.Log($"Loading data from: {dataPath}");
#endif

            if (File.Exists(dataPath))
            {
                using (var fileStream = new FileStream(dataPath, FileMode.Open, FileAccess.Read))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    var parsingText = streamReader.ReadToEnd();
#if UNITY_EDITOR
                    Debug.Log($"Loaded JSON Data: {parsingText}");
#endif
                    LogData = JsonUtility.FromJson<LogData>(parsingText);
                    return true;
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("File does not exist. Creating new log data.");
#endif
                LogData = new LogData { diceData = new List<DiceDatum>() };
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError($"Failed to load data: {e.Message}");
#endif
            LogData = new LogData { diceData = new List<DiceDatum>() };
        }

        return false;
    }
}