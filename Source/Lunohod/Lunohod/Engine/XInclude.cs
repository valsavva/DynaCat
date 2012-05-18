using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace Lunohod.Objects
{
    [XmlRoot("Include")]
    [XmlSchemaProvider("MySchema")]
    public class XInclude : XElement
	{
        public static XmlQualifiedName MySchema(XmlSchemaSet xs)
        {
            return new XmlQualifiedName("blah", @"http://www.w3.org/2001/XMLSchema");
        }
        
        private string file;
		
		[XmlAttribute]
		public string File
		{
			get { return this.file; }
			set { 
				this.file = value;

                if (this.file == null)
                    return;
				
				var include = GameEngine.LoadXml<XInclude>(this.file);
				var subComponents = include.Subcomponents;
				include.Subcomponents = null;
				this.Subcomponents = subComponents;
			}
		}
		
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.File = reader["File"];
            
            base.ReadXml(reader);
        }
	}
}

