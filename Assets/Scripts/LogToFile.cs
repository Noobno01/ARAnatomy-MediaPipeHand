using System;
using System.IO;
using UnityEngine;

public class LogToFile : MonoBehaviour
{
    private string logFilePath;

    void Awake()
    {
        // Set log file path
        logFilePath = Path.Combine(Application.persistentDataPath, "unity_log.txt");

        // Subscribe to log messages
        Application.logMessageReceived += HandleLog;

        Debug.Log("Logging to: " + logFilePath);
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"{DateTime.Now:HH:mm:ss} [{type}] {logString}";
        if (type == LogType.Exception || type == LogType.Error)
            logEntry += $"\n{stackTrace}";

        // Append to file
        try
        {
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write log to file: " + e.Message);
        }
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
