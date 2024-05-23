using System;

namespace VTNET.Extensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public MapNameAttribute(string name)
        {
            Name = name;
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreMapNameAttribute : Attribute
    {
    }
}
