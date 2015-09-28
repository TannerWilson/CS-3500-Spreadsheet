using System;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        // String used to represent the formula
        private String formulaString;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
            if (!isFormula(formula))
                throw new FormulaFormatException("Input string was an invalid formula");
            formulaString = formula;
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            // Normalize string
            formulaString = normalize(formula);
            if(!isFormula(formulaString))
                throw new FormulaFormatException("Input string was an invalid formula");
            // Check if the formula is valid after normalization
            if (!isValid(formulaString)) 
                throw new FormulaFormatException("Input string was an invalid formula");

        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            // Array of individual character strings of the formula  
            string[] substrings = GetTokens(formulaString).ToArray<String>();

            //string[] substrings = Regex.Split(formulaString, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // Two stacks used to evaluate the expression
            Stack<Double> values = new Stack<Double>();
            Stack<String> operators = new Stack<String>();

            for (int i = 0; i < substrings.Length; i++)
            {
                String t = substrings[i];
                // Ensure t is not whitespace
                if (t == " " || t == "")
                    continue;

                Double value1; // Used to store parsed int value and the final pushed result
                if (Double.TryParse(t, out value1)) // t is an integer
                {
                    // If expression stack is empty, push t
                    if (operators.Count == 0 || values.Count == 0)
                        values.Push(value1);

                    // If operator is * or /, pop and evaluate accordingly and push back to values stack
                    else if (operators.Peek() == "*" || operators.Peek() == "/")
                    {
                        String expression = operators.Pop();
                        Double value2 = values.Pop();
                        // Evaluate and push
                        try
                        {   // Surrounded by try to catch division by 0
                            performOperation(expression, value1, value2, values);
                        }
                        catch(FormulaFormatException e)
                        {
                            return new FormulaError(e.Message);
                        }
                    }
                    else
                        values.Push(value1);
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
                                // Pop top two values and top operator
                                Double val1, val2;
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
                        operators.Push(t);

                    else if (t == "(") // t is a ( 
                        operators.Push(t);

                    else if (t == ")") // t is a )
                    {
                        String top = operators.Peek();
                        // Evaluate addition and subtraction
                        if (top == "+" || top == "-")
                        {
                            // Pop top two values and top operator
                            Double val1, val2;
                            val1 = values.Pop();
                            val2 = values.Pop();
                            String operation = operators.Pop();
                            // Evaluate using given operator
                            performOperation(operation, val1, val2, values);
                        }

                        // Pop next operator to ensure it is a (, if not throw exception
                        String leftParand = "";
                        try
                        { // Ensure operators is not empty
                            leftParand = operators.Pop();
                        }catch (InvalidOperationException)
                        {
                            // Trow exception if no values are in the stack when we pop.
                            return new FormulaError("Too few operators");
                        }

                        if (leftParand != "(")
                            return new FormulaError("Left parand was expected, but not found");

                        // Evaluate mutiplicaton and division
                        if (top == "*" || top == "/")
                        {
                            // Pop top two values and top operator
                            Double val1, val2;
                            val1 = values.Pop();
                            val2 = values.Pop();
                            String operation = operators.Pop();
                            // Evaluate using given operator
                            try
                            { // Ensure no division by 0
                                performOperation(operation, val1, val2, values);
                            }
                            catch (FormulaFormatException e)
                            {
                                return new FormulaError(e.Message);
                            }
                        }
                    }
                    else // Operation possiblities are exhausted, so must be a varible
                    {
                        Double variableValue = 1;
                        // Look up value of varible
                        try
                        {
                            variableValue = lookup(t);
                        }
                        catch(Exception)
                        {
                            return new FormulaError("Invalid variable. Variable value was not found.");
                        }

                        // If operator stack is empty, push t
                        if (operators.Count == 0 || values.Count == 0)
                            values.Push(variableValue);

                        // If operator is * or /, pop and evaluate accordingly and push back to values stack
                        else if (operators.Peek() == "*" || operators.Peek() == "/")
                        {
                            String expression = operators.Pop();
                            Double value2 = values.Pop();
                            // Evaluate and push
                            try
                            { // Ensure no division by 0
                                performOperation(expression, variableValue, value2, values);
                            }
                            catch (FormulaFormatException e)
                            {
                                return new FormulaError(e.Message);
                            }
                        }
                        else
                            values.Push(variableValue);
                    }

                }
            }
            // Array of strings is exhausted
            if (operators.Count == 0) // No more operations
            {
                return values.Pop();
            }
            else // Still one + or - left to evaluate
            {
                // Pop top two values and top operator
                Double val1, val2;
                val1 = values.Pop();
                val2 = values.Pop();
                String operation = operators.Pop();
                // Evaluate using given operator
                try
                { // Ensure no division by 0
                    performOperation(operation, val1, val2, values);
                }
                catch (FormulaFormatException e)
                {
                    return new FormulaError(e.Message);
                }
                // Return final value
                return values.Pop();
            }
        }

        private void performOperation(string operation, Double value1, Double value2, Stack<Double> values)
        {
            Double result = 0;

            if (operation == "*")
            {
                result = value1 * value2;
                values.Push(result);
            }
            else if (operation == "/")
            {
                // Ensures no dividing by zero
                if (value1 == 0) throw new FormulaFormatException("Divided by zero");
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
                result = value2 - value1;
                values.Push(result);
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<string> result = new HashSet<string>();
            string[] tokens = GetTokens(formulaString).ToArray<string>();
            foreach(String s in tokens)
            {
                if (isVariable(s))
                    result.Add(s);
            }
            return result;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            String result = formulaString.Trim();
            result = Regex.Replace(result, @"\s+", "");
            return result;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            // Preliminary null checks
            if (obj == null || !(obj is Formula))
                return false;

            // Generate a string from the input formula
            Formula input = (Formula)obj;
            String form2 = input.ToString();

            // Generate arrays of the tokens of each formula expression
            string[] formula1 = GetTokens(formulaString).ToArray<string>();
            
            string[] formula2 = GetTokens(form2).ToArray<string>();

            // If the two are equal the arrays should be the same size
            if (formula1.Length != formula2.Length)
                return false;

            // Step through each entry in the arrays
            for(int i = 0; i < formula1.Length; i++)
            {
                // If both intries are equal, step to next
                if (formula1[i] == formula2[i])
                    continue;
                // Return false if not
                else return false;
            }
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            // Use defined equals method
            bool f1Null = false;
            bool f2Null = false;
            /* 
            Try catches used to check if the objects are null. If we try
            to access a null objet then that will thro an null refrence exception
            so if that is thrown, we know that the object is null, so we set that
            boolean to true and proceed.
            */
            //try
            //{
            //    if(f1 != null)
            //    {
            //        f1Null = false;
            //    }
            //}
            //catch (NullReferenceException)
            //{
            //    f1Null = true;
            //}
            //try
            //{
            //    if (f2 != null)
            //    {
            //        f2Null = false;
            //    }
            //}
            //catch (NullReferenceException)
            //{
            //    f2Null = true;
            //}
            //// Checking null booleans
            //if (f1Null == true && f2Null == true)
            //    return true;
            //if (f1Null == true && f2Null == false)
            //    return false;
            //if (f1Null == false && f2Null == true)
            //    return false;

            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            bool f1Null = false;
            bool f2Null = false;
            /* 
            Try catches used to check if the objects are null. If we try
            to access a null objet then that will thro an null refrence exception
            so if that is thrown, we know that the object is null, so we set that
            boolean to true and proceed.
            */
            //try
            //{
            //    if (f1 != null)
            //    {
            //        f1Null = false;
            //    }
            //}
            //catch (NullReferenceException)
            //{
            //    f1Null = true;
            //}
            //try
            //{
            //    if (f2 != null)
            //    {
            //        f2Null = false;
            //    }
            //}
            //catch (NullReferenceException)
            //{
            //    f2Null = true;
            //}
            //// Checking null booleans
            //if (f1Null == true && f2Null == true)
            //    return false;
            //if (f1Null == true && f2Null == false)
            //    return true;
            //if (f1Null == false && f2Null == true)
            //    return true;

            // Use defined equals method
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            // Array of individual character strings of the formula  
            string[] substrings = GetTokens(formulaString).ToArray<String>();
            int hash = 0;
            Double dub = 0;

            // Iterate over each token in the string
            foreach(String s in substrings)
            {
                // If is a double, strip ending 0's and add to hash
                if(Double.TryParse(s, out dub))
                {
                    String num = dub.ToString();
                    hash += num.GetHashCode();
                }
                else // Just use strings given hash code to add to total
                    hash += s.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        /// <summary>
        /// Used to check the validity of a given formula string. Restraints follow the 
        /// ones given in the formula documentation, and the rules given in the 
        /// assignment document on the class webpage.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool isFormula(String input)
        {
            // ints used to count the number of parands
            int openParands = 0;
            int closeParands = 0;

            // Double used to store double values (don't actually care about value stored)
            double value;

            // Sperarate the tokens in the string
            String[] tokens = GetTokens(input).ToArray<String>();

            // Tokens must have at least one token
            if (tokens.Length < 1)
                throw new FormulaFormatException("Formula cannot be empty");

            // First token must be a double, variable or a '('
            if (!isVariable(tokens[0]) && tokens[0] != "(" && !Double.TryParse(tokens[0], out value))
                throw new FormulaFormatException("Invalad first token");

            // Last token must be a double, variable or a ')'
            if (!isVariable(tokens[tokens.Length - 1]) && !(tokens[tokens.Length - 1] == ")") 
                && !Double.TryParse(tokens[tokens.Length - 1], out value))
                throw new FormulaFormatException("Invalad last token");

            // Iterate over each token to ensure validity
            for (int i = 0; i < tokens.Length; i++)
            {
                // Check if the token is an accepted string
                if (isOperator(tokens[i]) || isVariable(tokens[i])
                    || Double.TryParse(tokens[i], out value) || isParand(tokens[i]))
                {
                    // Token is acceptable, check each syntatic rule

                    if (tokens[i] == "(")
                        openParands++;

                    // Right Parentheses Rule
                    if (tokens[i] == ")")
                    {
                        closeParands++;
                        if (closeParands > openParands)
                            throw new FormulaFormatException("Too many closing parentheses");
                    }

                    // Parenthesis Following Rule
                    if (tokens[i] == "(" || isOperator(tokens[i]))
                    {
                        /*
                         Token following an opening parenthesis oroperator 
                         must be either a number, a variable, or an opening parenthesis.
                        */
                        if (i+1 < tokens.Length && (!isVariable(tokens[i+1]) && !Double.TryParse(tokens[i+1], out value) && tokens[i+1] != "("))
                            throw new FormulaFormatException("Invalid token folowing an open parand or operator");
                    }

                    // Extra Following Rule
                    if (isVariable(tokens[i]) || Double.TryParse(tokens[i], out value) || tokens[i] == ")")
                    {
                        /*
                        Token following a number, a variable, or a closing parenthesis
                        must be either an operator or a closing parenthesis.
                        */
                        if(i+1 < tokens.Length && (!isOperator(tokens[i+1]) && tokens[i+1] != ")"))
                            throw new FormulaFormatException("Invalid token folowing a number, variable, or closing parenthesis");
                    }

                } // token is not an accepted operator or varible name

                else return false;
            }

            // Ensure the number of parenthesis are equal
            if (openParands != closeParands)
                throw new FormulaFormatException("Unbalanced parands");

            return true;
        }

        /// <summary>
        /// Checks if the given input string is a valid varible name.
        /// Valid varible names are defined as strings that consist of a letter 
        /// or underscore followed by zero or more letters, underscores, or digits.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool isVariable(string input)
        {
            // Convert the string into a character array
            char[] chars = input.ToCharArray();

            // Ensure the first character in the string is a letter or undersore
            if (Char.IsLetter(chars[0]) || chars[0] == '_')
            {
                // Ensure the last character is either a letter, number, or an underscore
                if (Char.IsLetter(chars[chars.Length - 1]) || Char.IsDigit(chars[chars.Length - 1])
                    || chars[chars.Length - 1] == '_')
                {
                    // First and last chars are valid, check validity of all chars
                    for(int i = 1; i < chars.Length - 1; i++)
                    {
                        // Check if each character is NOT a letter, digit, or underscore
                        if (!(Char.IsLetter(chars[i]) || Char.IsDigit(chars[i]) || chars[i] == '_'))
                        {
                            return false;
                        }
                    }

                } // Last char wasnt valid
                else return false;

            } // First char wasn't valid
            else return false; 
               
            return true;
        }

        /// <summary>
        /// Checks if the input string is one of our accepted 
        /// operators; +, -, /, *. This also includes parands; (, ).
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool isOperator(String input)
        {
            if(input == "+" || input == "-" || input == "/" || input == "*")
                return true;
            return false;
        }

        /// <summary>
        /// Checks if the input string is one of our accepted 
        /// operators; +, -, /, *. This also includes parands; (, ).
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool isParand(String input)
        {
            if (input == "(" || input == ")")
                return true;
            return false;
        }
    }
}

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }


