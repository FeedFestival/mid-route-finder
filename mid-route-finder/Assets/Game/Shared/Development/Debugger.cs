using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Game.Shared.Development {

public enum StoppedBy {
    Cancel,
    Clear,
    Done,
}

public static class Debugger {
    static readonly Dictionary<StoppedBy, string> _stoppedBy = new() {
        { StoppedBy.Cancel, "- ✕" },
        { StoppedBy.Clear, "--©" },
        { StoppedBy.Done, "✓" },
    };

    static readonly List<byte> LogBuffer = new List<byte>();

    static readonly object LockObj = new object();

    const int MaxLogSize = 512 * 1024; // 512 KB

    // const int MaxLogSize = 8 * 1024; // 8 KB
    static readonly string LogFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");

    static Debugger() {
        Application.logMessageReceived += HandleUnityLog;
    }

    static void HandleUnityLog(string logString, string stackTrace, LogType type) {
        string logEntry = $"{DateTime.UtcNow:HH:mm:ss} [{type}] {logString}\n";
        if (type == LogType.Error || type == LogType.Exception) {
            logEntry += $"StackTrace:\n{stackTrace}\n";
        }

        AddToBuffer(logEntry);
    }

    static void AddToBuffer(string message) {
        lock (LockObj) {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            LogBuffer.AddRange(messageBytes);
            if (LogBuffer.Count >= MaxLogSize) {
                _ = FlushLogs(); // Fire and forget
            }
        }
    }

    static async Task FlushLogs() {
        List<byte> bytesToWrite;
        lock (LockObj) {
            if (LogBuffer.Count == 0) return;
            bytesToWrite = new List<byte>(LogBuffer);
            LogBuffer.Clear();
        }

        try {
            using (FileStream fs = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read, 4096,
                       true)) {
                await fs.WriteAsync(bytesToWrite.ToArray(), 0, bytesToWrite.Count);
            }
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to write logs: {ex.Message}");
        }
    }

    /// <summary>
    /// Class to log only in Unity editor, double clicking console logs produced by this class still open the calling source file)
    /// NOTE: Implement your own version of this is supported. Just implement a class named "DebugEditor" and any method inside this class starting with "Log" will, when double clicked, open the file of the calling method. Use [Conditional] attributes to control when any of these methods should be included.
    /// </summary>
    public static void Log(object message, Object context = null) {
#if UNITY_EDITOR
        Debug.Log(message, context);
#endif
        AddToBuffer(message + "\n");
    }

    public static void LogWarning(object message, Object context = null) {
#if UNITY_EDITOR
        Debug.LogWarning(message, context);
#endif
        AddToBuffer("[WARNING] " + message + "\n");
    }

    public static void LogError(object message, Object context = null) {
#if UNITY_EDITOR
        Debug.LogError(message, context);
#endif
        AddToBuffer("[ERROR] " + message + "\n");
    }

    public static void LogCmd(int step, string message) {
        string cmdMessage = $"<b>{step}.</b> {message} <b>↙</b>";
        Debugger.Log(cmdMessage);
    }

    public static void LogCmd(int step, StoppedBy stoppedBy, string message) {
        string cmdMessage

            // = $" -{(stoppedBy == StoppedBy.Clear ? "-" : "")} <b>{_stoppedBy[stoppedBy]}</b> -> <b>{step}.</b> - {message}";
            = $" <b>{_stoppedBy[stoppedBy]}</b> -> <b>{step}.</b> - {message}";
        Debugger.Log(cmdMessage);
    }
}

}
