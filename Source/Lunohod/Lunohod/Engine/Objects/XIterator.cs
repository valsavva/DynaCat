using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod.Objects
{
	
    [XmlType("Iterator")]
	public class XIterator : XRunnableBase
	{
		private List<XObject> objects;
		
		[XmlAttribute()]
		public string TypeFilter;
		
		[XmlAttribute()]
		public string ObjectIds;
		
		[XmlIgnore]
		public Type TypeFilterType { get; private set; }
		[XmlIgnore]
		public XObject Current { get; private set; }
		
		public XIterator()
		{
			this.RepeatCount = 1;
			this.TypeFilter = "Element";
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			string internalTypeName = "Lunohod.Objects.X" + this.TypeFilter;
			this.TypeFilterType = this.GetType().Assembly.GetType(internalTypeName);
			
			if (this.TypeFilterType == null)
			{
				throw new InvalidOperationException("Iterator could not find type: " + internalTypeName);
			}
			
			if (this.ObjectIds != null)
			{
				this.objects = this.ObjectIds.Split(',').Select(id => FindObject(id, p.ScreenEngine.RootComponent)).ToList();
			}
		}
		
        private XObject FindObject(string id, XObject root)
        {
            var o = root.FindDescendant(id);
            if (o == null)
                throw new InvalidOperationException(string.Format("Iterator '{0}' could not find obect id: '{1}'", this.Id, id));
            return o;
        }

		internal override void UpdateProgress(UpdateParameters p)
		{
			if (this.objects != null)
			{
				for(int i = 0; i < this.objects.Count; i++)
				{
					var obj = this.objects[i];

					if (obj.Enabled)
					{
						this.Current = this.objects[i];
						this.UpdateChildren(p);
					}
				}
			}
			else
			{
				p.ScreenEngine.RootComponent.TraveseTree(o => {
					if (this.TypeFilterType.IsAssignableFrom(o.GetType()))
					{
						if (o.Enabled)
						{
							this.Current = o;
							this.UpdateChildren(p);
						}
					}
				});
			}
		}
	}
}

