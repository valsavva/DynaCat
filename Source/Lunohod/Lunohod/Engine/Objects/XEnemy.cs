using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Lunohod.Objects
{
    [XmlType("Enemy")]
    public class XEnemy : XElement, IExploding
    {
        /// <inheritdoc />
        [XmlAttribute]
        public bool IsExploding { get; set; }
    }
}

