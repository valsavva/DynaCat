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
using Microsoft.Xna.Framework.Audio;

namespace Lunohod.Objects
{
	[XmlType("SoundFile")]	
	public class XSoundResource : XResource
	{
		private SoundEffect soundEffect;
		
		public XSoundResource()
		{
		}

		public SoundEffect SoundEffect
		{
			get { return this.soundEffect; }
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

            this.soundEffect = LoadResource<SoundEffect>(p.Game.Content, "SoundEffectProcessor", "wav", "xnb");
		}
	}
}

