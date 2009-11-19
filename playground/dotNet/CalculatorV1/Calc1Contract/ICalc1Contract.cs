using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Contract;
using System.AddIn.Pipeline;

namespace CalculatorContracts
{
    // The AddInContractAttribute identifes this pipeline segment as a 
    // contract.
    [AddInContract]
    public interface ICalc1Contract : IContract
    {
        double Add(double a, double b);
        double Subtract(double a, double b);
        double Multiply(double a, double b);
        double Divide(double a, double b);
    }
}