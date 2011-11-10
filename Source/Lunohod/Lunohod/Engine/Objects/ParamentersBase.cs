using System;

namespace Lunohod.Objects
{
	public abstract class ParamentersBase
	{
		private ScreenEngine screenEngine;
		
		public GameEngine Game;
		public ScreenEngine ScreenEngine 
		{
			get { return screenEngine; }
			set
			{
				screenEngine = value;
				this.LevelEngine = value as LevelEngine;
			}
		}
		public LevelEngine LevelEngine { get; private set; }
		public XResourceBundle Resources;
	}
}
