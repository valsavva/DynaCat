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
    /// <summary>
    /// Represents a higher level object that describes the structure of a game.
    /// It includes collection of screens and levels that participate in the game.
    /// </summary>
	[XmlType("Game")]
	public class XGame : XElement
	{
		public XGame()
		{
		}
		
        /// <summary>
        /// Specifies the name of the XML file that contains the first screen that user sees after the game is loaded into the memory.
        /// </summary>
		[XmlAttribute]
		public string StartScreen;

        /// <summary>
        /// Collection of <see cref="XLevel"/> objects that will participate in the game./>
        /// </summary>
		[XmlArray]
		public XLevel[] Levels;

        /// <summary>
        /// Collection of <see cref="XScreen"/> objects that will participate in the game./>
        /// </summary>
        [XmlArray]
		public XScreen[] Screens;
		
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
	}
}

