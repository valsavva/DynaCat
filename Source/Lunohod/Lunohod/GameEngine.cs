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
using Microsoft.Xna.Framework.Storage;

namespace Lunohod
{
	public class GameEngine : Game
	{
        public const string MetadataRootDirectory = "Metadata";
        public const string ContentRootDirectory = "Content";
		private const string SaveFileName = "records.xml";
		
		private StorageDevice storageDevice;
		
		private GraphicsDeviceManager graphics;
        private List<ScreenEngine> screenEngines;

        private GameEventQueue eventQueue;

        private InputProcessorBase[] inputProcessors;
		
		public Texture2D BlankTexture { get; private set; }
		public SpriteFont SystemFont { get; private set; }
		public List<Texture2D> WaveTextures { get; private set; }
        public const int MinWaveRadius = 200;
        public const int MaxWaveRadius = 1100;
        public const int WaveRadiusStep = 200;
		
		//public Stopwatch gameWatch = new Stopwatch();
		
		public static GameEngine Instance { get; private set; }
		
		public XGame GameObject { get; private set; }
		public XSaveFile Score { get; private set; }
		
		public int CycleNumber { get; private set; }
		
		public GameTime CurrentUpdateTime { get; set; }

        public List<ScreenEngine> ScreenEngines { get { return screenEngines; } }

        public GameEventQueue EventQueue { get { return eventQueue; } }
        
		public ScreenEngine ScreenEngine
		{
			get
			{ 
				lock(this.screenEngines)
				{
					return this.screenEngines.Count == 0 ? null : this.screenEngines[this.screenEngines.Count - 1];
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

#region Loading/Saving score
        private void LoadScore()
        {
			// get device
            IAsyncResult result = StorageDevice.BeginShowSelector(null, null);
            result.AsyncWaitHandle.WaitOne();

            this.storageDevice = StorageDevice.EndShowSelector(result);

            result.AsyncWaitHandle.Close();
			
			// get file
            result = storageDevice.BeginOpenContainer("DynaCat", null, null);
            result.AsyncWaitHandle.WaitOne();

            using (StorageContainer container = storageDevice.EndOpenContainer(result))
			{
	            result.AsyncWaitHandle.Close();
				
				if (container.FileExists(SaveFileName))
				{
					LoadScoreFromContainer(container);
				}
				else
				{
					CreateSaveFile(container);
				}
				
			}
        }

		private void LoadScoreFromContainer(StorageContainer container)
		{
			try
        	{
        		var serializer = new System.Xml.Serialization.XmlSerializer(typeof(XSaveFile));
        		
        		using (var stream = container.OpenFile(SaveFileName, FileMode.Open, FileAccess.Read))
        		{
        			this.Score = (XSaveFile)serializer.Deserialize(stream);
        		}
        	}
        	catch (Exception ex)
        	{
        		System.Diagnostics.Debug.WriteLine(ex.ToString());
        		
        		throw;
        	}
		}
		
		private void CreateSaveFile(StorageContainer container)
		{
			this.Score = new XSaveFile();
			this.Score.LevelScores = this.GameObject.Levels.Select(l => 
				new XLevelScore(l)
			).ToList();
			
			SaveScoreToContainer(container);
		}
		
		private void SaveScore()
		{
			// get file
            var result = storageDevice.BeginOpenContainer("DynaCat", null, null);
            result.AsyncWaitHandle.WaitOne();

            using (StorageContainer container = storageDevice.EndOpenContainer(result))
			{
	            result.AsyncWaitHandle.Close();
				
				SaveScoreToContainer(container);
			}
		}
		
		private void SaveScoreToContainer(StorageContainer container)
		{
			try
        	{
        		var serializer = new System.Xml.Serialization.XmlSerializer(typeof(XSaveFile));
        		
        		using (var stream = container.OpenFile(SaveFileName, FileMode.Create, FileAccess.Write))
        		{
        			serializer.Serialize(stream, this.Score);
        		}
        	}
        	catch (Exception ex)
        	{
        		System.Diagnostics.Debug.WriteLine(ex.ToString());
        		
        		throw;
        	}
		}
		
#endregion
		
		#region Standard method overrides
		
		protected override void Initialize()
		{
			base.Initialize ();
		}
		
		protected override void LoadContent()
		{
            LoadGameElement();

			LoadScore();

			LoadScreen(GameObject.StartScreen);

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
            var newScreenEngine = new ScreenEngine(this, fileName, this.ScreenEngine);

			lock(this.screenEngines)
			{
				screenEngines.Add(newScreenEngine);
			}

			newScreenEngine.Initialize();

			GC.Collect();
		}

		public void LoadLevel(XLevelInfo levelInfo)
		{
			var newScreenEngine = new LevelEngine(this, this.ScreenEngine, levelInfo, new XLevelScore());

			lock(this.screenEngines)
			{
				screenEngines.Add(newScreenEngine);
			}
			
			newScreenEngine.Initialize();

			GC.Collect();
		}
		
		protected void LoadGameElement()
		{
			this.GameObject = LoadXml<XGame>("Game.xml");
			
			GameObject.InitHierarchy();
			GameObject.Initialize(new InitializeParameters() { Game = this });

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
			this.BlankTexture = ((XTextureResource)this.GameObject.FindLocal("blank")).Image;
			this.SystemFont = ((XFontResource)this.GameObject.FindLocal("SystemFont")).Font;
			
			this.WaveTextures = new List<Texture2D>();
			for (int i = MinWaveRadius; i <= MaxWaveRadius; i += WaveRadiusStep)
			{
				string textureId = "wave" + i.ToString("0000");
				this.WaveTextures.Add(((XTextureResource)this.GameObject.FindLocal(textureId)).Image);
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
                    case GameEventType.StartNextLevel:
                        {
                            var levelInfo = this.LevelEngine.LevelInfo;
                            this.CloseCurrentScreen();

                            var series = this.GameObject.LevelSeries.Where(s => s.Levels.Contains(levelInfo)).First();

                            var levelIndex = series.Levels.IndexOf(levelInfo);

                            levelIndex++;

                            if (levelIndex < series.Levels.Count)
                                this.LoadLevel(series.Levels[levelIndex]);
    
                            e.IsHandled = true;

                            // break out of the loop
                            this.eventQueue.Clear();
                            numOfEvents = 0;
                            continue;
                        } break;
                    case GameEventType.RestartLevel:
                        {
                            var levelInfo = this.LevelEngine.LevelInfo;
                            this.CloseCurrentScreen();
                            this.LoadLevel(levelInfo);
                            e.IsHandled = true;

                            // break out of the loop
                            this.eventQueue.Clear();
                            numOfEvents = 0;
                            continue;
                        } break;
                    case GameEventType.EndLevel:
						{
							goto case GameEventType.AbandonLevel;
						}
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