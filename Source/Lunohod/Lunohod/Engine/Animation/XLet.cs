using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;
using Lunohod.Xge;

namespace Lunohod.Objects
{
    /// <summary>
    /// A "runnalbe" component that changes attrubute values.
    /// </summary>
    [XmlType("Let")]
    public class XLet : XRunnableBase
    {
        private List<NumProperty> accessors;
        private List<INumExpression> valueReaders;

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

            accessors = this.Target.Split(',').Select(s => (NumProperty)Compiler.CompileNumExpression(this, s)).ToList();
            valueReaders = this.Value.Split(',').Select(s => Compiler.CompileNumExpression(this, s)).ToList();

            if (accessors.Count != valueReaders.Count)
                throw new InvalidOperationException("Number of values must match the number of properties.");
        }

        internal override void UpdateProgress(UpdateParameters p)
        {
            for (int i = 0; i < accessors.Count; i++)
            {
                accessors[i].SetValue(valueReaders[i].Value);
            }
            this.repeatsDone++;
        }
    }
}
