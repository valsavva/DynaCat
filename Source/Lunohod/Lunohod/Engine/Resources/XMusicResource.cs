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
		
		public Song Song
		{
			get { return this.song; }
		}
		
		public override void InitializeMainThread(InitializeParameters p)
		{
			base.InitializeMainThread(p);

			if (this.song != null)
				return;

            this.song = LoadResource<Song>(p.Content, "SongProcessor", "mp3", "wma", "Mp3Importer");
		}
		public override void Dispose()
		{
			this.song.Dispose();
		}
	}
}

