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
		[XmlIgnore]
		public XObject TemplateObject { get; private set; }
		
		public override void InitHierarchy()
		{
			this.TemplateObject = this.Subcomponents.Find(o => !(o is XResourceBundle));
			this.Subcomponents.Remove(TemplateObject);
			
			base.InitHierarchy();
		}
		
		public XObject CreateInstance(XObject placeholder)
		{
			var instance = TemplateObject.Copy();
			CopyAttributes(placeholder, instance);
			
			// move subcomponents from the placeholder to the new instance
			while(placeholder.Subcomponents != null && placeholder.Subcomponents.Count > 0)
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
			
			ReplaceParameters(instance, placeholder.ClassParams);
			
			return instance;
		}

		public void CopyAttributes(XObject src, XObject dest)
		{
			dest.Id = src.Id;
			//dest.Class = src.Class;
			
			dest.Enabled = src.Enabled;
			
			var srcElement = src as XElement;
			if (srcElement != null)
			{
				var destElement = (XElement)dest;
				
				destElement.Bounds = srcElement.Bounds;
				
				if (srcElement.backColor.HasValue)
					destElement.BackColor = srcElement.BackColor;
			}
		}

		public void ReplaceParameters(XObject instance, string classParams)
		{
			List<string> pars = new List<string> { "this" };
			List<string> vals = new List<string> { instance.Id };
			
			if (classParams != null)
			{
				classParams.Split(',').ForEach(p => {
					var ss = p.Split('=');
					
					pars.Add(ss[0].Trim());
					vals.Add(ss[1].Trim());
				});
			}
			
			instance.ReplaceParameters(pars, vals);
		}
	}
}

