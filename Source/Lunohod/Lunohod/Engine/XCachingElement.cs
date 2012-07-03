using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml.Serialization;

namespace Lunohod
{
	public class XCachingElement : XElement
	{
		private RenderTarget2D cachedContent;
		private bool updated;

		[XmlAttribute]
		public bool CacheContent;
		
		public override void Update(UpdateParameters p)
		{
			if (!CacheContent || !updated)
			{
				base.Update(p);
				updated = true;
			}
			else
				this.TraveseTree(o => o.PreventUpdate());
		}
		
//		public override void Draw(DrawParameters p)
//		{
//			if (this.CacheContent && updated)
//			{
//				if (cachedContent == null)
//				{
//					cachedContent = new RenderTarget2D(
//						p.Game.GraphicsDevice,
//						p.Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
//						p.Game.GraphicsDevice.PresentationParameters.BackBufferHeight,
//			           	false, SurfaceFormat.Color, DepthFormat.None
//			   		);
//
//					p.SpriteBatch.End();
//					//var oldSpriteBatch = p.SpriteBatch;
//					//p.SpriteBatch = new SpriteBatchWithFloats(p.Game.GraphicsDevice);
//
//					p.Game.GraphicsDevice.SetRenderTarget(cachedContent);
//					p.Game.GraphicsDevice.Clear(Color.Transparent);
//					
//					p.SpriteBatch.Begin();
//					
//					base.Draw(p);
//					
//					p.SpriteBatch.End();
//					
//					p.Game.GraphicsDevice.SetRenderTarget(null);
//					
//					p.SpriteBatch.Begin();
//					//p.SpriteBatch = oldSpriteBatch;
//				}
//
//				p.SpriteBatch.Draw(cachedContent, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, p.NextSystemImageDepth());
//			}
//			else
//			{
//				if (cachedContent != null)
//				{
//					cachedContent.Dispose();
//					cachedContent = null;
//				}
//
//				base.Draw(p);
//			}
//			
//		}
		
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsBoolean("CacheContent", ref this.CacheContent);
			
			base.ReadXml(reader);
        }
		
		public override void Dispose()
		{
			if (cachedContent != null)
			{
				cachedContent.Dispose();
				cachedContent = null;
			}
			
			base.Dispose();
		}
	}
}

