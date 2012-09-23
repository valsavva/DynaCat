using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AudioToolbox;
using Microsoft.Xna.Framework;
using System.IO;

namespace Lunohod
{
	public class LaunchController : UIViewController
	{
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			Program.game = new GameEngine();
			Program.game.Run();

			Program.startWindow.RemoveFromSuperview();
			Program.startWindow = null;
		}
	}

    [Register("AppDelegate")]
    public class Program : UIApplicationDelegate
    {
		internal static GameEngine game;

		internal static UIWindow startWindow;

		private static DateTime splashStart;
		private static TimeSpan splashDuration = TimeSpan.FromSeconds(2.5);

		public override void FinishedLaunching(UIApplication app)
        {
			var image = new UIImage(@"splash.png");
			UIImageView view = new UIImageView(image);

			startWindow = new UIWindow(UIScreen.MainScreen.Bounds);
			startWindow.RootViewController = new LaunchController();
			startWindow.Add(view);
			startWindow.MakeKeyAndVisible();

			splashStart = DateTime.UtcNow;
        }

		public static void FinishShowingSplash()
		{
			while((DateTime.UtcNow - splashStart) < splashDuration)
				System.Threading.Thread.Sleep(200);
		}

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleAppDomainCurrentDomainUnhandledException; ;

			UIApplication.Main(args, null, "AppDelegate");
        }

		static void HandleAppDomainCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogError((Exception)e.ExceptionObject);
        }

        static void LogError(Exception ex)
        {
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "lunohod_error.txt"), ex.ToString());
            Console.WriteLine("**** Unhandled error!\n" + ex.ToString());
        }

		public override void DidEnterBackground(UIApplication application)
		{
			GotoBackgroundMode();
		}

		public override void OnResignActivation(UIApplication application)
		{
			GotoBackgroundMode();
		}

		public override void OnActivated(UIApplication application)
		{
			GotoForegroundMode();
		}

		public override void WillEnterForeground(UIApplication application)
		{
			GotoForegroundMode();
		}

		private void GotoBackgroundMode()
		{
			if (Program.game == null)
				return;

			game.InBackground = true;

			if (game.ScreenEngine is LevelEngine)
				game.EnqueueEvent(new GameEvent(GameEventType.Pause, game.CurrentUpdateTime, true));
		}

		private void GotoForegroundMode()
		{
			if (Program.game == null)
				return;

			game.InBackground = false;
		}
	
	}    
}
