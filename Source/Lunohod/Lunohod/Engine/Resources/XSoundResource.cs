using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Lunohod.Objects
{
	[XmlType("SoundFile")]	
	public class XSoundResource : XResource
	{
		private SoundEffect soundEffect;

		private Dictionary<SoundEffectInstance, int> instanceOwners;
		private Dictionary<SoundEffectInstance, int> instanceCheckoutTimes;
		private Dictionary<int, SoundEffectInstance> ownerInstances;

		internal SoundEffectInstance soundEffectInstance;

		public XSoundResource()
		{
		}

		/// <summary>
		/// The number of instances of the current sound effect. Default value is 3. If set to zero or a negative value, the number of instances will not be limited.
		/// </summary>
		[XmlAttribute]
		public int NumberOfInstances = 3;
		/// <summary>
		/// The maximum number of instances that can start playing at once.
		/// This handles the situation when more than one events of the same type and having the same sound effect happen at the same time.
		/// If sound effects play simutaniously, it just increases the volume of the sound.
		/// </summary>
		[XmlAttribute]
		public int MaxStartAtOnce = 1;

		public SoundEffect SoundEffect
		{
			get { return this.soundEffect; }
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

            this.soundEffect = LoadResource<SoundEffect>(p.Game.Content, "SoundEffectProcessor", "wav", "xnb");

			if (this.NumberOfInstances > 0)
			{
				// number of instances will be limited
				this.instanceOwners = new Dictionary<SoundEffectInstance, int>(this.NumberOfInstances);
				this.instanceCheckoutTimes = new Dictionary<SoundEffectInstance, int>(this.NumberOfInstances);

				for(int i = 0; i < this.NumberOfInstances; i++)
				{
					var soundEffectInstance = this.soundEffect.CreateInstance();
					this.instanceOwners.Add(soundEffectInstance, 0);
					this.instanceCheckoutTimes.Add(soundEffectInstance, 0);
				}
			}
			else
				this.ownerInstances = new Dictionary<int, SoundEffectInstance>();
		}

		internal bool CheckOutInstance(XSound owner)
		{
			SoundEffectInstance result = null;

			if (this.NumberOfInstances > 0)
			{
				// limited number of instances

				var currCheckoutTime = GameEngine.Instance.CycleNumber;

				int checkedoutThisCycle = 0;
				int oldestCycle = int.MaxValue;
				SoundEffectInstance oldestItem = null;
				foreach(var item in this.instanceCheckoutTimes)
				{
					// if the instance already belongs to the owner - quit
					if (this.instanceOwners[item.Key] == owner.GetHashCode())
					{
						result = item.Key;
						break;
					}

					// find the oldest instance
					if (oldestCycle > item.Value)
					{
						oldestCycle = item.Value;
						oldestItem = item.Key;
					}

					// count the numbe of instances checked out in this cycle
					if (item.Value == currCheckoutTime)
						checkedoutThisCycle++;
			    }


				if (result == null)
				{
					// if we exceeded the limit - quit
					if (this.MaxStartAtOnce > 0 && checkedoutThisCycle >= this.MaxStartAtOnce)
					{
						owner.soundEffectInstance = null;
						return false;
					}

					result = oldestItem;
					this.instanceOwners[result] = owner.GetHashCode();
				}

				this.instanceCheckoutTimes[result] = currCheckoutTime;

				Console.WriteLine("*** Id:" + this.Id + " Source:" + this.Source + " Result:" + result.GetHashCode() + " ***");
				Console.WriteLine("*** chekout times *** ");
				foreach(var item in this.instanceCheckoutTimes)
				{
					Console.WriteLine(item.Key.GetHashCode().ToString() + " - " + item.Value);
				}
				Console.WriteLine("*** owners *** ");
				foreach(var item in this.instanceOwners)
				{
					Console.WriteLine(item.Key.GetHashCode().ToString() + " - " + item.Value);
				}
				Console.WriteLine("******");
			}
			else
			{

				if (!this.ownerInstances.TryGetValue(owner.GetHashCode(), out result))
				{
					result = this.soundEffect.CreateInstance();
					this.ownerInstances[owner.GetHashCode()] = result;
				}
			}

			owner.soundEffectInstance = result;

			return true;
		}

		internal bool VerifyInstance(XSound owner)
		{
			if (owner.soundEffectInstance == null)
				return false;

			if (this.NumberOfInstances > 0)
			{
				if (this.instanceOwners[owner.soundEffectInstance] != owner.GetHashCode())
				{
					owner.soundEffectInstance = null;
					return false;
				}
			}

			return true;
		}

		public override void ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadAttrAsInt("NumberOfInstances", ref this.NumberOfInstances);
			reader.ReadAttrAsInt("MaxStartAtOnce", ref this.MaxStartAtOnce);

			base.ReadXml(reader);
		}

		public override void Dispose()
		{
			if (this.NumberOfInstances > 0)
			{
				foreach(var item in this.instanceOwners)
					item.Key.Dispose();

				this.instanceOwners = null;
			}
			else
			{
				foreach(var item in this.ownerInstances)
					item.Value.Dispose();

				this.ownerInstances = null;
			}

			base.Dispose();
		}
	}
}

