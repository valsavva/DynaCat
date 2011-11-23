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
    [XmlType("Sound")]
	public class XSound : XObject
	{
		private XSoundResource soundFile;
		
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
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.soundFile = (XSoundResource)p.ScreenEngine.RootComponent.FindDescendant(this.FileId);
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
		}
		
		public void Play()
		{
			this.soundFile.SoundEffect.Play(this.Volume, this.Pitch, this.Pan);
		}
	}
}

