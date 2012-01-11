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

			var parent = placeholder.Parent;

			int instanceIndex = parent.Subcomponents.IndexOf(placeholder);
			parent.Subcomponents[instanceIndex] = instance;
			instance.Parent = parent;
			
			CopyAttributes(placeholder, instance);
			
			ReplaceThisKeyword(instance);
			
			placeholder.Parent = null;
			
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
			var descendants = instance.GetAllDescendants();
			
			for(int i = 0; i < descendants.Count; i++)
			{
				var subcomponent = descendants[i];
				
				if (subcomponent.Id != null)
					subcomponent.Id = subcomponent.Id.Replace("this", instance.Id);
				
				// animation
				XAnimationBase animation = subcomponent as XAnimationBase;
				if (animation != null)
				{
					animation.Target = animation.Target.Replace("this", instance.Id);
				}
				
				// triggers
				XTriggerBase trigger = subcomponent as XTriggerBase;
				if (trigger != null)
				{
					if (trigger.EnterAction != null)
						trigger.EnterAction = trigger.EnterAction.Replace("this", instance.Id);
					if (trigger.Action != null)
						trigger.Action = trigger.Action.Replace("this", instance.Id);
					if (trigger.ExitAction != null)
						trigger.ExitAction = trigger.ExitAction.Replace("this", instance.Id);
					
					XDistanceTrigger distanceTrigger = trigger as XDistanceTrigger;
					if (distanceTrigger != null)
					{
						distanceTrigger.ObjectId1 = distanceTrigger.ObjectId1.Replace("this", instance.Id);
						distanceTrigger.ObjectId2 = distanceTrigger.ObjectId2.Replace("this", instance.Id);
					}
				}
				
				// states
				XState state = subcomponent as XState;
				if (state != null)
				{
					if (state.When != null)
						state.When = state.When.Replace("this", instance.Id);
				}
			}
			
			
		}
	}
}

