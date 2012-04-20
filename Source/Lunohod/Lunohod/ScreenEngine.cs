using System;
using Microsoft.Xna.Framework;
using System.IO;
using Lunohod.Objects;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod
{
	public class ScreenEngine
	{
		protected GameEngine game;
		
		// cycle parameters
		protected InitializeParameters initializeParameters;
		protected UpdateParameters updateParameters;
		protected DrawParameters drawParameters;
		
		protected SpriteBatchWithFloats spriteBatch;
		private Matrix? scaleMatrix;
		
		// FPS plumbing
		private Stopwatch fpsSw = new Stopwatch();
		private Vector2 fpsPos = new Vector2(5, 280);
		private double fps = 0;
		
		private List<string> tempList = new List<string>();
		
		public List<XTapArea> tapAreas;
		public List<XElement> obstacles;

        public string FileName { get; private set; }
        
        public virtual Type RootComponentType { get { return typeof(XScreen); } }
		
		public XScreen RootComponent { get; protected set; }

		public Dictionary<string, bool> CurrentEvents { get; private set; }
		
		public ScreenEngine(GameEngine game, string fileName)
		{
			this.game = game;
			this.FileName = fileName;
			this.CurrentEvents = new Dictionary<string, bool>();
		}
		
		public virtual void ProcessEvent(GameTime gameTime, GameEvent e)
		{
			int switchCharIndex = e.Id.IndexOf(':') + 1;
			int switchChar = e.Id[switchCharIndex];
			
			if (switchChar == '+')
			{
				// turning on the event permanently
				string eventId = e.Id.Remove(switchCharIndex, 1);
				this.CurrentEvents[eventId] = true;
			}
			else if (switchChar == '*')
			{
				// switching the event on/off
				string eventId = e.Id.Remove(switchCharIndex, 1);
				bool currValue = false;
				this.CurrentEvents.TryGetValue(eventId, out currValue);
				this.CurrentEvents[eventId] = currValue ^ true;
			}
			else if (switchChar == '-')
			{
				// removing the event
				string eventId = e.Id.Remove(switchCharIndex, 1);
				this.CurrentEvents.Remove(eventId);
			}
			else
			{
				// turning on the event for one update
				this.CurrentEvents[e.Id] = false;
			}
		}

		public virtual bool EventAllowed(GameEvent e)
		{
			return true;
		}
		
		public virtual void Initialize()
		{
			PerfMon.Start("ScreenInitialize");

			this.tapAreas = new List<XTapArea>();
			this.obstacles = new List<XElement>();

			spriteBatch = new SpriteBatchWithFloats(this.game.GraphicsDevice);
			
			if (this.game.Scale != Vector3.One)
				scaleMatrix = Matrix.CreateScale(this.game.Scale);

			initializeParameters = new InitializeParameters() { Game = game, ScreenEngine = this };
			updateParameters = new UpdateParameters() { Game = game, ScreenEngine = this };
			drawParameters = new DrawParameters() { Game = game, ScreenEngine = this, SpriteBatch = spriteBatch };
			
			PerfMon.Start("LoadXml");
			this.RootComponent = (XScreen)GameEngine.LoadXml(this.FileName, this.RootComponentType);
			PerfMon.Stop("LoadXml");

			PerfMon.Start("InitHierarchy");
			this.RootComponent.InitHierarchy();
			PerfMon.Stop("InitHierarchy");

			PerfMon.Start("Initialize");
			this.RootComponent.Initialize(initializeParameters);
			PerfMon.Stop("Initialize");

			PerfMon.Stop("ScreenInitialize");
			Console.WriteLine(PerfMon.Dump());
		}
		
		public virtual void Update(GameTime gameTime)
		{
			updateParameters.GameTime = gameTime;
			this.RootComponent.Update(updateParameters);

			if (this.CurrentEvents.Count > 0)
			{
				this.tempList.AddRange(this.CurrentEvents.Keys);
				for(int i = 0; i < tempList.Count; i++)
				{
					if (!this.CurrentEvents[tempList[i]])
						this.CurrentEvents.Remove(tempList[i]);
				}
				this.tempList.Clear();
			}
		}
		
		protected void PreDraw(GameTime gameTime)
		{
			drawParameters.Initiazlize(gameTime);

			if (this.scaleMatrix.HasValue)
			{
				this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearClamp,
					DepthStencilState.None, RasterizerState.CullCounterClockwise, null, this.scaleMatrix.Value);
			}
			else
			{
				this.spriteBatch.Begin(SpriteSortMode.FrontToBack, null);
			}
		}
		
		protected void PostDraw()
		{
			if (this.game.GameObject.ShowFPS)
				this.DrawDebugInfo(drawParameters);
			
			this.spriteBatch.End();
		}
		
		public virtual void Draw(GameTime gameTime)
		{
			try 
			{
				PreDraw(gameTime);
				
				this.RootComponent.Draw(drawParameters);
				
				PostDraw();				
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		public void DrawDebugInfo(DrawParameters dp)
		{
			fpsSw.Stop();
			if (fpsSw.Elapsed.TotalMilliseconds > 0)
				fps = fps * 0.9 + (1000.0 / fpsSw.Elapsed.TotalMilliseconds) * 0.1;
			fpsSw.Reset();
			fpsSw.Start();
			
			dp.SpriteBatch.DrawString(game.SystemFont, Math.Round(fps).ToString(), fpsPos, Color.Red);
		}

		public virtual void Unload()
		{
			this.RootComponent.Dispose();
			this.RootComponent = null;

			this.spriteBatch.Dispose();
			this.spriteBatch = null;
			
			this.obstacles = null;
		}
	}
}

