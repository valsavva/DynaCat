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
	public class XMusic : XObject, IHasVolume, IRunnable
	{
		private XMusicResource musicFile;
		private float volume;
		
		public XMusic()
		{
		}
		
        [XmlAttribute]
        public string FileId;
		
		[XmlAttribute]
		public float Volume
		{
			get { return this.volume; }
			set { 
				this.volume = value;
				MediaPlayer.Volume = this.volume;
			}
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
				Start();
		}
		
        public bool InProgress
        {
            get { return MediaPlayer.State == MediaState.Playing; }
            set
            {
                // noop for now
            }
        }

        public bool IsPaused
        {
            get { return MediaPlayer.State == MediaState.Paused; }
        }

        public void Start()
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(this.musicFile.Song);
				MediaPlayer.Volume = this.Volume;

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

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.FileId = reader["FileId"];
            this.Volume = reader.ReadAttrAsFloat("Volume");

            base.ReadXml(reader);
        }
	}
}

