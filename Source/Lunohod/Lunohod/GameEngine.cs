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
        private List<ScreenEngine> screenEngines;

        private GameEventQueue eventQueue;

		private XGame gameObject;
        private InputProcessorBase[] inputProcessors;
		
		public Texture2D BlankTexture { get; private set; }
		public SpriteFont SystemFont { get; private set; }
		public List<Texture2D> WaveTextures { get; private set; }
        public const int MinWaveRadius = 200;
        public const int MaxWaveRadius = 1100;
        public const int WaveRadiusStep = 200;
		
		//public Stopwatch gameWatch = new Stopwatch();
		
		public static GameEngine Instance { get; private set; }

		public int CycleNumber { get; private set; }
		
		public GameTime CurrentUpdateTime { get; set; }

        public List<ScreenEngine> ScreenEngines { get { return screenEngines; } }

        public GameEventQueue EventQueue { get { return eventQueue; } }
        
        public XGame GameObject
		{
			get { return this.gameObject; }
		}		
		
		public ScreenEngine ScreenEngine
		{
			get
			{ 
				lock(this.screenEngines)
				{
					return this.screenEngines[this.screenEngines.Count - 1];
				}
			}
		}	
		
		public LevelEngine LevelEngine
		{
			get { return this.ScreenEngine as LevelEngine; }
		}
		
		public Vector3 Scale { get; private set; }
		
		public bool DrawDebugInfo
		{
			get { 
#if WINDOWS
				return Keyboard.GetState().IsKeyDown(Keys.LeftShift)
                    || Keyboard.GetState().IsKeyDown(Keys.RightShift);
#else
				return true;
#endif
			}
		}
		
		public GameEngine()
		{
			GameEngine.Instance = this;
			
			screenEngines = new List<ScreenEngine>();
			
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

            this.Scale = Vector3.One;
#else
			this.Scale = new Vector3(
				(float)this.Window.ClientBounds.Width / 480f,
				(float)this.Window.ClientBounds.Height / 320f,
				1f
			);
 
			inputProcessors = new InputProcessorBase[] {
                new TouchPanelProcessor(this)
            };
#endif
        }
		
		
		#region Standard method overrides
		
		protected override void Initialize()
		{
			base.Initialize ();
		}
		
		protected override void LoadContent()
		{
            LoadGameElement();

			LoadScreen(gameObject.StartScreen);

			base.LoadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			this.CycleNumber++;
			this.CurrentUpdateTime = gameTime;
			
			ProcessInputProcessors(gameTime);
            
            ProcessQueue(gameTime);
			
			this.ScreenEngine.Update(gameTime);
			/*
			for(int i = 0; i < screenEngines.Count; i++)
				screenEngines[i].Update(gameTime);
				*/

			base.Update(gameTime);
		}
		
		protected override void Draw(GameTime gameTime)
		{
            GraphicsDevice.Clear(Color.CornflowerBlue);
			
            for(int i = 0; i < screenEngines.Count; i++)
            {
                if (i == screenEngines.Count - 1 || screenEngines[i + 1].RootComponent.IsModal)
                    screenEngines[i].Draw(gameTime);
            }
			
			base.Draw(gameTime);
        }
		
		protected override void UnloadContent()
		{
			base.UnloadContent ();
		}

		#endregion
		
		#region Content management

		public void LoadScreen(string fileName)
		{
            var newScreenEngine = new ScreenEngine(this, fileName);

			lock(this.screenEngines)
			{
				screenEngines.Add(newScreenEngine);
			}

			newScreenEngine.Initialize();

			GC.Collect();
		}

		public void LoadLevel(string fileName)
		{
			var newScreenEngine = new LevelEngine(this, fileName);

			lock(this.screenEngines)
			{
				screenEngines.Add(newScreenEngine);
			}
			
			newScreenEngine.Initialize();

			GC.Collect();
		}
		
		protected void LoadGameElement()
		{
			this.gameObject = LoadXml<XGame>("Game.xml");
			
			gameObject.InitHierarchy();
			gameObject.Initialize(new InitializeParameters() { Game = this });

			PrepareGlobals();
		}

		public void CloseCurrentScreen()
		{
			ScreenEngine screenEngine;
			
			lock(this.screenEngines)
			{
				screenEngine = this.screenEngines[this.screenEngines.Count - 1];
				this.screenEngines.Remove(screenEngine);
			}
			
			screenEngine.Unload();
			
			GC.Collect();
		}
		
		void PrepareGlobals()
		{
			this.BlankTexture = ((XTextureResource)this.gameObject.FindDescendant("blank")).Image;
			this.SystemFont = ((XFontResource)this.gameObject.FindDescendant("SystemFont")).Font;
			
			this.WaveTextures = new List<Texture2D>();
			for (int i = MinWaveRadius; i <= MaxWaveRadius; i += WaveRadiusStep)
			{
				string textureId = "wave" + i.ToString("0000");
				this.WaveTextures.Add(((XTextureResource)this.gameObject.FindDescendant(textureId)).Image);
			}
		}
		
		public static T LoadXml<T>(string fileName)
			where T : XObject
		{
			return (T)LoadXml(fileName, typeof(T));
		}
		
		public static XObject LoadXml(string fileName, Type type)
		{
			string gameXmlFile = Path.Combine(GameEngine.MetadataRootDirectory, fileName);
			
			try
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(type);
				
				using (FileStream stream = new FileStream(gameXmlFile, FileMode.Open, FileAccess.Read))
				{
					var result = (XObject)serializer.Deserialize(stream);
					if (result is XScreen)
						result.Subcomponents.Insert(0, new XSystem());
					
					return result;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				
				throw;
			}
		}
		
		#endregion
		
		#region Event processing

		public void EnqueueEvent(GameEvent e)
		{
			if (!this.ScreenEngine.EventAllowed(e))
				return;
			
			this.eventQueue.Enqueue(e);
		}
		
		public void ProcessQueue(GameTime gameTime)
		{
			int numOfEvents = this.eventQueue.Count;

			for (int i = 0; i < numOfEvents; i++)
			{
				var e = this.eventQueue.Dequeue();

                switch (e.EventType)
                {
                    case GameEventType.RestartLevel:
                        {
                            var fileName = this.LevelEngine.FileName;
                            this.CloseCurrentScreen();
                            this.LoadLevel(fileName);
                            e.IsHandled = true;

                            // break out of the loop
                            this.eventQueue.Clear();
                            numOfEvents = 0;
                            continue;
                        } break;
                    case GameEventType.EndLevel:
                    case GameEventType.AbandonLevel:
                        {
                            this.CloseCurrentScreen();
                            e.IsHandled = true;

                            // break out of the loop
                            this.eventQueue.Clear();
                            numOfEvents = 0;
                            continue;
                            //
                        } break;
                    case GameEventType.CloseCurrentScreen:
                        {
                            this.CloseCurrentScreen();
                            e.IsHandled = true;
                        } break;
                }
				
				if (!e.IsHandled)
				{
					this.ScreenEngine.ProcessEvent(gameTime, e);
				}
				
				if (!e.IsHandled)
					this.eventQueue.Enqueue(e);
			}
		}
		
		private void ProcessInputProcessors(GameTime gameTime)
		{
            for (int i = 0; i < inputProcessors.Length; i++)
            {
                inputProcessors[i].Process(gameTime);
            }
		}			
		
		public bool ProcessTouch(GameTime gameTime, XTapType tapType, int x, int y)
		{
			var tapAreas = this.ScreenEngine.tapAreas;
			
			for(int j = tapAreas.Count - 1; j >= 0; j--)
			{
				var tapArea = tapAreas[j];
				
				if (!tapArea.WasUpdated)
					continue;
				
                var screenBounds = tapArea.GetScreenBounds();

				if (screenBounds.Contains(x, y))
				{
					tapArea.TapType = tapType;
                    tapArea.TapX = x - screenBounds.X;
                    tapArea.TapY = y - screenBounds.Y;
					return true;
				}
			}
			return false;
		}

#endregion

	}
}