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
#else
            inputProcessors = new InputProcessorBase[] {
                new TouchPanelProcessor(this)
            };
#endif
        }
		
		
		#region Standard method overrides
		
		protected override void Initialize()
		{
			base.Initialize ();
			
			float singleScale = this.Window.ClientBounds.Height * 1.0f / 480.0f;
			
			this.Scale = new Vector3(singleScale, singleScale, 1.0f);
		}
		
		protected override void LoadContent()
		{
            LoadGameElement();

			//LoadLevel(gameObject.Levels[0].File);
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
			
			this.ScreenEngine.Draw(gameTime);
			/*
			for(int i = 0; i < screenEngines.Count; i++)
				screenEngines[i].Draw(gameTime);
				*/
			
			base.Draw(gameTime);
        }
		
		protected override void UnloadContent()
		{
			base.UnloadContent ();
		}

		#endregion
		
		#region Content management

		public void LoadScreen(string id)
		{
            var newScreenEngine = new ScreenEngine(this, this.gameObject.Screens.First(s => s.Id == id).File);

			lock(this.screenEngines)
			{
				screenEngines.Add(newScreenEngine);
			}
			
			newScreenEngine.Initialize();

			GC.Collect();
		}

		public void LoadLevel(string id)
		{
            var newScreenEngine = new LevelEngine(this, this.gameObject.Levels.First(l => l.Id == id).File);

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
			
			this.eventQueue.Clear();
			
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
					case GameEventType.EndCurrentLevel :
					case GameEventType.CloseCurrentScreen :{
						this.CloseCurrentScreen();
						e.IsHandled = true;
					}; break;
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
		
		public bool ProcessTouch(GameTime gameTime, int x, int y)
		{
			var tapAreas = this.ScreenEngine.tapAreas;
			
			for(int j = 0; j < tapAreas.Count; j++)
			{
				var tapArea = tapAreas[j];
				
				if (tapArea.GetScreenBounds().Contains(x, y))
				{
					tapArea.IsTapped = true;
					return true;
				}
			}
			return false;
		}

#endregion

	}
}