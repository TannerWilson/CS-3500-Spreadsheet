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
            // Array of individual character strings of "exp"
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // Two stacks used to evaluate the expression
            Stack<int> values = new Stack<int>();
            Stack<String> operators = new Stack<String>();

            for (int i = 0; i < substrings.Length; i++)
            {
                String t = substrings[i];
                int value1; // Used to store parsed int value and the final pushed result
                if (Int32.TryParse(t, out value1)) // t is an integer
                {
                    // If expression stack is empty, push t
                    if (operators.Count == 0 || values.Count == 0)
                    {
                        values.Push(value1);
                    }
                    // If operator is * or /, pop and evaluate accordingly and push back to values stack
                    else if (operators.Peek() == "*" || operators.Peek() == "/")
                    {
                        String expression = operators.Pop();
                        int value2 = values.Pop();
                        if (expression == "*")
                        {
                            value1 = value1 * value2;
                            values.Push(value1);
                        }
                        else
                        {
                            // Ensures no dividing by zero
                            if (value2 == 0) throw new ArgumentException("Divided by zero");
                            value1 = value1 / value2;
                            values.Push(value1);
                        }

                    }
                }

                // Insert same evaluation for if t is a varible...it may belong after the check if t is a string

                else
                {
                    // t is either a + or -
                    if (t == "+" || t == "-")
                    {
                        // Ensure enough values are in the values stack
                        if (values.Count < 2)
                            throw new ArgumentException("Not enough values in stack to perform operation");

                        // Pop top two values and top operator
                        int val1, val2;
                        val1 = values.Pop();
                        val2 = values.Pop();
                        String operation = operators.Pop();
                        // Evaluate using given operator
                        if (operation == "*")
                        {
                            val1 = val1 * val2;
                            values.Push(val1);
                        }
                        else if (operation == "/")
                        {
                            // Ensures no dividing by zero
                            if (val2 == 0) throw new ArgumentException("Divided by zero");
                            val1 = val1 / val2;
                            values.Push(val1);
                        }
                        else if (operation == "+")
                        {
                            val1 = val1 + val2;
                            values.Push(val1);
                        }
                        else if (operation == "-")
                        {
                            val1 = val1 - val2;
                            values.Push(val1);
                        }
                        // Push t onto operators
                        operators.Push(t);
                    }

                    // t is either a * or /
                    if (t == "*" || t == "/")
                    {
                        operators.Push(t);
                    }

                    // t is a ( 
                    if (t == "(")
                    {
                        operators.Push(t);
                    }
                    // t is a )
                    if (t == ")")
                    {
                        String top = operators.Peek();
                        // Evaluate addition and subtraction
                        if (top == "+" || top == "-")
                        {
                            // Ensure enough values are in the values stack
                            if (values.Count < 2)
                                throw new ArgumentException("Not enough values in stack to perform operation");

                            // Pop top two values and top operator
                            int val1, val2;
                            val1 = values.Pop();
                            val2 = values.Pop();
                            String operation = operators.Pop();
                            // Evaluate using given operator
                            if (operation == "+")
                            {
                                val1 = val1 + val2;
                                values.Push(val1);
                            }
                            else
                            {
                                val1 = val1 - val2;
                                values.Push(val1);
                            }

                        }
                        // Pop next operator to ensure it is a (, if not throw exception
                        String leftParand = operators.Pop();
                        if (leftParand != "(")
                            throw new ArgumentException("Left parand not where one was expected");

                        // Evaluate mutiplicaton and division
                        if (top == "*" || top == "/")
                        {
                            // Ensure enough values are in the values stack
                            if (values.Count < 2)
                                throw new ArgumentException("Not enough values in stack to perform operation");

                            // Pop top two values and top operator
                            int val1, val2;
                            val1 = values.Pop();
                            val2 = values.Pop();
                            String operation = operators.Pop();
                            // Evaluate using given operator
                            if (operation == "*")
                            {
                                val1 = val1 * val2;
                                values.Push(val1);
                            }
                            else
                            {
                                // Ensures no dividing by zero
                                if (val2 == 0) throw new ArgumentException("Divided by zero");
                                val1 = val1 / val2;
                                values.Push(val1);
                            }
                        }
                    }

                }
            }
            // Array of strings is exhausted
            if (operators.Count == 0) // No more operations
            { 
                if (values.Count > 1) throw new ArgumentException("More than one finishing value");
                return values.Pop();
            }
            else // Still one + or - left to evaluate
            {
                // Not excatly one operator or two values
                if (values.Count > 2) throw new ArgumentException("More than two finishing value");
                if (operators.Count > 1) throw new ArgumentException("More than one finishing operator");

                // Pop top two values and top operator
                int val1, val2;
                val1 = values.Pop();
                val2 = values.Pop();
                String operation = operators.Pop();
                // Evaluate using given operator
                if (operation == "+")
                {
                    val1 = val1 + val2;
                }
                else
                {
                    val1 = val1 - val2;
                }
                // Return final value
                return val1;
            }
        }
    }
}

