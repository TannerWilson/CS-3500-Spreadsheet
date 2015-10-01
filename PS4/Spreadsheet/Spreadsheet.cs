using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        // Used to store all cells in the spreadsheet
        private Dictionary<string, cell> cells;
        // Used to store all the dependencies in the spreadsheet
        private DependencyGraph dependencies;

        /// <summary>
        /// Constructs an empty SpreadSheet object.
        /// </summary>
        public Spreadsheet()
        {
            cells = new Dictionary<string, cell>();
            dependencies = new DependencyGraph();
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            if (name == null || !isName(name))
                throw new InvalidNameException();

            cell value;
            // Check if cells has the cell "name", if so store its value cell in value
            if (cells.TryGetValue(name, out value))
            {
                // return value's contents
                return value.getContents();
            }
            else return "";

        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            /*
               If a cell was added to the dictionary "cells" then the cell value must be 
               non-empty. So simply return the names of every added cell, or the keys of the 
               "cells" dictionary.
            */
            LinkedList<string> result = new LinkedList<string>();
            foreach (String key in cells.Keys)
            {
                result.AddLast(key);
            }
            return result;
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            // Formula null check
            if (formula == null)
                throw new ArgumentNullException("Given formula was null");

            // Valid name check
            if (name == null || !isName(name))
                throw new InvalidNameException();

            // Check if name is in the changed cells
            cell outVal;
            if (cells.TryGetValue(name, out outVal))
            {
                outVal.setContents(formula);

                LinkedList<string> reCalc = (LinkedList<string>)GetCellsToRecalculate(name);
                HashSet<string> result = new HashSet<string>();

                foreach (string s in reCalc)
                    result.Add(s);
                return result;
            }
            else // No cell named "name" was found
            {
                // Add cell to changed cells
                cell adding = new cell(name, formula, null);

                // Add name to dependencies and add its varibles as dependents
                dependencies.AddDependency(name, " ");
                LinkedList<String> vars = (LinkedList<String>) formula.GetVariables();
                dependencies.ReplaceDependents(name, vars);

                // Return new dependents
                LinkedList<string> reCalc = (LinkedList<string>)GetCellsToRecalculate(name);
                HashSet<string> result = new HashSet<string>();

                foreach (string s in reCalc)
                    result.Add(s);
                return result;
            }


        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, string text)
        {
            // Text null check
            if (text == null)
                throw new ArgumentNullException("Given formula was null");

            // Valid name check
            if (name == null || !isName(name))
                throw new InvalidNameException();

            // Check if name is in the changed cells
            cell outVal;
            if (cells.TryGetValue(name, out outVal))
            {
                outVal.setContents(text);
                return new HashSet<String>();
            }
            else // No cell named "name"
            {
                // Add cell and return empty set
                cell adding = new cell(name, text, text);
                cells.Add(name, adding);

                LinkedList<string> reCalc = (LinkedList<string>)GetCellsToRecalculate(name);
                HashSet<string> result = new HashSet<string>();

                foreach (string s in reCalc)
                    result.Add(s);
                return result;
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, double number)
        {
            // Valid name check
            if (name == null || !isName(name))
                throw new InvalidNameException();

            cell outVal;
            // Check if cell named "name" is in our non-empty cells
            if (cells.TryGetValue(name, out outVal))
            {
                outVal.setContents(number);
                return new HashSet<String>();
            }
            else // Cell is not in "cells"
            {
                // add cell to changed cells and return empty set
                cell adding = new cell(name, number, number);
                cells.Add(name, adding);

                // Return dependent cells
                LinkedList<string> reCalc = (LinkedList<string>)GetCellsToRecalculate(name);
                HashSet<string> result = new HashSet<string>();
                foreach (string s in reCalc)
                    result.Add(s);
                return result;
            }
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            // Error checking
            if (name == null)
                throw new ArgumentNullException();
            if (!isName(name))
                throw new InvalidNameException();

            // Ensure cell "name" is in dependencies
            if (dependencies.HasDependents(name))
            {
                return dependencies.GetDependents(name);
            }
            // Not in dependencies, so has no dependents
            else return new HashSet<string>();
        }

        /// <summary>
        /// Checks if the given input string is a valid cell name.
        /// Valid cell names are defined as strings that consist of a letter 
        /// or underscore followed by zero or more letters, underscores, or digits.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected static bool isName(string input)
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
                    for (int i = 1; i < chars.Length - 1; i++)
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
        /// A private class used to represent the cells of a spreadsheet.
        /// A cell's contents can be a string, a double, or a Formula.
        /// A cell's value can be a string, a double, or a FormulaError.
        /// </summary>
        private class cell
        {
            // The name of the cell
            private String name;
            // The contents and value of a cell
            private Object contents;
            private Object value;

            /// <summary>
            /// Constructor to create a new empty cell 
            /// </summary>
            public cell()
            {
                name = null;
                contents = null;
                value = null;
            }

            /// <summary>
            /// Constructor to create a new empty cell with a givin input name
            /// </summary>
            /// <param name="Name"></param>
            public cell (String Name)
            {
                if (!isName(Name))
                    throw new InvalidNameException();
                name = Name;
                contents = null;
                value = null;
            }

            /// <summary>
            /// A constructor that sets the name, contents and value of the cell.
            /// </summary>
            /// <param name="Name"></param>
            /// <param name="content"></param>
            /// <param name="val"></param>
            public cell(String Name, Object content, Object val)
            {
                if (!isName(Name))
                    throw new InvalidNameException();
                name = Name;
                contents = content;
                value = val;
            }

            /// <summary>
            /// Returns the name of a cell
            /// </summary>
            /// <returns></returns>
            public Object getName()
            {
                return name;
            }

            /// <summary>
            /// Returns the contents of a cell
            /// </summary>
            /// <returns></returns>
            public Object getContents()
            {
                return contents;
            }

            /// <summary>
            /// Returns the value of a cell
            /// </summary>
            /// <returns></returns>
            public Object getValue()
            {
                return value;
            }

            /// <summary>
            /// If Name is a valid cell name, sets the cells name to Name 
            /// and returns true. If not it returns false.
            /// </summary>
            /// <param name="Name"></param>
            /// <returns></returns>
            public bool setName(String Name)
            {
                if (isName(Name))
                {
                    name = Name;
                    return true;
                }
                else return false;
            }

            /// <summary>
            /// Sets the contents of the cell to obj.
            /// If obj is a valid input for contents (string, double, Formula)
            /// the method returns true, and false if otherwise.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public bool setContents(Object obj)
            {
                if (obj is string || obj is double || obj is Formula)
                {
                    contents = obj;
                    return true;
                }
                else return false;
                
            }

            /// <summary>
            /// Sets the value of the cell to obj.
            /// If obj is a valid input for value (string, double, FormulaError)
            /// the method returns true, and false if otherwise.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public bool setValue(Object obj)
            {
                if (obj is string || obj is double || obj is FormulaError)
                {
                    value = obj;
                    return true;
                }
                else return false;
            }
        }
    }
}
