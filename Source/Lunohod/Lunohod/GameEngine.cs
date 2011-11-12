using System;
using System.Collections.Generic;
using System.Linq;
using Lunohod.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace Lunohod
{
	public class GameEngine : Game
	{
        public const string MetadataRootDirectory = "Metadata";
        public const string ContentRootDirectory = "Content";

		private GraphicsDeviceManager graphics;
		private ScreenEngine screenEngine;
		
		private GameEventQueue eventQueue;
		
		private TouchCollection touches;
		private KeyboardProcessor keyboardProcessor;
		
		private XGame gameObject;

		public Texture2D BlankTexture { get; private set; }

		public XGame GameObject
		{
			get { return this.gameObject; }
		}		
		
		public TouchCollection Touches
		{
			get { return this.touches; }
		}

		public ScreenEngine ScreenEngine
		{
			get { return this.screenEngine; }
		}		
		public GameEngine()
		{
            Content.RootDirectory = ContentRootDirectory;

			graphics = new GraphicsDeviceManager(this);
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
			graphics.PreferMultiSampling = true;
            this.IsFixedTimeStep = false;


            keyboardProcessor = new KeyboardProcessor(this);
			this.eventQueue = new GameEventQueue();

#if WINDOWS
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 320;
            graphics.PreferredBackBufferWidth = 480;
#endif
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
		
		public void EnqueueEvent(GameEvent e)
		{
			this.eventQueue.Enqueue(e);
		}
		
		/*
        List<Tuple<DateTime, DateTime, string>> log = new List<Tuple<DateTime,DateTime,string>>();
        DateTime begin = DateTime.Now;

		
		int[] ccounts = new int[3];
		
		private void CollectionCounts()
		{
			for (int i = 0; i < ccounts.Length; i++)
			{
				int c = GC.CollectionCount(i);
				if (ccounts[i] != c)
				{
					Console.WriteLine("Collection occured. Generation: {0} Count: {1}", i, c);
					ccounts[i] = c;
				}
			}
		}
		*/
		
		protected override void Update(GameTime gameTime)
		{
            //DateTime start = DateTime.Now;
			this.touches = TouchPanel.GetState();

            keyboardProcessor.Process(gameTime);

			if (screenEngine == null)
			{
				screenEngine = new LevelEngine(this, "TestLevel");
				screenEngine.Initialize();
				
				GC.Collect();
			}
			
			ProcessQueue();
			
			base.Update (gameTime);

			screenEngine.Update(gameTime);
			
			//CollectionCounts();

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

		protected void ProcessQueue()
		{
			while (this.eventQueue.Count > 0)
			{
				var e = this.eventQueue.Dequeue();
				
				this.screenEngine.ProcessEvent(e);
				
				if (e.IsHandled)
					continue;
				
				// handle the event on system level
			}
		}
		
		public bool ProcessTouch(GameTime gameTime, int x, int y)
		{
			var tapAreas = this.ScreenEngine.tapAreas;
			
			for(int j = 0; j < tapAreas.Count; j++)
			{
				var tapArea = tapAreas[j];
				
				if (tapArea.Bounds.Contains(x, y))
				{
		            this.EnqueueEvent(new GameEvent(tapArea.Event, gameTime));
					
					return true;
				}
			}
			return false;
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