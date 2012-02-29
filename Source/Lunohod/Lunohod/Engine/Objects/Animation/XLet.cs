using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;

namespace Lunohod.Objects
{
    /// <summary>
    /// A "runnalbe" component that changes attrubute values.
    /// </summary>
    [XmlType("Let")]
    public class XLet : XRunnableBase
    {
        private List<PropertyAccessor> accessors;
        private List<NumValueReader> valueReaders;

        /// <summary>
        /// An attribute, or a list of attributes, that will be assigned a new value.
        /// </summary>
        [XmlAttribute]
        public string Target;
        /// <summary>
        /// A new value, or list of values.
        /// </summary>
        [XmlAttribute]
        public string Value;

        public XLet()
        {
            this.RepeatCount = 1;
        }

        /// <inheritdoc />
        public override void Initialize(InitializeParameters p)
        {
            base.Initialize(p);

            accessors = this.Target.Split(',').Select(s => new PropertyAccessor(this, s)).ToList();
            valueReaders = this.Value.Split(',').Select(s => new NumValueReader(this, s)).ToList();

            if (accessors.Count != valueReaders.Count)
                throw new InvalidOperationException("Number of values must match the number of properties.");
        }

        internal override void UpdateProgress(UpdateParameters p)
        {
            for (int i = 0; i < accessors.Count; i++)
            {
                accessors[i].FloatPropertyValue = valueReaders[i].Value;
            }
            this.repeatsDone++;
        }
    }
}
