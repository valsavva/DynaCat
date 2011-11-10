using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Lunohod
{
	
    [Register("AppDelegate")]
    class Program : UIApplicationDelegate
    {
        public override void FinishedLaunching(UIApplication app)
        {
            // Fun begins..
            using (GameEngine game = new GameEngine())
            {
                game.Run();
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }
    }    
/*	
    [Register ("AppDelegate")]
    class Application : UIApplicationDelegate 
    {
        public override void FinishedLaunching (UIApplication app)
        {
            using (var game = new GameEngine())
			{
            	game.Run();
			}
        }
		
		public override void OnActivated(UIApplication application)
		{
		}

        static void Main (string [] args)
        {
			AppDomain.CurrentDomain.UnhandledException += HandleAppDomainCurrentDomainUnhandledException;;
			try
			{
	            UIApplication.Main (args,null,"AppDelegate");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
        }

        static void HandleAppDomainCurrentDomainUnhandledException (object sender, UnhandledExceptionEventArgs e)
        {
        	Console.WriteLine(e.ExceptionObject.ToString());
        }
    }
	 */
	
//	[Register ("AppDelegate")]
//	public class Application
//	{
//		static void Main (string[] args)
//		{
//			UIApplication.Main (args, null, "AppDelegate");
//		}
//	}
//	
//	// The name AppDelegate is referenced in the MainWindow.xib file.
//	public partial class AppDelegate : UIApplicationDelegate
//	{
//		private GameEngine game;
//		// This method is invoked when the application has loaded its UI and its ready to run
//		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
//		{
//			// If you have defined a view, add it here:
//			// window.AddSubview (navigationController.View);
//            // Fun begins..
//			//window.MakeKeyAndVisible ();
//	
//			return true;
//		}
//
//		// This method is required in iPhoneOS 3.0
//		public override void OnActivated (UIApplication application)
//		{
//            game = new GameEngine();
//            game.Run();
//		}
//	}
}
