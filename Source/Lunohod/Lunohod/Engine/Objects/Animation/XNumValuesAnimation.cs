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
    public class XNumValueAnimation : XAnimationBase
    {
        private NumericValueTimeline timeline;
        private PropertyAccessor propertyAccessor;

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

            propertyAccessor = PropertyAccessor.CreatePropertyAccessor(
                target, this.TargetProperty
            );
        }

        public override void UpdateAnimation(UpdateParameters p)
        {
            var newPropertyValue = (float)timeline.GetValue(this.elapsedTime);
            propertyAccessor.PropertyValue = newPropertyValue;
        }
    }
}

