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
		None,
        Bounce,
        Stick,
        Teleport
    }

    public enum XAlignType
    {
        Top = 0,
        Left = 1,
        Right = 2,
        Bottom = 3
    }

    [XmlType("Edge")]
    public class XEdge : XElement
    {
		private XBlock parentBlock;
		
		[XmlAttribute]
        public XAlignType Align;
        [XmlAttribute]
        public XEdgeType Type = XEdgeType.Stick;
		
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
			if (!this.UseBackColor)
				this.BackColor = this.GetDefaultColor();
			
			this.parentBlock = (XBlock)this.Parent;
			
			base.Initialize(p);
		}
		
		public override void Draw(DrawParameters p)
		{
			if (this.Type == XEdgeType.None)
				return;
			
			var blockBounds = this.parentBlock.Bounds;
			
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
			
			//p.SpriteBatch.Draw(p.Game.BlankTexture, this.GetScreenBounds(), this.BackColor);
		}

		public override bool ProcessCollision(LevelEngine level, Rectangle intersect)
        {
			switch (this.Type)
			{
				case XEdgeType.None : break;
				case XEdgeType.Bounce : {
					// change hero's direction
					level.hero.Direction = this.Align.ToVector();
					
					level.hero.Bounds.Offset(
						(int)(intersect.Width * level.hero.Direction.X),
						(int)(intersect.Height * level.hero.Direction.Y)
					);
				}; break;
				case XEdgeType.Stick : {
					// don't change hero's direction, just keep him in place
					var direction = this.Align.ToVector();
					
					level.hero.Bounds.Offset(
						(int)(intersect.Width * direction.X),
						(int)(intersect.Height * direction.Y)
					);
				}; break;
				case XEdgeType.Teleport : {
					return ProcessTeleport(level, intersect);
				};
			}
			
			return true;
        }
		private bool ProcessTeleport(LevelEngine level, Rectangle intersect)
		{
			return true;
			
			var heroDirectionAlign = level.hero.Direction.ToAlign();
			
			if (this.Align == heroDirectionAlign)
				// hero is exiting
				return true;
			
		}
    }
}
