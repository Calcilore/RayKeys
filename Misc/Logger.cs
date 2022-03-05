using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RayKeys.Misc {
    public static class Logger {
        public static LogLevel LoggingLevel;
        private static FileStream logFile;
        private static StreamWriter streamWriter;
        private static Task writeTask;
        private static string typeText;
        
        public static void Log(string log, LogLevel level) {
            if (LoggingLevel < level) return;
            
            log = $"[{DateTime.Now.ToLongTimeString()}] [{level}]: {log}\n";
            Console.Write(log);
            
            typeText += log;

            if (writeTask.IsCompleted) {
                writeTask = streamWriter.WriteAsync(typeText);
                typeText = "";
            }
        }

        public static void WaitFlush() {
            while (!writeTask.IsCompleted) {
                Thread.Sleep(50);
            }

            streamWriter.Write(typeText);
            typeText = "";
        }

        public static void Init(LogLevel logLevel) {
            LoggingLevel = logLevel;
            
            if (!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");
            
            string logFileName = $"Logs/{DateTime.Now:yyyy-MM-dd}-0";

            for (int i = 1; File.Exists(logFileName + ".log"); i++) {
                logFileName = logFileName[..^1] + i;
            }

            logFileName += ".log";
            
            logFile = File.Create(logFileName);
            streamWriter = new StreamWriter(logFile);
            streamWriter.AutoFlush = true;
            writeTask = Task.CompletedTask;
            typeText = "";
            Logger.Info($"Logging to: {logFileName}");
        }
        
        public static void Error(string log) {
            Log(log, LogLevel.Error);
        }
        
        public static void Info(string log) {
            Log(log, LogLevel.Info);
        }
        
        public static void Debug(string log) {
            Log(log, LogLevel.Debug);
        }
    }

    public enum LogLevel {
        Error,
        Info,
        Debug
    }
}