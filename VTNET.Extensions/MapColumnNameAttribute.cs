using System;

namespace VTNET.Extensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapColumnNameAttribute : MapNameAttribute
    {
        public MapColumnNameAttribute(string name) : base(name)
        {
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreMapColumnNameAttribute : IgnoreMapNameAttribute
    {
    }
}
