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
		private const string SaveFileName = "scores.xml";
		private const string SettingsFileName = "settings.xml";
		
		private StorageDevice storageDevice;
		
		private GraphicsDeviceManager graphics;
        private List<ScreenEngine> screenEngines;

        private GameEventQueue eventQueue;

        private InputProcessorBase[] inputProcessors;
		
		public Texture2D BlankTexture { get; private set; }
		public SpriteFont SystemFont { get; private set; }
		public List<Texture2D> WaveTextures { get; private set; }
        
		public const int MinWaveTextureDiameter = 200;
        public const int MaxWaveTextureDiameter = 1100;
        public const int WaveTextureDiameterStep = 200;
		public const int MaxWaveTravelDistance = 577;
		
		//public Stopwatch gameWatch = new Stopwatch();
		
		public static GameEngine Instance { get; private set; }
		
		public XGame GameObject { get; private set; }
		public XScoreFile ScoreFile { get; private set; }
		public XSettingsFile SettingsFile { get; private set; }
		
		public int CycleNumber { get; private set; }
		
		public GameTime CurrentUpdateTime { get; set; }

        public List<ScreenEngine> ScreenEngines { get { return screenEngines; } }

        public GameEventQueue EventQueue { get { return eventQueue; } }

		public bool InBackground;

		public bool IsMute
		{
			get { return this.SettingsFile.MuteSound; }
			set
			{
				if (this.SettingsFile.MuteSound == value)
					return;

				this.SettingsFile.MuteSound = value;

				if (MuteChanged != null)
					MuteChanged(this, EventArgs.Empty);

				SaveSettings();
			}
		}
        
		public event EventHandler MuteChanged;

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
		
		public Vector2 Scale2D { get; private set; }
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
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 320;

			this.Scale2D = Vector2.One;
            this.Scale = Vector3.One;
#else
			this.Scale2D = new Vector2(
				(float)this.Window.ClientBounds.Width / 480f,
				(float)this.Window.ClientBounds.Height / 320f
			);
			this.Scale = new Vector3(
				this.Scale2D.X,
				this.Scale2D.Y,
				1f
			);
 
			inputProcessors = new InputProcessorBase[] {
                new TouchPanelProcessor(this)
            };
#endif
        }

