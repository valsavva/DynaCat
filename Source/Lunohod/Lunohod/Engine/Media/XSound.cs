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
		internal SoundEffectInstance soundEffectInstance;

		[XmlAttribute]
		public double Pitch = 0;
		
		[XmlAttribute]
		public double Pan = 0;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			PerfMon.Start("Other-Sfx");

			this.soundFile = (XSoundResource)this.FindGlobal(this.FileId);

			if (this.soundFile == null)
				throw new InvalidOperationException(string.Format("Sound effect resource with id '{0}' was not found", this.FileId));

			this.soundFile.CheckOutInstance(this);
		
			PerfMon.Stop("Other-Sfx");
        }
		
		protected override void AdjustVolumeImpl(double volume)
		{
			if (this.soundFile != null && this.soundFile.VerifyInstance(this))
				this.soundEffectInstance.Volume = (float)volume;
		}

        public bool InProgress
        {
            get { return this.soundFile.VerifyInstance(this) && this.soundEffectInstance.State != SoundState.Stopped; }
            set
            {
                // noop for now
            }
        }

        public bool IsPaused
        {
            get { return this.soundFile.VerifyInstance(this) && this.soundEffectInstance.State == SoundState.Paused; }
        }

        public void Start()
		{
			if (!this.soundFile.CheckOutInstance(this))
				return;

			if (this.soundEffectInstance.State != SoundState.Stopped)
				this.soundEffectInstance.Stop();

			AdjustVolume();
#if WINDOWS
            this.soundEffectInstance.Pitch = (float)this.Pitch;
#else
			// mapping XNA Pitch values to MonoGame (OpenAL)
			// http://monogame.codeplex.com/discussions/324813
            this.soundEffectInstance.Pitch = 1f + (float)this.Pitch * 0.5f;
#endif
            this.soundEffectInstance.Pan = (float)this.Pan;
			this.soundEffectInstance.Play();
		}
		public void Play()
		{
			if (!this.soundFile.VerifyInstance(this))
				return;

			if (this.soundEffectInstance.State == SoundState.Stopped)
				Start();
			else if (this.soundEffectInstance.State == SoundState.Paused)
				Resume();
		}
		public void Stop()
		{
			if (!this.soundFile.VerifyInstance(this))
				return;

			this.soundEffectInstance.Stop();
		}
		public void Pause()
		{
			if (!this.soundFile.VerifyInstance(this))
				return;

			if (this.soundEffectInstance.State == SoundState.Playing)
				this.soundEffectInstance.Pause();
		}
		public void Resume()
		{
			if (!this.soundFile.VerifyInstance(this))
				return;

			if (this.soundEffectInstance.State == SoundState.Paused)
				this.soundEffectInstance.Resume();
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsFloat("Pitch", ref this.Pitch);
            reader.ReadAttrAsFloat("Pan", ref this.Pan);
            
            base.ReadXml(reader);
        }
	}
}

