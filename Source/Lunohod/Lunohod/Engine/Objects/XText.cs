using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Lunohod.Objects
{
    [XmlType("Text")]
	public class XText : XElement
	{
		private XFontResource font;
		private Vector2 location;
		private StrValueReader strValueReader;

        [XmlIgnore]
        public Color ForeColor;
		[XmlAttribute]
		public string Text;
		[XmlAttribute]
		public string FontId;

        [XmlAttribute("Color")]
        public string zColor
		{
			set { this.ForeColor = value.ToColor(); }
			get { return this.ForeColor.ToStr(); }
		}
		
		public XText()
		{
			this.ForeColor = Color.White;
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.font = (XFontResource)p.ScreenEngine.RootComponent.FindDescendant(this.FontId);
			this.strValueReader = new StrValueReader(this, this.Text);
			
			if (this.font == null)
				throw new InvalidOperationException(string.Format(
					"Font was not found. Text ID:{0} FontId:{1}", this.Id, this.FontId
                ));
		}
		
        private float screenRotation;

        public override void Update(UpdateParameters p)
        {
            base.Update(p);

            this.GetScreenBounds();
			screenRotation = MathHelper.ToRadians(this.PropState.Rotation);
        }

		public override void Draw(DrawParameters p)
		{
			this.location.X = this.PropState.ScreenBounds.Value.X;
			this.location.Y = this.PropState.ScreenBounds.Value.Y;
			
			if (screenRotation != 0 || this.Origin != Vector2.Zero || this.PropState.Scale != Vector2.One)
				p.SpriteBatch.DrawString(this.font.Font, this.strValueReader.Value, this.location, this.ForeColor, screenRotation, this.Origin, this.PropState.Scale, SpriteEffects.None, 0);
			else
				p.SpriteBatch.DrawString(this.font.Font, this.strValueReader.Value, this.location, this.ForeColor);
			
			base.Draw(p);
		}
		
		public override void ReplaceParameter(string par, string val)
		{
			if (this.Text != null && this.Text.StartsWith("="))
				this.Text = this.Text.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}
	}
}

