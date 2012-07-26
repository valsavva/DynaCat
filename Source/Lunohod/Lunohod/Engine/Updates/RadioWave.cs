using System;
using Lunohod.Objects;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Lunohod
{
	public class RadioWave : XObject
	{
		private TimeSpan time;
		
		private double textureDiameter;
		private Texture2D texture;
		private System.Drawing.RectangleF bounds;

		public RadioWave()
		{
		}
		
		public double Diameter;
		public double Radius;
		public bool IsEnclosingHero;
		public bool IsHeroActive;
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			time += p.GameTime.ElapsedGameTime;
			
			Radius = p.LevelEngine.Tower.SignalSpeed * time.TotalSeconds;
			Diameter = Radius * 2.0;

			if (Radius > GameEngine.MaxWaveTravelDistance)
				return;

            if (Diameter <= GameEngine.MinWaveTextureDiameter)
                textureDiameter = GameEngine.MinWaveTextureDiameter;
            else if (Diameter >= GameEngine.MaxWaveTextureDiameter)
                textureDiameter = GameEngine.MaxWaveTextureDiameter;
			else {
                textureDiameter = Diameter - Diameter % GameEngine.WaveTextureDiameterStep;
			}

            texture = p.Game.WaveTextures[(int)textureDiameter / GameEngine.WaveTextureDiameterStep - 1];
			bounds.X = (p.LevelEngine.Tower.Bounds.X + p.LevelEngine.Tower.Bounds.Width / 2 - (int)(Diameter / 2));
			bounds.Y = (p.LevelEngine.Tower.Bounds.Y + p.LevelEngine.Tower.Bounds.Height / 2 - (int)(Diameter / 2));
			
			bounds.Width = bounds.Height = (int)(Diameter);
		}
		
		public override void Draw(DrawParameters p)
		{
			base.Draw(p);

            if (Diameter > GameEngine.MaxWaveTextureDiameter * 2)
				return;

			if (this.IsHeroActive)
				p.SpriteBatch.Draw(texture, bounds, Color.White);
			else
				p.SpriteBatch.Draw(texture, bounds, Color.Gray);
		}
	}
}

