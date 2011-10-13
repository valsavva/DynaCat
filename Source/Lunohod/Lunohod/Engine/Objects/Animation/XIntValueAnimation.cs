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
	[XmlType("IntValueAnimation")]
	public class XIntValueAnimation : XAnimationBase
	{
		private NumericValueTimeline timeline;
		private PropertyAccessor<int> propertyAccessor;
		private TimeSpan elapsedTime;
			
		public XIntValueAnimation()
		{
		}
		
		[XmlAttribute]
		public int From;
		[XmlAttribute]
		public int To;
		
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
			
			propertyAccessor = (PropertyAccessor<int>)PropertyAccessorBase.CreatePropertyAccessor(
				target, this.TargetProperty
			);
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			this.elapsedTime += p.GameTime.ElapsedGameTime;
			
			var newPropertyValue = timeline.GetIntValue(this.elapsedTime);
			
			propertyAccessor.PropertyValue = newPropertyValue;
		}
		
	}
}

