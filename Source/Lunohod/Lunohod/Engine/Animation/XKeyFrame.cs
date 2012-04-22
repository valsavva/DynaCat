using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;
using Lunohod.Xge;

namespace Lunohod.Objects
{
    [XmlType("KeyFrame")]
    public class XKeyFrame : XObject
    {
        private List<IExpression<double>> valueReaders;
        private IExpression<double> timeReader;
        private XNumAnimation animation;

        [XmlAttribute]
        public string Time;

        [XmlAttribute]
        public string Value;

        [XmlAttribute]
        public CurveTangent Smoothing = CurveTangent.Linear;

        [XmlIgnore]
        public List<CurveKey> CurveKeys { get; private set; }

		[XmlIgnore]
		public double CurrentTime { get; private set; }
		
        public override void Initialize(InitializeParameters p)
        {
            base.Initialize(p);

            animation = (XNumAnimation)this.Parent;

            this.valueReaders = this.Value.Split(',').Select(s => Compiler.CompileExpression<double>(this.Parent, s)).ToList();
            this.timeReader = Compiler.CompileExpression<double>(this.Parent, this.Time);
			this.CurrentTime = 0f;
            this.CurveKeys = valueReaders.Select(
				r => new CurveKey(0f, 0f)
             ).ToList();
        }

        public override void Update(UpdateParameters p)
        {
            base.Update(p);

            if (!animation.InProgress || animation.IsPaused)
                return;
			
			double newTime = timeReader.GetValue();
			
			if (this.CurrentTime != newTime)
			{
				this.CurrentTime = newTime;
				
				for(int i = 0; i < animation.curves.Count; i++)
				{
					animation.curves[i].Keys.Remove(this.CurveKeys[i]);
                    this.CurveKeys[i] = new CurveKey((float)this.CurrentTime, (float)this.valueReaders[i].GetValue());
					animation.curves[i].Keys.Add(this.CurveKeys[i]);
				}
			} 
			else
			{
		        for (int i = 0; i < this.valueReaders.Count; i++)
		        {
		            this.CurveKeys[i].Value = (float)this.valueReaders[i].GetValue();
		        }
			}
        }
		
		internal override void ReplaceParameter(string par, string val)
		{
			this.Value = this.Value.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Time = reader["Time"];
            this.Value = reader["Value"];
            reader.ReadAttrAsEnum<CurveTangent>("Smoothing", ref this.Smoothing);

            base.ReadXml(reader);
        }
    }
}
