using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod.Objects
{
	
    [XmlType("Group")]
	public class XGroup : XElement
	{
		private SpriteBatchWithFloats spriteBatch;
		
		public XGroup()
		{
		}
		
//		public override void Draw(DrawParameters p)
//		{
//			
//			if (this.spriteBatch == null)
//				this.spriteBatch = new SpriteBatchWithFloats(p.Game.GraphicsDevice);
//			
//			
//			var oldSpriteBatch = p.SpriteBatch;
//			
//			p.SpriteBatch = this.spriteBatch;
//
//			Matrix transform = Matrix.Identity *
//				Matrix.CreateTranslation(-this.Origin.X, -this.Origin.Y, 0) *
//				Matrix.CreateRotationZ(MathHelper.ToRadians(this.Rotation)) *
//				Matrix.CreateTranslation(this.Bounds.X, this.Bounds.Y, 0);
//			
//			
//			this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
//				DepthStencilState.None, RasterizerState.CullCounterClockwise, null, transform );
//			
//			base.Draw(p);
//			oldSpriteBatch.End();
//			this.spriteBatch.End();
//			
//			p.SpriteBatch = oldSpriteBatch;
//		}
	}
}

