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
	
    [Register("AppDelegate")]
    class Program : UIApplicationDelegate
    {
		GameEngine game;
		
        public override void FinishedLaunching(UIApplication app)
        {
			// Fun begins..
            this.game = new GameEngine();
            this.game.Run();
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
			game.InBackground = true;

			if (game.ScreenEngine is LevelEngine)
				game.EnqueueEvent(new GameEvent(GameEventType.Pause, game.CurrentUpdateTime, true));
		}

		private void GotoForegroundMode()
		{
			game.InBackground = false;
		}
	
	}    
}
