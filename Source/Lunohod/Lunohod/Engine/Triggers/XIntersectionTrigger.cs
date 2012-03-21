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
        private ObjectProxy object1;
        private ObjectProxy object2;
        private System.Drawing.RectangleF rect1;
        private System.Drawing.RectangleF rect2;
        private System.Drawing.RectangleF rectInt;

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

            object1 = new ObjectProxy(FindObject(ObjectId1));
            object2 = new ObjectProxy(FindObject(ObjectId2));
        }

        private XObject FindObject(string id)
        {
            var o = (XObject)this.GetRoot().FindDescendant(id);
            if (o == null)
                throw new InvalidOperationException("IntersectionTrigger could not find obect id: " + id);
            return o;
        }

        public override float GetValue1()
        {
            rect1 = object1.Element.GetScreenBounds();
            rect2 = object2.Element.GetScreenBounds();

            Utility.Intersect(ref rect1, ref rect2, out rectInt);

            return rectInt.Area();
        }

		internal override void ReplaceParameter(string par, string val)
		{
            this.ObjectId1 = this.ObjectId1.Replace(par, val);
            this.ObjectId2 = this.ObjectId2.Replace(par, val);

			base.ReplaceParameter(par, val);
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.ObjectId1 = reader["ObjectId1"];
            this.ObjectId2 = reader["ObjectId2"];
            
            base.ReadXml(reader);
        }
	}
}
