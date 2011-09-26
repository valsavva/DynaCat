using System;
using Microsoft.Xna.Framework;
using System.IO;

namespace Lunohod
{
	abstract public class ScreenEngine
	{
		protected GameEngine gameEngine;
		protected string name;
		
		public ScreenEngine(GameEngine gameEngine, string name)
		{
			this.gameEngine = gameEngine;
			this.name = name;
		}
		
		public abstract string PrivateContentFolder { get;}
		
		public virtual void Initialize()
		{
		}
		
		public virtual void Update(GameTime gameTime)
		{
		}
		
		public virtual void Draw(GameTime gameTime)
		{
		}

		public virtual void Unload()
		{
		}
	}
}

