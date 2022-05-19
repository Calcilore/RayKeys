using System;
using System.IO;
using System.Threading.Tasks;
using MonoGame.Framework.Utilities.Deflate;

namespace RayKeys.Misc {
    public static class Logger {
        public static LogLevel LoggingLevel;
        private static FileStream logFile;
        private static StreamWriter streamWriter;
        private static Task writeTask = Task.CompletedTask;
        private static string typeText;
        
        public static void Log(object log, LogLevel level) {
            if (LoggingLevel < level) return;
            
            string text = log.ToString();
            
            text = $"[{DateTime.Now.ToLongTimeString()}] [{level}]: {text}\n";
            Console.Write(text);
            
            typeText += text;

            if (writeTask.IsCompleted) {
                writeTask = streamWriter.WriteAsync(typeText);
                typeText = "";
            }
        }

        public static void WaitFlush() {
            writeTask.Wait();

            streamWriter.Write(typeText);
            typeText = "";
        }

        public static void Init(LogLevel logLevel) {
            LoggingLevel = logLevel;

            if (!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");
            
            if (File.Exists("Logs/latest.log")) {
                using FileStream originalFileStream = File.Open("Logs/latest.log", FileMode.Open);
                string gzFileLoc = new StreamReader(originalFileStream).ReadLine();

                try {
                    gzFileLoc = "Logs" + gzFileLoc[gzFileLoc.LastIndexOf('/')..] + ".gz";
                }
                catch (Exception) {
                    gzFileLoc = "Logs/Unknown-" + ThingTools.Rand.Next() + ".log.gz";
                }

                originalFileStream.Seek(0, SeekOrigin.Begin);

                using FileStream compressedFileStream = File.Create(gzFileLoc);
                using GZipStream compressor = new GZipStream(compressedFileStream, CompressionMode.Compress);
                originalFileStream.CopyTo(compressor);
            }

            string logFileName = $"Logs/{DateTime.Now:yyyy-MM-dd}-";

            int i;
            for (i = 1; File.Exists(logFileName + i + ".log.gz"); i++) { }

            logFileName += i + ".log";
            
            logFile = File.OpenWrite("Logs/latest.log");
            streamWriter = new StreamWriter(logFile);
            streamWriter.AutoFlush = true;
            typeText = "";
            Logger.Info($"Logging to: {logFileName}");
        }
        
        public static void Error(object log) {
            Log(log, LogLevel.Error);
        }
        
        public static void Info(object log) {
            Log(log, LogLevel.Info);
        }
        
        public static void Debug(object log) {
            Log(log, LogLevel.Debug);
        }
    }

    public enum LogLevel {
        Error,
        Info,
        Debug
    }
}