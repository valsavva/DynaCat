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
    [XmlType("Include")]
	public class XInclude : XElement
	{
		private string file;
		
		[XmlAttribute]
		public string File
		{
			get { return this.file; }
			set { 
				this.file = value;
				
				this.Subcomponents = new XObjectCollection
				{
					GameEngine.LoadMetadata(this.file, typeof(XInclude))
				};
			}
		}
		
		public XInclude()
		{
		}
		
		public override void InitHierarchy()
		{
			base.InitHierarchy();
		}
	}
}

