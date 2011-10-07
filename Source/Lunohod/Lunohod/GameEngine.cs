using System;
using Lunohod.Objects;
using Microsoft.Xna.Framework;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Lunohod
{
	public class GameEngine : Game
	{
        private GraphicsDeviceManager graphics;

		private XGame gameDesriptor;

		private ScreenEngine screenEngine;
		
		public Texture2D BlankTexture {get; private set;}

		public XGame GameDesriptor
		{
			get {
				return this.gameDesriptor;
			}
		}		

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
			
			LoadGameElement();
			
			base.LoadContent ();
		}

		protected void LoadGameElement()
		{
			string gameXmlFile = Path.Combine(this.Content.RootDirectory, "game.xml");
			
			try
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(XGame));
				
				using (FileStream stream = new FileStream(gameXmlFile, FileMode.Open))
				{
					this.gameDesriptor = (XGame)serializer.Deserialize(stream);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				
				throw;
			}
			
			gameDesriptor.InitHierarchy();
			gameDesriptor.Initialize(new InitializeParameters() { Game = this });
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

