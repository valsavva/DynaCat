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
		
        /// <summary>
        /// Specifies action to execute when the area is tapped.
        /// </summary>
		[XmlAttribute]
		public string Action;
        /// <summary>
        /// Specifies action to execute when the area is tapped.
        /// </summary>
        [XmlAttribute]
        public string MoveAction;
        /// <summary>
        /// Specifies action to execute when the area is tapped.
        /// </summary>
        [XmlAttribute]
        public string ReleaseAction;
        /// <summary>
        /// Gets/sets tap type.
        /// </summary>
        [XmlIgnore]
		public XTapType TapType;
        /// <summary>
        /// The X coordinate of the tap.
        /// </summary>
        [XmlIgnore]
        public double TapX;
        /// <summary>
        /// The Y coordinate of the tap.
        /// </summary>
        [XmlIgnore]
        public double TapY;

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			PerfMon.Start("Other-Tap");

			p.ScreenEngine.tapAreas.Add(this);

            InitActions(this.Action, ref this.pressActions);
            InitActions(this.MoveAction, ref this.moveActions);
            InitActions(this.ReleaseAction, ref this.releaseActions);

			PerfMon.Stop("Other-Tap");
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
			
			this.TapType = XTapType.None;
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

        public override void ReadXml(System.Xml.XmlReader reader)
        {
			reader.ReadAttrAsString("Action", ref this.Action);
			reader.ReadAttrAsString("MoveAction", ref this.MoveAction);
			reader.ReadAttrAsString("ReleaseAction", ref this.ReleaseAction);

			var newEvent = reader["Event"];
			if (newEvent != null)
				this.Event = newEvent;
            
            base.ReadXml(reader);
        }
        
        public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
        {
            switch (propertyName)
            {
                case "TapX": getter = () => TapX; setter = (v) => TapX = v; break;
                case "TapY": getter = () => TapY; setter = (v) => TapY = v; break;
                default:
                    base.GetProperty(propertyName, out getter, out setter); break;
            }
        }
    }
}

