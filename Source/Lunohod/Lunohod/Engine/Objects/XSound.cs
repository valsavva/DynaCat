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
	public class XSound : XObject
	{
		private XSoundResource soundFile;
		private SoundEffectInstance soundEffectInstance;
		
		public XSound()
		{
		}

		[XmlAttribute]
        public string FileId;
		
		[XmlAttribute]
		public float Volume = 1;
		
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
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
		}
		
		public void Play()
		{
			this.soundEffectInstance.Volume = this.Volume;
			this.soundEffectInstance.Pitch = this.Pitch;
			this.soundEffectInstance.Pan = this.Pan;
			this.soundEffectInstance.IsLooped = this.IsLooped;
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

