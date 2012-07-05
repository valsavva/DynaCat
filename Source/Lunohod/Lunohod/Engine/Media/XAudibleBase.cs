using System;
using System.Xml.Serialization;
using Lunohod.Objects;


namespace Lunohod
{
	public abstract class XAudibleBase : XObject, IHasVolume
	{
		private double volume;

		[XmlAttribute]
		public double Volume
		{
			get { return this.volume; }
			set
			{ 
				this.volume = value;
				AdjustVolume();
			}
		}

		protected void AdjustVolume()
		{
			AdjustVolumeImpl(GameEngine.Instance.IsMute ? 0 : volume);
		}

		protected abstract void AdjustVolumeImpl(double volume);

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			GameEngine.Instance.MuteChanged += HandleMuteChanged;
		}

		void HandleMuteChanged (object sender, EventArgs e)
		{
			AdjustVolume();
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Volume = reader.ReadAttrAsFloat("Volume");

            base.ReadXml(reader);
        }

		public override void Dispose()
		{
			GameEngine.Instance.MuteChanged -= HandleMuteChanged;
		}
	}
}

