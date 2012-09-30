using System;
using System.Collections.Generic;
using System.Linq;
using Lunohod.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Storage;
using System.Threading;

namespace Lunohod
{
	public partial class GameEngine : Game
	{
		private object crossThreadSync = new object();

		private Thread mainThread = System.Threading.Thread.CurrentThread;

		private AutoResetEvent executedEvent = new AutoResetEvent(false);

		public event Action ThreadCallbacks;

		private void DoEvents()
		{
			lock(crossThreadSync)
			{
				if (this.ThreadCallbacks != null)
					this.ThreadCallbacks();
				this.ThreadCallbacks = null;

				executedEvent.Set();
			}
		}

		public void InvokeOnMainThread(Action action)
		{
			if (System.Threading.Thread.CurrentThread == mainThread)
			{
				action();
				return;
			}

			lock(crossThreadSync)
			{
				this.ThreadCallbacks += action;
				executedEvent.Reset();
			}

			executedEvent.WaitOne();
		}
	}
}

