using System;

namespace RayKeys {
    public static class Program {
        [STAThread]
        static void Main() {
            using (var game = new Game1())
                game.Run();
            
            // Stop all threads
            Environment.Exit(0);
        }
    }
}