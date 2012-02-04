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
using System.Diagnostics;

namespace Lunohod
{
	public class GameEngine : Game
	{
        public const string MetadataRootDirectory = "Metadata";
        public const string ContentRootDirectory = "Content";

		private GraphicsDeviceManager graphics;
		private ScreenEngine screenEngine;
		
		private GameEventQueue eventQueue;
		
		private XGame gameObject;
        private InputProcessorBase[] inputProcessors;
		
		private Vector3 scale = new Vector3(1,1,1);

		public Texture2D BlankTexture { get; private set; }
		public SpriteFont SystemFont { get; private set; }
		public List<Texture2D> WaveTextures { get; private set; }
        public const int MinWaveRadius = 200;
        public const int MaxWaveRadius = 1100;
        public const int WaveRadiusStep = 200;
		
		//public Stopwatch gameWatch = new Stopwatch();

		public XGame GameObject
		{
			get { return this.gameObject; }
		}		
		
		public ScreenEngine ScreenEngine
		{
			get { return this.screenEngine; }
		}	
		
		public Vector3 Scale
		{
			get { return this.scale; }
		}
		
		public bool DrawDebugInfo
		{
			get { 
#if WINDOWS
				return Keyboard.GetState().IsKeyDown(Keys.LeftShift)
                    || Keyboard.GetState().IsKeyDown(Keys.RightShift);
#else
				return false;
#endif
			}
		}
		
		public GameEngine()
		{
            Content.RootDirectory = ContentRootDirectory;

			graphics = new GraphicsDeviceManager(this);
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
			graphics.PreferMultiSampling = true;
            this.IsFixedTimeStep = false;

			this.eventQueue = new GameEventQueue();

#if WINDOWS
            inputProcessors = new InputProcessorBase[] {
                new KeyboardProcessor(this),
                new MouseProcessor(this)
            };

            this.IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 320;
            graphics.PreferredBackBufferWidth = 480;
#else
            inputProcessors = new InputProcessorBase[] {
                new TouchPanelProcessor(this)
            };
#endif
        }
		
		protected override void Initialize()
		{
			base.Initialize ();
			
			float singleScale = this.Window.ClientBounds.Height * 1.0f / 480.0f;
			
			this.scale = new Vector3(singleScale, singleScale, 1.0f);
			
			//this.gameWatch.Start();
		}
		
		protected override void LoadContent()
		{
            LoadGameElement();

			this.BlankTexture = ((XTextureResource)this.gameObject.FindDescendant("blank")).Image;
			this.SystemFont = ((XFontResource)this.gameObject.FindDescendant("SystemFont")).Font;
			
			this.WaveTextures = new List<Texture2D>();
			for (int i = MinWaveRadius; i <= MaxWaveRadius; i += WaveRadiusStep)
			{
				string textureId = "wave" + i.ToString("0000");
				this.WaveTextures.Add(((XTextureResource)this.gameObject.FindDescendant(textureId)).Image);
			}
			
			base.LoadContent();
		}

		protected void LoadGameElement()
		{
			this.gameObject = (XGame)LoadMetadata("Game.xml", typeof(XGame));
			
			gameObject.InitHierarchy();
			gameObject.Initialize(new InitializeParameters() { Game = this });
		}
		
		public static XObject LoadMetadata(string fileName, Type type)
		{
			string gameXmlFile = Path.Combine(GameEngine.MetadataRootDirectory, fileName);
			
			try
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(type);
				
				using (FileStream stream = new FileStream(gameXmlFile, FileMode.Open, FileAccess.Read))
				{
					 return (XObject)serializer.Deserialize(stream);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				
				throw;
			}
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
			//gameWatch.Stop();
			//gameTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, gameWatch.Elapsed);
			//gameWatch.Reset();
			//gameWatch.Start();

			if (screenEngine == null)
            {
                screenEngine = new LevelEngine(this, this.gameObject.Levels[0].File);
                screenEngine.Initialize();

                GC.Collect();
            }

			ProcessInputProcessors(gameTime);
            
            ProcessQueue(gameTime);
			
			base.Update(gameTime);

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
		
		
		private void ProcessInputProcessors(GameTime gameTime)
		{
            for (int i = 0; i < inputProcessors.Length; i++)
            {
                inputProcessors[i].Process(gameTime);
            }
		}			

		protected void ProcessQueue(GameTime gameTime)
		{
			for (int i = 0; i < this.eventQueue.Count; i++)
			{
				var e = this.eventQueue.Dequeue();
				
				this.screenEngine.ProcessEvent(gameTime, e);
				
				if (!e.IsHandled)
					this.eventQueue.Enqueue(e);
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

        public void MoveHero(int x, int y)
        {
            var levelEngine = this.ScreenEngine as LevelEngine;

            if (levelEngine == null)
                return;

            levelEngine.hero.Bounds.X = x - levelEngine.hero.Bounds.Width / 2;
            levelEngine.hero.Bounds.Y = y - levelEngine.hero.Bounds.Height / 2;
        }

		protected override void Draw(GameTime gameTime)
		{
			//gameWatch.Stop();
			//gameTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, gameWatch.Elapsed);
			//gameWatch.Reset();
			//gameWatch.Start();

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