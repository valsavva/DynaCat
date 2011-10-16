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
	[XmlType("FloatValueAnimation")]
	public class XFloatValueAnimation : XAnimationBase
	{
		private NumericValueTimeline timeline;
		private PropertyAccessor<float> propertyAccessor;
		private TimeSpan elapsedTime;
		
		[XmlAttribute]
		public float From;
		[XmlAttribute]
		public float To;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			timeline = new NumericValueTimeline();
			timeline.Duration = this.Duration;
			timeline.From = this.From;
			timeline.To = this.To;
			timeline.Autoreverse = this.Autoreverse;
			timeline.BuildTimeline();
			
			XElement target = (XElement)p.ScreenEngine.RootComponent.FindDescendant(this.TargetId);
			
			propertyAccessor = (PropertyAccessor<float>)PropertyAccessorBase.CreatePropertyAccessor(
				target, this.TargetProperty
			);
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			this.elapsedTime += p.GameTime.ElapsedGameTime;
			
			var newPropertyValue = (float)timeline.GetValue(this.elapsedTime);
			
			propertyAccessor.PropertyValue = newPropertyValue;
		}
		
	}
}

