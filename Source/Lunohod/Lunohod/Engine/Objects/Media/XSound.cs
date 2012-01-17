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
using Microsoft.Xna.Framework.Audio;

namespace Lunohod.Objects
{
    [XmlType("Sound")]
	public class XSound : XObject, IHasVolume, IRunnable
	{
		private XSoundResource soundFile;
		private SoundEffectInstance soundEffectInstance;
		
		private float volume;
		
		public XSound()
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
				if (this.soundEffectInstance != null)
					this.soundEffectInstance.Volume = this.volume;
			}
		}
		
		[XmlAttribute]
		public float Pitch = 0;
		
		[XmlAttribute]
		public float Pan = 0;
		
		[XmlAttribute]
		public bool IsLooped;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.soundFile = (XSoundResource)p.ScreenEngine.RootComponent.FindDescendant(this.FileId);
			this.soundEffectInstance = this.soundFile.SoundEffect.CreateInstance();

            // this can be set only once
            this.soundEffectInstance.IsLooped = this.IsLooped;
        }
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
		}

        public bool InProgress
        {
            get { return this.soundEffectInstance.State == SoundState.Playing; }
            set
            {
                // noop for now
            }
        }
        public void Start()
		{
			this.soundEffectInstance.Volume = this.Volume;
			this.soundEffectInstance.Pitch = this.Pitch;
			this.soundEffectInstance.Pan = this.Pan;
			this.soundEffectInstance.Play();
		}
		public void Stop()
		{
			this.soundEffectInstance.Stop();
		}
		public void Pause()
		{
			this.soundEffectInstance.Pause();
		}
		public void Resume()
		{
			this.soundEffectInstance.Resume();
		}
	}
}

