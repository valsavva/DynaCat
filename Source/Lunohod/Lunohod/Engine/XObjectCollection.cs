using System;
using Lunohod.Objects;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
	public class XObjectCollection : List<XObject>
	{
		[XmlIgnore]
		internal XObject Parent { get; set; }
		
		public XObjectCollection()
		{
		}
		
		public XObjectCollection(int capacity)
			: base(capacity)
		{
		}
		
		public XObjectCollection(IEnumerable<XObject> collection)
			: base(collection)
		{
		}
		
		new public void Insert(int index, XObject item)
		{
			base.Insert(index, item);
			
			item.Parent = this.Parent;
		}
		
		new public void Add(XObject item)
		{
			base.Add(item);
			
			item.Parent = this.Parent;
		}
		
		new public void AddRange(IEnumerable<XObject> collection)
		{
			base.AddRange(collection);
			
			foreach(var o in collection)
				o.Parent = this.Parent;
		}
		
		new public void InsertRange(int index, IEnumerable<XObject> collection)
		{
			base.InsertRange(index, collection);
			
			foreach(var o in collection)
				o.Parent = this.Parent;
		}
		
		public void InsertBefore(XObject item, XObject newItem)
		{
			int index = this.IndexOf(item);
			this.Insert(index, newItem);
		}
		
	    new public bool Remove(XObject item)
		{
			item.Parent = null;

			return base.Remove(item);
		}
		
		new public void RemoveAt(int index)
		{
			this[index].Parent = null;
			base.RemoveAt(index);
		}
	}
}

