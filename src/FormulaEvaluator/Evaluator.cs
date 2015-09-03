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
        /// Evaluates the equation string "exp", using infix notation, and returns the 
        /// value of the expression.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            // Array of individual characters of "exp"
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            
            // Two stacks used to evaluate the expression
            Stack<int> values = new Stack<int>();
            Stack<String> operators = new Stack<String> ();

            for(int i = 0; i < substrings.Length; i++)
            {
                Object t = substrings[i];
                if (t is int) // t is an integer
                {
                    // If expression stack is empty, push t
                    if (operators.Count == 0)
                    {
                        values.Push((int)t);
                    }
                    // If operator is * or /, pop and evaluate accordingly and push back to values stack
                    else if (operators.Peek() == "*" || operators.Peek() == "/")
                    {
                        String expression = operators.Pop();
                        int value = values.Pop();
                        if (expression == "*")
                        {
                            t = (int) t * value;
                            values.Push((int) t);
                        }
                        else
                        {
                            t = (int)t / value;
                            values.Push((int)t);
                        }

                    }
                }

                // Insert same evaluation for if t is a varible...it may belong after the check if t is a string

                else if (t is String) // t is a String
                {
                    // t is either a + or -
                    if((String) t == "+" || (String) t == "-")
                    {

                    }
                    // t is either a * or /
                    if ((String)t == "*" || (String)t == "/")
                    {
                        operators.Push((String) t);
                    }
                    // t is a ( 
                    if ((String)t == "(")
                    {

                    }
                    // t is a )
                    if ((String)t == ")")
                    {

                    }
                }

            }
        }
    }
}
