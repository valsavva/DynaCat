using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Lunohod.Objects
{
    [XmlType("Sprite")]
	public class XSprite : XImage
	{
		[XmlIgnore]
		public Rectangle FrameBounds;
		[XmlAttribute]
		public int FrameCount;
        [XmlAttribute]
        public int CurrentFrame;
		
		public override void Draw(DrawParameters p)
		{
			this.SourceRectangle = new Rectangle(this.CurrentFrame * this.FrameBounds.Width, this.FrameBounds.Top, this.FrameBounds.Width, FrameBounds.Height);
			base.Draw(p);
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsRect("FrameBounds", ref this.FrameBounds);
            reader.ReadAttrAsInt("FrameCount", ref this.FrameCount);
            reader.ReadAttrAsInt("CurrentFrame", ref this.CurrentFrame);

            base.ReadXml(reader);
        }
	}
}

