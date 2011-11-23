using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace Lunohod.Objects
{
	[XmlType("MusicFile")]	
	public class XMusicResource : XResource
	{
		private Song song;
		
		public XMusicResource()
		{
		}

		[XmlAttribute]
        public string Source;
		
		public Song Song
		{
			get { return this.song; }
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			XResourceBundle r = (XResourceBundle)this.Parent;
			
			string fileName = Path.Combine(r.RootFolder.Replace('/', Path.DirectorySeparatorChar), this.Source);
		
			this.song = p.Game.Content.Load<Song>(fileName);
		}
	}
}

