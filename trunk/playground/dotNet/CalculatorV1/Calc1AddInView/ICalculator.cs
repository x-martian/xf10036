using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Pipeline;

namespace CalcAddInViews
{
    // The AddInBaseAttribute identifes this interface as the basis for
    // the add-in view pipeline segment.
    [AddInBase()]
    public interface ICalculator
    {
        double Add(double a, double b);
        double Subtract(double a, double b);
        double Multiply(double a, double b);
        double Divide(double a, double b);
    }
}