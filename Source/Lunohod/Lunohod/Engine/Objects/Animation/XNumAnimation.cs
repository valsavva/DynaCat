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
    [XmlType("NumAnimation")]
    public class XNumAnimation : XAnimationBase
    {
        private NumericValueTimeline timeline;
        private PropertyAccessor propertyAccessor;
		
        [XmlAttribute]
        public float From;
        [XmlAttribute]
        public float To;
		[XmlAttribute]
		public bool IsDelta;
		
		private float? startValue;
		
        public override void Initialize(InitializeParameters p)
        {
            base.Initialize(p);

            timeline = new NumericValueTimeline();
            timeline.Duration = this.Duration;
            timeline.From = this.From;
            timeline.To = this.To;
            timeline.Autoreverse = this.Autoreverse;
            timeline.BuildTimeline();

            propertyAccessor = new PropertyAccessor(this, this.Target);
        }
		
		public override void Start()
		{
			base.Start();
			
			startValue = null;
		}

        public override void UpdateAnimation()
        {
            var newPropertyValue = (float)timeline.GetValue(this.elapsedTime);

			if (this.IsDelta)
			{
				if (!startValue.HasValue)
					startValue = propertyAccessor.FloatPropertyValue;
				
				newPropertyValue += startValue.Value;
			}
            
			propertyAccessor.FloatPropertyValue = newPropertyValue;
        }
    }
}

