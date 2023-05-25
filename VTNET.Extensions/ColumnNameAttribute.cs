using System;

namespace VTNET.Extensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public ColumnNameAttribute(string name)
        {
            Name = name;
        }
    }
}
