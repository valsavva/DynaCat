using System;
using Lunohod.Objects;
using Microsoft.Xna.Framework;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod
{
	public class GameEngine : Game
	{
        private GraphicsDeviceManager graphics;
		
		public ScreenEngine screenEngine;
		
		public Texture2D BlankTexture {get; private set;}
		
		public GameEngine()
		{
            graphics = new GraphicsDeviceManager(this);
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            Content.RootDirectory = "Content";
		}
		
		protected override void Initialize()
		{
			base.Initialize ();
		}
		
		protected override void LoadContent()
		{
			this.BlankTexture = this.Content.Load<Texture2D>("Global/blank.png");
			base.LoadContent ();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update (gameTime);
			
			if (screenEngine == null)
			{
				screenEngine = new LevelEngine(this, "TestLevel");
				screenEngine.Initialize();
			}

			screenEngine.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
            GraphicsDevice.Clear(Color.CornflowerBlue);

			this.screenEngine.Draw(gameTime);
			
			base.Draw(gameTime);
		}
		
		protected override void UnloadContent()
		{
			base.UnloadContent ();
		}
	}
}

