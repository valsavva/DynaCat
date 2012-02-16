using System;
using Lunohod.Objects;

namespace Lunohod.Objects
{
	public class ObjectProxy
	{
		private XObject obj;
		private XElement ele;
		private XIterator itr;
		
		private Func<XObject> getObject;
		private Func<XElement> getElement;
		private Func<Type> getType;
		
		public ObjectProxy(XObject o)
		{
			if (o is XIterator)
			{
				itr = o as XIterator;
				getObject = this.GetIteratorObject;
				getElement = this.GetIteratorElement;
				getType = this.GetIteratorType;
			}
			else
			{
				obj = o;
				ele = o as XElement;
				getObject = this.GetInternalObject;
				getElement = this.GetInternalElement;
				getType = this.GetInternalType;
			}
		}
		
		private XObject GetInternalObject()
		{
			return obj;
		}
		
		private XElement GetInternalElement()
		{
			return ele;
		}
		
		private XObject GetIteratorObject()
		{
			return itr.Current;
		}
		
		private XElement GetIteratorElement()
		{
			return itr.Current as XElement;
		}
		
		private Type GetInternalType()
		{
			return obj.GetType();
		}
		
		private Type GetIteratorType()
		{
			return itr.TypeFilterType;
		}
		
		public string ObjectId
		{
			get { return obj == null ? itr.Id : obj.Id; }
		}
		
		public XObject Object
		{
			get { return getObject(); }
		}
		
		public XElement Element
		{
			get { return getElement(); }
		}
		
		public Type ObjectType
		{
			get { return getType(); }
		}
	}
}

