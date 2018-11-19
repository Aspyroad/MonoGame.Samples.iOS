using System;
using NeonShooter;


namespace NoenShooter.OpenGL
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var game = new NeonShooterGame();
            game.Run();
        }
    }
}
