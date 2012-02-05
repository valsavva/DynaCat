using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Lunohod.Objects
{
    [XmlType("Class")]
    public class XClass : XElement
    {
		private XObject templateObject;
		
		public override void InitHierarchy()
		{
			this.templateObject = this.Subcomponents.Find(o => !(o is XResourceBundle));
			this.Subcomponents.Remove(templateObject);
			
			base.InitHierarchy();
		}
		
		public XObject CreateInstance(XObject placeholder)
		{
			var instance = templateObject.Copy();
			CopyAttributes(placeholder, instance);
			
			// move subcomponents from the placeholder to the new instance
			while(placeholder.Subcomponents.Count > 0)
			{
				var subcomponent = placeholder.Subcomponents[0];
				placeholder.Subcomponents.RemoveAt(0);
				
				instance.Subcomponents.Add(subcomponent);
			}

			var parent = placeholder.Parent;
			
			// replace the placeholder with the new instance
			int instanceIndex = parent.Subcomponents.IndexOf(placeholder);
			parent.Subcomponents.RemoveAt(instanceIndex);
			parent.Subcomponents.Insert(instanceIndex, instance);
			
			ReplaceThisKeyword(instance);
			
			return instance;
		}

		public void CopyAttributes(XObject src, XObject dest)
		{
			dest.Id = src.Id;
			//dest.Class = src.Class;
			
			if (src is XElement)
			{
				((XElement)dest).Bounds = ((XElement)src).Bounds;
			}
		}

		public void ReplaceThisKeyword(XObject instance)
		{
			if (instance.Subcomponents != null)
				for(int i = 0; i < instance.Subcomponents.Count; i++)
					instance.ReplaceThis(instance.Id);
		}
	}
}

