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
		private Point c1;
		private Point c2;
		private float squaredDistance;
		private float squaredValue;
		
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
			squaredValue = this.Value * this.Value;
		}
		
		private XElement FindObject(string id)
		{
			var o = (XElement)this.GetRoot().FindDescendant(id);
			if (o == null)
				throw new InvalidOperationException("DistanceTrigger could not find obect id: " + id);
			return o;
		}
		
		public override float GetNewValue()
		{
			c1 = object1.GetScreenBounds().Center;
			c2 = object2.GetScreenBounds().Center;
			
			squaredDistance = c1.SquaredDistanceTo(c2);
			
			return squaredDistance;
		}
		
		public override bool IsTriggered()
		{
			var v = GetNewValue();
			return compareFunc(v, squaredValue);
		}
	}
}

