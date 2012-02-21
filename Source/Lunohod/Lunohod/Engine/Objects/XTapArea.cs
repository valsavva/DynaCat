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
		public string Event;
		
		[XmlAttribute]
		public string Action;
		
		public ActionCallerBase ActionCaller { get { return actionCaller; } }
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			p.Game.ScreenEngine.tapAreas.Add(this);
			
			if (this.Action != null)
				actionCaller = Lunohod.Objects.ActionCaller.CreateActionCaller(this, this.Action);
		}
		
		internal override void ReplaceParameter(string par, string val)
		{
			if (this.Action != null)
				this.Action = this.Action.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}
	}
}

