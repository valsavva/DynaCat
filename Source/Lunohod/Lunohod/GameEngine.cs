using System;
using System.Collections.Generic;
using System.Linq;
using Lunohod.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Lunohod
{
	public class GameEngine : Game
	{
        private GraphicsDeviceManager graphics;

		private XGame gameObject;

		private ScreenEngine screenEngine;

        public Texture2D BlankTexture { get; private set; }
        public Texture2D chargersTexture { get; private set; }

		public XGame GameObject
		{
			get {
				return this.gameObject;
			}
		}		

		public GameEngine()
		{
            graphics = new GraphicsDeviceManager(this);
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
			graphics.PreferMultiSampling = true;

#if WINDOWS
            graphics.PreferredBackBufferHeight = 320;
            graphics.PreferredBackBufferWidth = 480;
#endif


            Content.RootDirectory = "Content";
		}
		
		protected override void Initialize()
		{
			base.Initialize ();
		}
		
		protected override void LoadContent()
		{
			this.BlankTexture = this.Content.Load<Texture2D>("Global/blank");
            this.chargersTexture = this.Content.Load<Texture2D>("Global/ch");
			
			LoadGameElement();
			
			base.LoadContent ();
		}

		protected void LoadGameElement()
		{
			string gameXmlFile = Path.Combine(this.Content.RootDirectory, "Game.xml");
			
			try
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(typeof(XGame));
				
				using (FileStream stream = new FileStream(gameXmlFile, FileMode.Open, FileAccess.Read))
				{
					this.gameObject = (XGame)serializer.Deserialize(stream);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				
				throw;
			}
			
			gameObject.InitHierarchy();
			gameObject.Initialize(new InitializeParameters() { Game = this });
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
			
            //var touches = TouchPanel.GetState();
            //touches
            //    .Where(t => t.State == TouchLocationState.Released)
            //    .ToArray()
            //    .ForEach(t => Console.WriteLine("Touch! {0}", t.Position.ToString()));
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

