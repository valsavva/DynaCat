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

namespace Lunohod.Objects
{
    [XmlRoot("Include")]
	public class XInclude : XElement
	{
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

