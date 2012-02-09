using System;
using System.Linq;
using System.Collections.Generic;

namespace Lunohod.Objects
{
	public class XSystem : XObject
	{
		private GameEngine game;
		
		public XSystem()
		{
			this.Id = "system";
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.game = p.Game;
		}
		
		public string GetLevelName(string si)
		{
			int i = int.Parse(si);
			
			if (i >= game.GameObject.Levels.Length)
				return "";
			
			var level = game.GameObject.Levels[i];
			
			return level.Name;
		}
		
		public void StartLevel(string si)
		{
			int i = int.Parse(si);
			
			if (i >= game.GameObject.Levels.Length)
				return;

			game.LoadLevel(game.GameObject.Levels[i].File);
		}
	}
}

