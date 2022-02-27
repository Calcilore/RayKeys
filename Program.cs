using System;
using RayKeys.Misc;

namespace RayKeys {
    public static class Program {
        [STAThread]
        static void Main(string[] args) {
            LogLevel logLevel = LogLevel.Info;
            
            foreach (string arg in args) {
                if (arg == "--help") {
                    Console.WriteLine("Usage:\n  RayKeys [OPTION]\n\n" +
                                      "RayKeys\n\n" +
                                      "Options:\n" +
                                      "  --help                                        Show Help\n" +
                                      "  --loglevel=level                              Change Log Level\n"
                    );
                    return;
                }

                if (arg.StartsWith("--loglevel")) {
                    switch (arg[(arg.IndexOf('=')+1)..]) {
                        case "error":
                            logLevel = LogLevel.Error;
                            break;
                        
                        case "info":
                            logLevel = LogLevel.Info;
                            break;
                            
                        case "debug":
                            logLevel = LogLevel.Debug;
                            break;
                        
                        default:
                            Console.WriteLine("Invalid Log Level, Options are: error, info and debug\n");
                            return;
                    }
                }
            }

            using (var game = new Game1(logLevel)) {
                game.Run();
            }

            // Stop all threads
            Environment.Exit(0);
        }
    }
}