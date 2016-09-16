﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cassandra.Mapping.Attributes
{
    /// <summary>
    /// Indicates that the property or field represents a column which key is frozen.
    /// Only valid for maps and sets.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FrozenKeyAttribute : Attribute
    {

    }
}
