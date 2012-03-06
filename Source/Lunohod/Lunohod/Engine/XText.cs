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
using Lunohod.Xge;

namespace Lunohod.Objects
{
    [XmlType("Text")]
	public class XText : XElement
	{
		private XFontResource font;
		private Vector2 location;
		private IStrExpression strValueReader;

        [XmlIgnore]
        public Color Color;
		[XmlAttribute]
		public string Text;
		[XmlAttribute]
		public string FontId;

        [XmlAttribute("Color")]
        public string zColor
		{
			set { this.Color = value.ToColor(); }
			get { return this.Color.ToStr(); }
		}
		
		public XText()
		{
			this.Color = Color.White;
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.font = (XFontResource)p.ScreenEngine.RootComponent.FindDescendant(this.FontId);

            if (this.Text != null && this.Text.StartsWith("="))
            {
                this.strValueReader = Compiler.CompileStrExpression(this, "system.Str(" + this.Text.Substring(1) + ")");
            }
			
			if (this.font == null)
				throw new InvalidOperationException(string.Format(
					"Font was not found. Text ID:{0} FontId:{1}", this.Id, this.FontId
                ));
		}
		
        private float screenRotation;
		private Color actualColor;

        public override void Update(UpdateParameters p)
        {
            base.Update(p);

            this.GetScreenBounds();
            actualColor = this.Color * this.PropState.Opacity;
			screenRotation = MathHelper.ToRadians(this.PropState.Rotation);
        }

        private string GetText()
        {
            return this.strValueReader == null ? this.Text : this.strValueReader.GetValue();
        }

		public override void Draw(DrawParameters p)
		{
			if (this.PropState.ScreenBounds.HasValue)
			{
				this.location.X = this.PropState.ScreenBounds.Value.X;
				this.location.Y = this.PropState.ScreenBounds.Value.Y;
				
				if (screenRotation != 0 || this.Origin != Vector2.Zero || this.PropState.Scale != Vector2.One)
					p.SpriteBatch.DrawString(this.font.Font, GetText(), this.location, actualColor, screenRotation, this.Origin, this.PropState.Scale, SpriteEffects.None, 0);
				else
                    p.SpriteBatch.DrawString(this.font.Font, GetText(), this.location, actualColor);
			}
			
			base.Draw(p);
		}
		
		internal override void ReplaceParameter(string par, string val)
		{
			if (this.Text != null)
				this.Text = this.Text.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}
	}
}

