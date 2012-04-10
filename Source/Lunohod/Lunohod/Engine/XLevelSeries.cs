using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Lunohod.Objects
{
    [XmlType("LevelSeries")]
	public class XLevelSeries : XLevelInfo
	{
		[XmlArray]
		public List<XLevelInfo> Levels;
		
        public override void AddSubcomponent(string name, XObject sub)
        {
			if (this.Levels == null)
				this.Levels = new List<XLevelInfo>();
            
			this.Levels.Add((XLevelInfo)sub);
		}
	}
}

