#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

using AppKit;
using Foundation;

using NeonShooter;
#endregion

namespace NeonShooterMac.MacOS
{
    class Program
    {
        static void Main(string[] args)
        {
            NSApplication.Init();

            using (var p = new NSAutoreleasePool())
            {
                NSApplication.SharedApplication.Delegate = new AppDelegate();

                // Set our Application Icon
                NSImage appIcon = NSImage.ImageNamed("monogameicon.png");
                NSApplication.SharedApplication.ApplicationIconImage = appIcon;

                NSApplication.Main(args);
            }
        }
    }
    [Register("AppDelegate")]
    class AppDelegate : NSApplicationDelegate
    {
        private NeonShooterGame game;

        public override void DidFinishLaunching(NSNotification notification)
        {
            game = new NeonShooterGame();
            game.Run();
        }

        //public override  (Foundation.NSObject notification)
        //{

        //}

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }
    }
}
