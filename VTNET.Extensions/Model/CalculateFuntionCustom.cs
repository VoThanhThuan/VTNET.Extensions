using System;
using System.Collections.Generic;
using System.Text;

namespace VTNET.Extensions.Model
{
    public class CalculateFuntionCustom
    {
        public CalculateFuntionCustom(Func<double[], double> function, int parameterRequired = 0)
        {
            ParameterRequired = parameterRequired;
            Function = function;
        }
        public int ParameterRequired { get; set; }
        public Func<double[], double> Function { get; set; } = (b)=> { return 0; };
    }
}
