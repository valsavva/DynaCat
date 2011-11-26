using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Lunohod.Objects
{
    [XmlType("Music")]
	public class XMusic : XObject
	{
		private XMusicResource musicFile;
		
		public XMusic()
		{
		}
		
        [XmlAttribute]
        public string FileId;
		
		[XmlAttribute]
		public float Volume
		{
			get { return MediaPlayer.Volume; }
			set { MediaPlayer.Volume = value; }
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.musicFile = (XMusicResource)p.ScreenEngine.RootComponent.FindDescendant(this.FileId);
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			if (MediaPlayer.State == MediaState.Stopped)
				Play();
		}
		
        public void Play()
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(this.musicFile.Song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }
		public void Pause()
		{
			MediaPlayer.Pause();
		}
		public void Resume()
		{
			MediaPlayer.Resume();
		}
		public void Stop()
		{
			MediaPlayer.Stop();
		}
	}
}

