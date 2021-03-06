using System;
using System.Xml.Serialization;
using Lunohod.Objects;


namespace Lunohod
{
	public abstract class XAudibleBase : XObject
	{
		private double volume;

		[XmlAttribute]
        public string FileId;

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

		[XmlAttribute]
		public bool IsLooped
		{
			get; set;
		}

		protected void AdjustVolume()
		{
			AdjustVolumeImpl(GameEngine.Instance.IsMute ? 0 : this.volume);
		}

		protected abstract void AdjustVolumeImpl(double volume);

		protected abstract void AdjustIsLooped();

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			GameEngine.Instance.MuteChanged += HandleMuteChanged;
		}

		void HandleMuteChanged(object sender, EventArgs e)
		{
			AdjustVolume();
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.FileId = reader["FileId"];
            this.Volume = reader.ReadAttrAsFloat("Volume");
			this.IsLooped = reader.ReadAttrAsBoolean("IsLooped");

            base.ReadXml(reader);
        }

		public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
		{
			switch (propertyName)
			{
                case "Volume": getter = () => this.Volume; setter = (v) => this.Volume = v; return;
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}

		public override void GetProperty(string propertyName, out Func<bool> getter, out Action<bool> setter)
		{
			switch (propertyName)
			{
				case "IsLooped": getter = () => this.IsLooped; setter = (v) => this.IsLooped = v; return;
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}

		public override void Dispose()
		{
			GameEngine.Instance.MuteChanged -= HandleMuteChanged;
		}
	}
}

