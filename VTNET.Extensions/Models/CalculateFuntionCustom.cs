using System;

namespace VTNET.Extensions.Models
{
    public class CalculateFuntionCustom
    {
        public CalculateFuntionCustom(Func<decimal?[], bool, decimal?> function, int parameterRequired = 0)
        {
            ParameterRequired = parameterRequired;
            Function = function;
        }
        public int ParameterRequired { get; set; }
        public Func<decimal?[], bool, decimal?> Function { get; set; } = (b,c)=> { return 0; };
    }
}
