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
		private IExpression<string> strValueReader;

        [XmlIgnore]
        public Color Color;
		[XmlAttribute]
		public string Text;
		[XmlAttribute]
		public string FontId;
		[XmlAttribute]
		public XFlipEffects FlipEffects;

		public XText()
		{
			this.Color = Color.White;
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			
			if (string.IsNullOrEmpty(this.FontId))
				throw new InvalidOperationException("Font is not specified for text field");
			
			this.font = (XFontResource)p.ScreenEngine.RootComponent.FindDescendant(this.FontId);

            if (this.Text != null && this.Text.StartsWith("="))
            {
                this.strValueReader = Compiler.CompileExpression<string>(this, "system.Str(" + this.Text.Substring(1) + ")");
            }
			
			if (this.font == null)
				throw new InvalidOperationException(string.Format(
					"Font was not found. Text ID:{0} FontId:{1}", this.Id, this.FontId
                ));
		}
		
        private double screenRotation;
		private Color actualColor;

        public override void Update(UpdateParameters p)
        {
            base.Update(p);

            this.GetScreenBounds();
            actualColor = this.Color * (float)this.PropState.Opacity;
			screenRotation = MathHelper.ToRadians((float)this.PropState.Rotation);
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
				
				p.SpriteBatch.DrawString(this.font.Font, GetText(), this.location, actualColor, (float)screenRotation, this.Origin, this.PropState.Scale, (SpriteEffects)this.FlipEffects, this.Depth + p.NextSystemImageDepth());
			}
			
			base.Draw(p);
		}
		
		internal override void ReplaceParameter(string par, string val)
		{
			if (this.Text != null)
				this.Text = this.Text.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsColor("Color", ref this.Color);
            this.Text = reader["Text"];
            this.FontId = reader["FontId"];
			reader.ReadAttrAsEnum<XFlipEffects>("FlipEffects", ref this.FlipEffects);

            base.ReadXml(reader);
        }
	}
}

