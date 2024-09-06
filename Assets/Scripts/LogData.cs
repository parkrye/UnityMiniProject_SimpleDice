using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

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

    public static void SaveData(LogData logData)
    {
        var jsonData = JsonUtility.ToJson(logData);
        FileStream fileStream = new FileStream(Application.persistentDataPath + fileName, FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public static LogData LoadData()
    {
        var dataPath = Application.persistentDataPath + fileName;
        try
        {
            var jsonData = File.ReadAllText(dataPath);
            return JsonUtility.FromJson<LogData>(jsonData);
        }
        catch
        {
            var logData = new LogData();
            logData.diceData = new List<DiceDatum>();
            return logData;
        }
    }
}