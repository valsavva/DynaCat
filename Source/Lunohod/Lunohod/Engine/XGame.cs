using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunohod;
using System.Xml;

namespace Lunohod.Objects
{
    /// <summary>
    /// Describes the structure of the game.
    /// It includes collection of screens and levels that participate in the game.
    /// </summary>
    [XmlRoot("Game")]
	public class XGame : XScreen
	{
        /// <summary>
        /// Specifies the name of the XML file that contains the first screen that user sees after the game is loaded into the memory.
        /// </summary>
        [XmlAttribute]
        public string StartScreen;
        /// <summary>
        /// Specifies whether to display Frames Per Second counter.
        /// </summary>
        [XmlAttribute]
        public bool ShowFPS;
        /// <summary>
        /// Specifies whether On Screen Debug Information will be accessible.
        /// </summary>
        [XmlAttribute]
        public bool ShowDebugInfo;
        /// <summary>
        /// Collection of <see cref="XLevel"/> objects that will participate in the game./>
        /// </summary>
		[XmlArray("Levels")]
		public List<XLevelSeries> LevelSeries;
        /// <summary>
        /// Collection of <see cref="XScreen"/> objects that will participate in the game./>
        /// </summary>
		[XmlIgnore]
		public List<XLevelInfo> Levels;

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.StartScreen = reader["StartScreen"];
            this.ShowFPS = reader.ReadAttrAsBoolean("ShowFPS");
            this.ShowDebugInfo = reader.ReadAttrAsBoolean("ShowDebugInfo");

            base.ReadXml(reader);
        }

        public override void AddSubcomponent(string name, XObject sub)
        {
            if (name == "Levels")
			{
                this.LevelSeries = sub.Subcomponents.Cast<XLevelSeries>().ToList();
				this.Levels = (
					from ls in this.LevelSeries
					from l in ls.Levels
					select l
				).ToList();
			}
            else
                base.AddSubcomponent(name, sub);
        }
	}
}

