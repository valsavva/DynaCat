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
    [XmlType("Screen")]
    public class XScreen : XElement
    {
        private Dictionary<string, float> numVariables;
        private Dictionary<string, bool> boolVariables;
        private Dictionary<string, string> strVariables;

        [XmlIgnore]
        public Dictionary<string, float> NumVariables
            { get { return numVariables ?? (numVariables = new Dictionary<string, float>()); } }
        [XmlIgnore]
        public Dictionary<string, bool> BoolVariables
            { get { return boolVariables ?? (boolVariables = new Dictionary<string, bool>()); } }
        [XmlIgnore]
        public Dictionary<string, string> StrVariables
            { get { return strVariables ?? (strVariables = new Dictionary<string, string>()); } }

        [XmlAttribute]
        public string Name;
		
		[XmlAttribute]
		public string File;
    }
}
