using System;
using Lunohod.Objects;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Lunohod
{
	public class RadioWave : XObject
	{
		private TimeSpan startTime;
		
		private double diameter;
		private double textureDiameter;
		private Texture2D texture;
		private System.Drawing.RectangleF bounds;

		public RadioWave(TimeSpan startTime)
		{
			this.startTime = startTime;
		}

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);

			diameter = p.LevelEngine.tower.SignalSpeed * (p.GameTime.TotalGameTime - this.startTime).TotalSeconds * 2.0;

			if (diameter > GameEngine.MaxWaveRadius * 2)
				return;

            if (diameter <= GameEngine.MinWaveRadius)
                textureDiameter = GameEngine.MinWaveRadius;
            else if (diameter >= GameEngine.MaxWaveRadius)
                textureDiameter = GameEngine.MaxWaveRadius;
			else {
                textureDiameter = diameter - diameter % GameEngine.WaveRadiusStep;
			}

            texture = p.Game.WaveTextures[(int)textureDiameter / GameEngine.WaveRadiusStep - 1];
			bounds.X = (p.LevelEngine.tower.Bounds.X + p.LevelEngine.tower.Bounds.Width / 2 - (int)(diameter / 2));
			bounds.Y = (p.LevelEngine.tower.Bounds.Y + p.LevelEngine.tower.Bounds.Height / 2 - (int)(diameter / 2));
			
			bounds.Width = bounds.Height = (int)(diameter);
		}
		
		public override void Draw(DrawParameters p)
		{
			base.Draw(p);

            if (diameter > GameEngine.MaxWaveRadius * 2)
				return;
			
			p.SpriteBatch.Draw(texture, bounds, Color.White);
		}
	}
}

