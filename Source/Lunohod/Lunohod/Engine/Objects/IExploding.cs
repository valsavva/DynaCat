using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
    /// <summary>
    /// This interface describes a component that can be exploded.
    /// </summary>
    public interface IExploding
    {
        /// <summary>
        /// Gets a value specifying whether the component can be exploded.
        /// </summary>
        [XmlAttribute]
        bool IsExploding { get; set; }
    }
}
