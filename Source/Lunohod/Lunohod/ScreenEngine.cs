using System;
using Microsoft.Xna.Framework;
using System.IO;
using Lunohod.Objects;
using System.Collections.Generic;

namespace Lunohod
{
	abstract public class ScreenEngine
	{
		protected GameEngine game;
		protected string name;

		public List<XTapArea> tapAreas;
		
		public ScreenEngine(GameEngine game, string name)
		{
			this.game = game;
			this.name = name;
		}
		
		public abstract XObject RootComponent { get; }
		
		public virtual void ProcessEvent(GameEvent e)
		{
			
		}
		
		public virtual void Initialize()
		{
			this.tapAreas = new List<XTapArea>();
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

