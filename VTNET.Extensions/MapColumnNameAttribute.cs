using System;

namespace VTNET.Extensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapColumnNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public MapColumnNameAttribute(string name)
        {
            Name = name;
        }
    }
}
