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
		private XElement object1;
		private XElement object2;
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
			
			object1 = FindObject(ObjectId1);
			object2 = FindObject(ObjectId2);
		}
		
		private XElement FindObject(string id)
		{
			var o = (XElement)this.GetRoot().FindDescendant(id);
			if (o == null)
				throw new InvalidOperationException("DistanceTrigger could not find obect id: " + id);
			return o;
		}
		
		public override float GetValue1()
		{
			c1 = object1.GetScreenBounds().Center();
			c2 = object2.GetScreenBounds().Center();
			
			return c1.SquaredDistanceTo(c2);
		}
		
		public override float GetValue2()
		{
			var value2 = value2Reader.Value;
			
			return value2 * value2;
		}
		
		public override void ReplaceThis(string iid)
		{
            this.ObjectId1 = this.ObjectId1.Replace("this", iid);
            this.ObjectId2 = this.ObjectId2.Replace("this", iid);

			base.ReplaceThis(iid);
		}
	}
}

