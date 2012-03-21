using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunohod;

namespace Lunohod.Objects
{
    [XmlRoot("Level")]
    public class XLevel : XScreen
    {
		[XmlIgnore]
		public XLevelSettings DefaultSettings { get; private set; }
		
		[XmlIgnore]
		public List<XElement> Exploding { get; private set; }
		
		public override void Initialize(InitializeParameters p)
		{
			this.DefaultSettings = this.GetComponents<XLevelSettings>().FirstOrNew();
			this.Exploding = new List<XElement>();

			base.Initialize(p);
			
			this.EnqueueEvent(GameEventType.LevelLoaded);
		}
		
		public override void Update(UpdateParameters p)
		{
			if (p.Game.ScreenEngine.CurrentEvents.ContainsKey("system:levelLoaded"))
				// removing the effect of level loading
#if WINDOWS || XBOX
				p.GameTime = new GameTime(
					p.GameTime.TotalGameTime, TimeSpan.FromMilliseconds(3)
				);
#else
				p.GameTime = new GameTime(
					p.GameTime.TotalRealTime, p.GameTime.ElapsedRealTime,
					p.GameTime.TotalGameTime, TimeSpan.FromMilliseconds(3)
				);
#endif
			
			base.Update(p);
		}
    }
}
