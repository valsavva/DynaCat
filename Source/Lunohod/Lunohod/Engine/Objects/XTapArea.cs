using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Lunohod.Objects
{
	
    [XmlType("TapArea")]
	public class XTapArea : XElement
	{
		private ActionCallerBase actionCaller;
		
		public XTapArea()
		{
		}
		
		[XmlAttribute]
		public string Event
		{
			get { return this.Action.Replace("~", ""); }
			set {
				if (value.StartsWith("~"))
					this.Action = value;
				else
					this.Action = "~" + value;
			}
		}
		
		[XmlAttribute]
		public string Action;
		
		[XmlIgnore]
		public bool IsTapped;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			p.Game.ScreenEngine.tapAreas.Add(this);
			
			actionCaller = Lunohod.Objects.ActionCaller.CreateActionCaller(this, this.Action);
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			if (this.IsTapped)
			{
				actionCaller.Call();
				this.IsTapped = false;
			}
			
		}
		
		internal override void ReplaceParameter(string par, string val)
		{
			this.Action = this.Action.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}
	}
}

