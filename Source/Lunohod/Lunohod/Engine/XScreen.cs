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
        private Dictionary<string, float> numVariables;
        private Dictionary<string, bool> boolVariables;
        private Dictionary<string, string> strVariables;

        [XmlAttribute]
        public string Name;
		
		[XmlAttribute]
		public string File;

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Name = reader["Name"];
            this.File = reader["File"];

            base.ReadXml(reader);
        }
    }
}
