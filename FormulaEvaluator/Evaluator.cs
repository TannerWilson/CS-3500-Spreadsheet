using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            Delegate lookUp = variableEvaluator;

            exp.Trim();
            // Array of individual character strings of "exp"
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // Two stacks used to evaluate the expression
            Stack<int> values = new Stack<int>();
            Stack<String> operators = new Stack<String>();

            for (int i = 0; i < substrings.Length; i++)
            {
                String t = substrings[i];
                // Ensure t is not whitespace
                if (t == " " || t == "")
                {
                    continue;
                }

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
                        // Evaluate and push
                        performOperation(expression, value1, value2, values);
                    }
                    else
                    {
                        values.Push(value1);
                    }
                }

                else // String is not interger, check possible strings
                {
                    if (t == "+" || t == "-") // t is either a + or -
                    {
                        if (operators.Count != 0) // Operators stack is empty, just push
                        {
                            // Check if the top operation in the operators stack is a + or -
                            String topOp = operators.Peek();
                            if (topOp == "+" || topOp == "-")
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
                                performOperation(operation, val1, val2, values);
                            }
                        }

                        // Push t onto operators
                        operators.Push(t);
                    }

                    else if (t == "*" || t == "/") // t is either a * or /
                    {
                        operators.Push(t);
                    }

                    else if (t == "(") // t is a ( 
                    {
                        operators.Push(t);
                    }

                    else if (t == ")") // t is a )
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
                            performOperation(operation, val1, val2, values);
                        }

                        // Pop next operator to ensure it is a (, if not throw exception
                        String leftParand = operators.Pop();
                        if (leftParand != "(")
                            throw new ArgumentException("Left parand was expected, but not found");

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
                            performOperation(operation, val1, val2, values);
                        }
                    }
                    else // Operation possiblities are exhausted, so must be a varible
                    {
                        int valribleValue;
                        // Look up value of varible
                      
                            valribleValue = variableEvaluator(t);
                      
                        // If expression stack is empty, push t
                        if (operators.Count == 0 || values.Count == 0)
                        {
                            values.Push(valribleValue);
                        }
                        // If operator is * or /, pop and evaluate accordingly and push back to values stack
                        else if (operators.Peek() == "*" || operators.Peek() == "/")
                        {
                            String expression = operators.Pop();
                            int value2 = values.Pop();
                            // Evaluate and push
                            performOperation(expression, valribleValue, value2, values);
                        }
                        else
                        {
                            values.Push(valribleValue);
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

        /// <summary>
        /// Per fprms the given operator passed as a parameter to the two input values and
        /// pushes the result back on to the given stack.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static void performOperation(String operation, int value1, int value2, Stack<int> values)
        {
            int result = 0;

            if (operation == "*")
            {
                result = value1 * value2;
                values.Push(result);
            }
            else if (operation == "/")
            {
                // Ensures no dividing by zero
                if (value1 == 0) throw new ArgumentException("Divided by zero");
                result = value2 / value1;
                values.Push(result);
            }
            else if (operation == "+")
            {
                result = value1 + value2;
                values.Push(result);
            }
            else if (operation == "-")
            {
                result = value1 - value2;
                values.Push(result);
            }
        }
    }
}

