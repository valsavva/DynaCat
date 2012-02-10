using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;

namespace Lunohod.Objects
{
    [XmlType("KeyFrame")]
    public class XKeyFrame : XObject
    {
        private List<NumValueReader> valueReaders;

        [XmlIgnore]
        public TimeSpan Time;

        [XmlAttribute]
        public string Value;

        [XmlAttribute]
        public CurveTangent Smoothing = CurveTangent.Linear;

        [XmlIgnore]
        public List<CurveKey> CurveKeys { get; private set; }

        [XmlAttribute("Time")]
        public string zTime
        {
            get { return this.Time.ToString(); }
            set { this.Time = value.ToDuration(); }
        }

        public override void Initialize(InitializeParameters p)
        {
            base.Initialize(p);

            this.valueReaders = this.Value.Split(',').Select(s => new NumValueReader(this.Parent, s)).ToList();
            this.CurveKeys = valueReaders.Select(r => new CurveKey((float)this.Time.TotalMilliseconds, r.Value)).ToList();
        }

        public override void Update(UpdateParameters p)
        {
            base.Update(p);

            for (int i = 0; i < this.valueReaders.Count; i++)
            {
                this.CurveKeys[i].Value = this.valueReaders[i].Value;
            }
        }
		
		public override void ReplaceParameter(string par, string val)
		{
			this.Value = this.Value.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}
    }
}
