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
	public class XSound : XAudibleBase, IRunnable
	{
		private XSoundResource soundFile;
		private SoundEffectInstance soundEffectInstance;

		[XmlAttribute]
        public string FileId;

		[XmlAttribute]
		public double Pitch = 0;
		
		[XmlAttribute]
		public double Pan = 0;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			PerfMon.Start("Other-Sfx");

			this.soundFile = (XSoundResource)this.FindGlobal(this.FileId);
			this.soundEffectInstance = this.soundFile.SoundEffect.CreateInstance();
		
			PerfMon.Stop("Other-Sfx");
        }
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
		}

		protected override void AdjustVolumeImpl(double volume)
		{
			if (this.soundEffectInstance != null)
				this.soundEffectInstance.Volume = (float)volume;
		}

        public bool InProgress
        {
            get { return this.soundEffectInstance.State == SoundState.Playing; }
            set
            {
                // noop for now
            }
        }

        public bool IsPaused
        {
            get { return this.soundEffectInstance.State == SoundState.Paused; }
        }

        public void Start()
		{
			AdjustVolume();
#if WINDOWS
            this.soundEffectInstance.Pitch = (float)this.Pitch;
#else
            this.soundEffectInstance.Pitch = 1;
#endif
            this.soundEffectInstance.Pan = (float)this.Pan;
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

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.FileId = reader["FileId"];
            reader.ReadAttrAsFloat("Pitch", ref this.Pitch);
            reader.ReadAttrAsFloat("Pan", ref this.Pan);
            
            base.ReadXml(reader);
        }
		public override void Dispose()
		{
            if (!this.soundEffectInstance.IsDisposed)
            {
                this.soundEffectInstance.Stop();
                this.soundEffectInstance.Dispose();
            }
			
			base.Dispose();
		}
	}
}

