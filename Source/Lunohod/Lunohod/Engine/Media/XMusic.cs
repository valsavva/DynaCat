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
	public class XMusic : XAudibleBase, IRunnable
	{
		private XMusicResource musicFile;

		public XMusic()
		{
		}
		
        [XmlAttribute]
        public string FileId;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.musicFile = (XMusicResource)this.FindGlobal(this.FileId);
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			if (MediaPlayer.State == MediaState.Stopped)
				Start();
		}

		protected override void AdjustVolumeImpl(double volume)
		{
			MediaPlayer.Volume = (float)volume;
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
				AdjustVolume();
                MediaPlayer.Play(this.musicFile.Song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }
		public void Play()
		{
			if (MediaPlayer.State == MediaState.Stopped)
				Start();
			else if (MediaPlayer.State == MediaState.Paused)
				Resume();
		}
		public void Pause()
		{
			if (MediaPlayer.State == MediaState.Playing)
				MediaPlayer.Pause();
		}
		public void Resume()
		{
			if (MediaPlayer.State == MediaState.Paused)
				MediaPlayer.Resume();
		}
		public void Stop()
		{
			MediaPlayer.Stop();
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Volume = reader.ReadAttrAsFloat("Volume");

            base.ReadXml(reader);
        }
	}
}

