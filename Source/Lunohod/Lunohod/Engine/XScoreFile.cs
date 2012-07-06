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
		[XmlIgnore]
		public Dictionary<string, XLevelScore> LevelScoreDict = new Dictionary<string, XLevelScore>();


		public void GenerateScoreDict()
		{
			this.LevelScoreDict.Clear();
			this.LevelScores.ForEach(s => this.LevelScoreDict[s.Id] = s);
		}

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

