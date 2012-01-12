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
    [XmlType("IntersectionTrigger")]
    public class XIntersectionTrigger : XNumTriggerBase
    {
        private XElement object1;
        private XElement object2;
        private Rectangle rect1;
        private Rectangle rect2;
        private Rectangle rectInt;

        [XmlAttribute]
        public string ObjectId1;
        [XmlAttribute]
        public string ObjectId2;

        public XIntersectionTrigger()
        {
            this.Compare = XValueComparison.G;
        }


        public override void Initialize(InitializeParameters p)
        {
            base.Initialize(p);

            object1 = FindObject(ObjectId1);
            object2 = FindObject(ObjectId2);
        }

        private XElement FindObject(string id)
        {
            var o = (XElement)this.GetRoot().FindDescendant(id);
            if (o == null)
                throw new InvalidOperationException("IntersectionTrigger could not find obect id: " + id);
            return o;
        }

        public override float GetNewValue()
        {
            rect1 = object1.GetScreenBounds();
            rect2 = object2.GetScreenBounds();

            Rectangle.Intersect(ref rect1, ref rect2, out rectInt);

            return rectInt.Area();
        }
    }
}
