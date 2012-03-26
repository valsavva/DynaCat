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
    [XmlType("DistanceTrigger")]
	public class XDistanceTrigger : XNumTriggerBase
	{
		private ObjectProxy object1;
		private ObjectProxy object2;
		private Vector2 c1;
		private Vector2 c2;
		
		[XmlAttribute]
		public string ObjectId1;
		[XmlAttribute]
		public string ObjectId2;

		public XDistanceTrigger()
		{
			// default comparison for distances to LE
			// (by default it triggers when objects get closer to each other than the given treshold)
			this.Compare = XValueComparison.LE;
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
				throw new InvalidOperationException("DistanceTrigger could not find obect id: " + id);
			return o;
		}
		
		public override double GetValue1()
		{
			c1 = object1.Element.GetScreenBounds().Center();
			c2 = object2.Element.GetScreenBounds().Center();
			
			return c1.SquaredDistanceTo(c2);
		}
		
		public override double GetValue2()
		{
			var value2 = value2Reader.GetValue();
			
			return value2 * value2;
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

