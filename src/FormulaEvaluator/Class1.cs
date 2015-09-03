using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    /// <summary>
    /// This class operates as a library to support a calculator application which can 
    /// evaluate basic equations fed in as strings.
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(String v);

        /// <summary>
        /// Evaluates the equation string "exp" and returns the 
        /// value of the expression.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
             
        }
    }
}
