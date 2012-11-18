using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AudioToolbox;
using System.IO;
using MonoTouch.AVFoundation;

namespace DynaCatFree
{
	[Register("AppDelegate")]
	public class Program : Lunohod.Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		public static void Main(string[] args)
		{
			Lunohod.GameEngine.IsFreeVersion = true;
			Lunohod.Program.Main(args);
		}
	}    
}
