using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunohod;
using System.Xml;
using System.Xml.Schema;

namespace Lunohod.Objects
{
    [XmlRoot("Screen")]
    [XmlSchemaProvider("MySchema")]
    public class XScreen : XElement
    {
        public static XmlQualifiedName MySchema(XmlSchemaSet xs)
        {
            return new XmlQualifiedName("blah", @"http://www.w3.org/2001/XMLSchema");
        }

        [XmlAttribute]
        public bool IsModal;
		[XmlIgnore]
		public ScreenEngine ScreenEngine;

#if IPHONE
		private int updates;
#endif

        public override void Update(UpdateParameters p)
        {
            if (p.Game.ScreenEngine.CurrentEvents.ContainsKey("system:levelLoaded"))
            {
                p.GameTime = new GameTime(
                    p.GameTime.TotalGameTime, TimeSpan.Zero
                );

            }

#if IPHONE
			if (updates <= 3)
			{
				if (updates == 3)
					MonoTouch.UIKit.UIApplication.SharedApplication.EndIgnoringInteractionEvents();
				updates++;
			}
#endif

            base.Update(p);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsBoolean("IsModal", ref this.IsModal);
            
            base.ReadXml(reader);
        }
    }
}
