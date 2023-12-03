using System;
using System.Collections.Generic;
using System.Text;

namespace VTNET.Extensions.Models
{
    public class CalculateFuntionCustom
    {
        public CalculateFuntionCustom(Func<double[], bool, double> function, int parameterRequired = 0)
        {
            ParameterRequired = parameterRequired;
            Function = function;
        }
        public int ParameterRequired { get; set; }
        public Func<double[], bool, double> Function { get; set; } = (b,c)=> { return 0; };
    }
}
