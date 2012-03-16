using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Lunohod.Xge;

namespace Lunohod.Objects
{
	
    [XmlType("TapArea")]
	public class XTapArea : XElement
	{
        private List<IAction> pressActions;
        private List<IAction> moveActions;
        private List<IAction> releaseActions;
		
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

        [XmlAttribute]
        public string MoveAction;

        [XmlAttribute]
        public string ReleaseAction;

		[XmlIgnore]
		public XTapType TapType;

        [XmlIgnore]
        public float TapX;

        [XmlIgnore]
        public float TapY;

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			p.Game.ScreenEngine.tapAreas.Add(this);

            InitActions(this.Action, ref this.pressActions);
            InitActions(this.MoveAction, ref this.moveActions);
            InitActions(this.ReleaseAction, ref this.releaseActions);
        }

        private void InitActions(string actionText, ref List<IAction> actionList)
        {
            if (!string.IsNullOrEmpty(actionText))
                actionList = Compiler.CompileStatements(this, actionText);
        }
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);


            if (this.TapType == XTapType.None)
            {
                // noop
            }
            else if (pressActions != null && this.TapType == XTapType.Press)
            {
                pressActions.ForEach(a => a.Call());
            }
            else if (moveActions != null && (this.TapType == XTapType.Press || this.TapType == XTapType.Move))
            {
                moveActions.ForEach(a => a.Call());
            }
            else if (releaseActions != null && this.TapType == XTapType.Release)
            {
                releaseActions.ForEach(a => a.Call());
            }
		}
		
		internal override void ReplaceParameter(string par, string val)
		{
            if (!string.IsNullOrEmpty(this.Action))
                this.Action = this.Action.Replace(par, val);

            if (!string.IsNullOrEmpty(this.MoveAction))
                this.MoveAction = this.MoveAction.Replace(par, val);

            if (!string.IsNullOrEmpty(this.ReleaseAction))
                this.MoveAction = this.MoveAction.Replace(par, val);

            base.ReplaceParameter(par, val);
		}
	}
}

