using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod.Objects
{
    public enum XEdgeType
    {
        Bounce,
        Stick,
        Teleport
    }

    public enum XAlignType
    {
        Top = 0,
        Bottom = 1,
        Left = 2,
        Right = 3
    }

    [XmlType("Edge")]
    public class XEdge : XElement
    {
        [XmlAttribute]
        public XAlignType Align;
        [XmlAttribute]
        public XEdgeType Type = XEdgeType.Stick;

        public virtual void ProcessCollision(LevelEngine engine, XBlock block, Rectangle newBounds)
        {
        }

        public Color GetDefaultColor()
        {
            switch (this.Type)
            {
                case XEdgeType.Bounce: return Color.Black;
                case XEdgeType.Stick: return Color.Green;
                case XEdgeType.Teleport: return Color.Orange;
            }
            return Color.Black;
        }
		
		public override void Initialize(InitializeParameters p)
		{
			if (!this.OverridesBackColor)
				this.BackColor = this.GetDefaultColor();
			
			base.Initialize(p);
		}
		
		public override void Draw(DrawParameters p)
		{
			var blockBounds = ((XBlock)this.Parent).Bounds;
			
			switch (this.Align)
			{
				case XAlignType.Top :
				{
					this.Bounds.X = 0;
					this.Bounds.Y = 0;
					this.Bounds.Width = blockBounds.Width;
					this.Bounds.Height = 3;
				}; break;
				case XAlignType.Bottom :
				{
					this.Bounds.X = 0;
					this.Bounds.Y = blockBounds.Height - 3;
					this.Bounds.Width = blockBounds.Width;
					this.Bounds.Height = 3;
				}; break;
				case XAlignType.Left :
				{
					this.Bounds.X = 0;
					this.Bounds.Y = 0;
					this.Bounds.Width = 3;
					this.Bounds.Height = blockBounds.Height;
				}; break;
				case XAlignType.Right :
				{
					this.Bounds.X = blockBounds.Width - 3;
					this.Bounds.Y = 0;
					this.Bounds.Width = 3;
					this.Bounds.Height = blockBounds.Height;
				}; break;
			}
			
			p.SpriteBatch.Draw(p.Game.BlankTexture, this.GetScreenBounds(), this.BackColor);
		}
    }
}
