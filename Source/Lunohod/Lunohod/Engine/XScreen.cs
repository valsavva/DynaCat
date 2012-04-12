using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunohod;

namespace Lunohod.Objects
{
    [XmlRoot("Screen")]
    public class XScreen : XElement
    {
        [XmlAttribute]
        public bool IsModal;

        public override void Update(UpdateParameters p)
        {
            if (p.Game.ScreenEngine.CurrentEvents.ContainsKey("system:levelLoaded"))
            {
                p.GameTime = new GameTime(
                    p.GameTime.TotalGameTime, TimeSpan.Zero
                );
            }

            base.Update(p);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsBoolean("IsModal", ref this.IsModal);
            
            base.ReadXml(reader);
        }
    }
}
