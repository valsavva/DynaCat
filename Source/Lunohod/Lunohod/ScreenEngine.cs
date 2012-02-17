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
		protected string fileName;
		
		// cycle parameters
		protected InitializeParameters initializeParameters;
		protected UpdateParameters updateParameters;
		protected DrawParameters drawParameters;
		
		protected SpriteBatchWithFloats spriteBatch;
		
		// FPS plumbing
		private Stopwatch fpsSw = new Stopwatch();
		private Vector2 fpsPos = new Vector2(5, 280);
		private double fps = 0;
		
		public List<XTapArea> tapAreas;
		public List<XElement> obstacles;
		
		public ScreenEngine(GameEngine game, string fileName)
		{
			this.game = game;
			this.fileName = fileName;
		}
		
		public virtual Type RootComponentType { get { return typeof(XScreen); } }
		
		public XObject RootComponent { get; protected set; }
		
		public virtual void ProcessEvent(GameTime gameTime, GameEvent e)
		{
			
		}

		public virtual bool EventAllowed(GameEvent e)
		{
			return true;
		}
		
		public virtual void Initialize()
		{
			this.tapAreas = new List<XTapArea>();
			this.obstacles = new List<XElement>();

			spriteBatch = new SpriteBatchWithFloats(this.game.GraphicsDevice);

			initializeParameters = new InitializeParameters() { Game = game, ScreenEngine = this };
			updateParameters = new UpdateParameters() { Game = game, ScreenEngine = this };
			drawParameters = new DrawParameters() { Game = game, ScreenEngine = this, SpriteBatch = spriteBatch };
			
			this.RootComponent = GameEngine.LoadXml(this.fileName, this.RootComponentType);
			this.RootComponent.InitHierarchy();
			this.RootComponent.Initialize(initializeParameters);
		}
		
		public virtual void Update(GameTime gameTime)
		{
			updateParameters.GameTime = gameTime;
			this.RootComponent.Update(updateParameters);
		}
		
		protected void PreDraw(GameTime gameTime)
		{
			drawParameters.GameTime = gameTime;

			if (this.game.Window.ClientBounds.Height > 900)
			{
				Matrix transform = Matrix.Identity *
					Matrix.CreateScale(2.0f);
				this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
					DepthStencilState.None, RasterizerState.CullCounterClockwise, null, transform );
			}
			else
			{
				this.spriteBatch.Begin();
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

