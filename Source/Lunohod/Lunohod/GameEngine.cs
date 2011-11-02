using System;
using System.Collections.Generic;
using System.Linq;
using Lunohod.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text;

namespace Lunohod
{
	public class GameEngine : Game
	{
        public const string MetadataRootDirectory = "Metadata";
        private GraphicsDeviceManager graphics;

		private XGame gameObject;

		private ScreenEngine screenEngine;

        public Texture2D BlankTexture { get; private set; }

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
            this.IsFixedTimeStep = false;
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
            LoadGameElement();
            this.BlankTexture = ((XTextureResource)this.gameObject.FindDescendant("blank")).Image;
			
			base.LoadContent();
		}

		protected void LoadGameElement()
		{
			string gameXmlFile = Path.Combine(GameEngine.MetadataRootDirectory, "Game.xml");
			
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

        List<Tuple<DateTime, DateTime, string>> log = new List<Tuple<DateTime,DateTime,string>>();
        DateTime begin = DateTime.Now;

		protected override void Update(GameTime gameTime)
		{
            //DateTime start = DateTime.Now;

			base.Update (gameTime);
			
			if (screenEngine == null)
			{
				screenEngine = new LevelEngine(this, "TestLevel");
				screenEngine.Initialize();
			}

			screenEngine.Update(gameTime);
			
            //log.Add(Tuple.Create(start, DateTime.Now, "Update"));

            //if (begin.AddSeconds(10) <= DateTime.Now)
            //{
            //    System.Threading.ThreadPool.QueueUserWorkItem(o =>
            //    {
            //        StringBuilder sb = new StringBuilder();
            //        var array = log.ToArray();
            //        log.Clear();
            //        begin = DateTime.Now;
            //        foreach (var t in array)
            //        {
            //            sb.Append(string.Format(
            //                "\r\n{0} {1} {2}", t.Item1.ToString("MM:ss:ffff"), t.Item2.ToString("MM:ss:ffff"), t.Item3
            //                ));
            //        }

            //        File.WriteAllText("log.txt", sb.ToString());
            //    });
            //}


            //var touches = TouchPanel.GetState();
            //touches
            //    .Where(t => t.State == TouchLocationState.Released)
            //    .ToArray()
            //    .ForEach(t => Console.WriteLine("Touch! {0}", t.Position.ToString()));
		}

		protected override void Draw(GameTime gameTime)
		{
            //DateTime start = DateTime.Now;
            
            GraphicsDevice.Clear(Color.CornflowerBlue);
			
			this.screenEngine.Draw(gameTime);
			
			base.Draw(gameTime);

            //log.Add(Tuple.Create(start, DateTime.Now, "Draw"));
        }
		
		protected override void UnloadContent()
		{
			base.UnloadContent ();
		}
	}
}