#region Loading/Saving score
		
		private void LoadSettings()
		{
			this.SettingsFile = LoadFromContainerOrCreateNew(SettingsFileName, () => new XSettingsFile());
		}
		
		private void SaveSettings()
		{
			SaveToContainer(SettingsFileName, this.SettingsFile);
		}
		
		private void LoadScore()
		{
			// TODO: This is a mindfuck, we need to simplify it

			this.ScoreFile = LoadFromContainerOrCreateNew(SaveFileName, () =>
				new XScoreFile() { 
					LevelScores = this.GameObject.Levels.Select(l => 
						new XLevelScore(l)
					).ToList()
				}
			);

			this.ScoreFile.GenerateScoreDict();

			this.ScoreFile.LevelScores = this.GameObject.Levels.Select(l => {
				var score = new XLevelScore(l);
				XLevelScore scoreFromFile;
				if (this.ScoreFile.LevelScoreDict.TryGetValue(score.Id, out scoreFromFile))
					scoreFromFile.CopyTo(score);
				return score;
			}).ToList();

			this.ScoreFile.GenerateScoreDict();
		}
		
		public void SaveScore()
		{
			SaveToContainer(SaveFileName, this.ScoreFile);
		}
		
		private void ChooseStorageDevice()
		{
			// get device
            var result = StorageDevice.BeginShowSelector(null, null);
            result.AsyncWaitHandle.WaitOne();

            this.storageDevice = StorageDevice.EndShowSelector(result);

            result.AsyncWaitHandle.Close();
		}
		
        private T LoadFromContainerOrCreateNew<T>(string fileName, Func<T> createNew)
        {
			// get file
            var async = storageDevice.BeginOpenContainer("DynaCat", null, null);
            async.AsyncWaitHandle.WaitOne();

            using (StorageContainer container = storageDevice.EndOpenContainer(async))
			{
	            async.AsyncWaitHandle.Close();
				
				if (container.FileExists(fileName))
					return LoadFromContainer<T>(container, fileName);
			}

			T result = createNew();
			SaveToContainer(fileName, result);
			
			return result;
		}

		private T LoadFromContainer<T>(StorageContainer container, string fileName)
		{
			try
        	{
        		var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        		
        		using (var stream = container.OpenFile(fileName, FileMode.Open, FileAccess.Read))
        		{
        			var result = (T)serializer.Deserialize(stream);
					
					return result;
        		}
        	}
        	catch (Exception ex)
        	{
        		System.Diagnostics.Debug.WriteLine(ex.ToString());
        		
        		throw;
        	}
		}
		
		private void SaveToContainer<T>(string fileName, T o)
		{
			// get file
            var async = storageDevice.BeginOpenContainer("DynaCat", null, null);
            async.AsyncWaitHandle.WaitOne();

            using (StorageContainer container = storageDevice.EndOpenContainer(async))
			{
	            async.AsyncWaitHandle.Close();
				
				try
	        	{
	        		var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
	        		
	        		using (var stream = container.OpenFile(fileName, FileMode.Create, FileAccess.Write))
	        		{
	        			serializer.Serialize(stream, o);
	        		}
	        	}
	        	catch (Exception ex)
	        	{
	        		System.Diagnostics.Debug.WriteLine(ex.ToString());
	        		
	        		throw;
	        	}
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

			ChooseStorageDevice();
			
			Microsoft.Xna.Framework.Media.MediaPlayer.IsMuted = true;
			
			LoadSettings();
			
			LoadScore();
			
			LoadScreen(GameObject.StartScreen);

			base.LoadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			if (this.InBackground)
				return;

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
#if IPHONE
			MonoTouch.UIKit.UIApplication.SharedApplication.BeginIgnoringInteractionEvents();
#endif

            var newScreenEngine = new ScreenEngine(this, fileName, this.ScreenEngine);

			lock(this.screenEngines)
			{
				screenEngines.Add(newScreenEngine);
				this.EnqueueEvent(new GameEvent(GameEventType.ScreenActivated, this.CurrentUpdateTime ?? new GameTime(), true));
			}

			newScreenEngine.Initialize();

			ResetControllers();

			GC.Collect();
		}

		public void LoadLevel(XLevelInfo levelInfo)
		{
#if IPHONE
			MonoTouch.UIKit.UIApplication.SharedApplication.BeginIgnoringInteractionEvents();
#endif

			var newScreenEngine = new LevelEngine(this, this.ScreenEngine, levelInfo, this.ScoreFile.LevelScoreDict[levelInfo.Id]);

			lock(this.screenEngines)
			{
				screenEngines.Add(newScreenEngine);
				this.EnqueueEvent(new GameEvent(GameEventType.ScreenActivated, this.CurrentUpdateTime ?? new GameTime(), true));
			}
			
			newScreenEngine.Initialize();

			ResetControllers();

			GC.Collect();
		}
		
		protected void LoadGameElement()
		{
			this.GameObject = LoadXml<XGame>("Game.xml");
			
			GameObject.InitHierarchy();
			GameObject.Initialize(new InitializeParameters() { Game = this });

			PrepareGlobals();
		}

		void ResetControllers()
		{
			this.inputProcessors.ForEach(c => c.ResetController());
		}

		public void CloseCurrentScreen()
		{
			ScreenEngine screenEngine;
			
			lock(this.screenEngines)
			{
				screenEngine = this.screenEngines[this.screenEngines.Count - 1];
				this.screenEngines.Remove(screenEngine);
				this.EnqueueEvent(new GameEvent(GameEventType.ScreenActivated, this.CurrentUpdateTime ?? new GameTime(), true));
			}
			
			screenEngine.Unload();
			
			GC.Collect();
		}
		
		void PrepareGlobals()
		{
			this.BlankTexture = ((XTextureResource)this.GameObject.FindLocal("blank")).Image;
			this.SystemFont = ((XFontResource)this.GameObject.FindLocal("SystemFont")).Font;
			
			this.WaveTextures = new List<Texture2D>();
			for (int i = MinWaveTextureDiameter; i <= MaxWaveTextureDiameter; i += WaveTextureDiameterStep)
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
			string gameXmlFile = Path.Combine(GameEngine.MetadataRootDirectory, fileName.Replace('/', Path.DirectorySeparatorChar));
			
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

				e.IsHandled = true;

				switch (e.EventType)
                {
                    case GameEventType.Pause:
						{
                            this.eventQueue.Clear();
							numOfEvents = 0;
							
							LoadScreen(this.GameObject.PauseScreen);
						} break;
                    case GameEventType.StartNextLevel:
                        {
                            this.eventQueue.Clear();
							numOfEvents = 0;

                            var levelInfo = this.LevelEngine.LevelInfo;
                            this.CloseCurrentScreen();

                            var series = this.GameObject.LevelSeries.Where(s => s.Levels.Contains(levelInfo)).First();

                            var levelIndex = series.Levels.IndexOf(levelInfo);

                            levelIndex++;

                            if (levelIndex < series.Levels.Count)
                                this.LoadLevel(series.Levels[levelIndex]);
                        } break;
                    case GameEventType.RestartLevel:
                        {
                            this.eventQueue.Clear();
							numOfEvents = 0;

							var levelInfo = this.LevelEngine.LevelInfo;
                            this.CloseCurrentScreen();
                            this.LoadLevel(levelInfo);
                        } break;
                    case GameEventType.EndLevel:
						{
							goto case GameEventType.AbandonLevel;
						}
                    case GameEventType.AbandonLevel:
                        {
                            this.eventQueue.Clear();
							i = -1;

                            this.CloseCurrentScreen();

                            // break out of the loop
                            numOfEvents = this.eventQueue.Count;
                        } break;
                    case GameEventType.CloseCurrentScreen:
                        {
                            this.CloseCurrentScreen();
                        } break;
				default: 
					e.IsHandled = false; break;
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
				
				if (!(tapArea.Enabled && tapArea.WasUpdated))
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