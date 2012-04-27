using System;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace Lunohod.Objects
{
    [XmlRoot("ScoreFile")]
	public class XScoreFile : XObject
	{
		[XmlArray("LevelScores")]
		public List<XLevelScore> LevelScores;
		
		public override void WriteXml(XmlWriter writer)
		{
			foreach(var score in this.LevelScores)
			{
				score.WriteXml(writer);
			}
		}
		
		public override void AddSubcomponent(string name, XObject sub)
        {
			if (this.LevelScores == null)
				this.LevelScores = new List<XLevelScore>();
			
			this.LevelScores.Add((XLevelScore)sub);
        }
	}
}

