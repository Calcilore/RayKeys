using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MonoGame.Framework.Utilities.Deflate;

namespace RayKeys.Misc {
    public static class Logger {
        public static LogLevel LoggingLevel;
        private static bool hasLoaded = false; // prevent saving to file before file exists
        private static FileStream logFile;
        private static StreamWriter streamWriter;
        private static Task writeTask = Task.CompletedTask;
        private static string typeText = "";
        
        public static void Log(object log, LogLevel level) {
            if (LoggingLevel < level) return;
            
            string text = log.ToString();
            
            text = $"[{DateTime.Now.ToLongTimeString()}] [{level}]: {text}\n";
            Console.Write(text);
            
            typeText += text;

            if (hasLoaded && writeTask.IsCompleted) {
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
            new Thread(SynchronousInit).Start(); 
            // why async? Because this gets loaded before the window appears
            // so therefore i want this to not delay the window opening bc i want that to be speedy.
            // this barely affects boot times
            // I just wanted to do this because im obsessed with efficiency
        }

        private static void SynchronousInit() {
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

            int i = 1;
            while (File.Exists(logFileName + i + ".log.gz")) {
                i++;
            }

            logFileName += i + ".log";
            
            logFile = File.OpenWrite("Logs/latest.log");
            streamWriter = new StreamWriter(logFile);
            streamWriter.AutoFlush = true;

            hasLoaded = true;
            
            // make first log Logging to: {LogFileName}
            string tt = typeText;
            typeText = "";
            Logger.Info($"Logging to: {logFileName}");
            typeText = tt;
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