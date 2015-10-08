using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// This spreadsheet class follows the same documentation ans specification
    /// that is described in the "AbstractSpreadsheet" class.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <variable>
        /// Used to store all cells in the spreadsheet
        /// </variable>
        private Dictionary<string, cell> cells;

        /// <variable>
        /// Used to store all the dependencies in the spreadsheet
        /// </variable>
        private DependencyGraph dependencies;

        /// <variable>
        /// String that holds the path to save the file 
        /// </variable>
        string filePath;

        /// <variable>
        /// Used to report if the document was changed
        /// </variable>
        private Boolean changed;

        /// <variable>
        /// Used to tell if the document has been changed.
        /// </variable>
        public override bool Changed
        {
            get
            {
                throw new NotImplementedException();
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Constructs an empty SpreadSheet object.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {

            cells = new Dictionary<string, cell>();
            dependencies = new DependencyGraph();
        }

        /// <summary>
        /// Implemented as documented in "AbstractSpreadsheet"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implemented as documented in "AbstractSpreadsheet"
        /// </summary>
        /// <param name="filename"></param>
        public override void Save(string filename)
        {
            throw new NotImplementedException();
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
        /// Implemented as documented in "AbstractSpreadsheet"
        /// 
        /// 
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            // Error checking
            if (content == null)
                throw new ArgumentNullException("Content was null");
            if (name == null || !isName(name) || !IsValid(name))
                throw new InvalidNameException();

            double value;
            // Content is a double
            if (Double.TryParse(content, out value))
            {
                return SetCellContents(name, value);
            }

            // Content is a formula
            else if (content.ToCharArray()[0] == '=')
            {

            }
            else // Content is a string
                return SetCellContents(name, content);



            return new HashSet<string>();
        }

        /// <summary>
        /// Follows the description from the AbstractSpreadsheet parent class.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
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
                cells.Add(name, adding);

                // Add name to dependencies and add its varibles as dependents
                dependencies.AddDependency(name, " ");
                HashSet<String> vars = (HashSet<String>) formula.GetVariables();
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
        /// Follows the description from the AbstractSpreadsheet parent class.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
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

                // Return cells to Recalculate
                LinkedList<string> reCalc = (LinkedList<string>)GetCellsToRecalculate(name);
                HashSet<string> result = new HashSet<string>();

                foreach (string s in reCalc)
                    result.Add(s);
                return result;
            }
            else // No cell named "name"
            {
                // Add cell and return empty set
                cell adding = new cell(name, text, text);
                cells.Add(name, adding);

                // Return cells to Recalculate
                LinkedList<string> reCalc = (LinkedList<string>)GetCellsToRecalculate(name);
                HashSet<string> result = new HashSet<string>();

                foreach (string s in reCalc)
                    result.Add(s);
                return result;
            }
        }

        /// <summary>
        /// Follows the description from the AbstractSpreadsheet parent class.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            // Valid name check
            if (name == null || !isName(name))
                throw new InvalidNameException();

            cell outVal;
            // Check if cell named "name" is in our non-empty cells
            if (cells.TryGetValue(name, out outVal))
            {
                outVal.setContents(number);

                // Return cells to Recalculate
                LinkedList<string> reCalc = (LinkedList<string>)GetCellsToRecalculate(name);
                HashSet<string> result = new HashSet<string>();

                foreach (string s in reCalc)
                    result.Add(s);
                return result;
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
        /// Follows the description from the AbstractSpreadsheet parent class.
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

            // Ensure the first character in the string is a letter 
            if (Char.IsLetter(chars[0]))
            {
                // Ensure the last character is a number
                if (Char.IsDigit(chars[chars.Length - 1]))
                {
                    // First and last chars are valid, check validity of all chars
                    for (int i = 1; i < chars.Length - 1; i++)
                    {
                        // Check if each character is NOT a letter, digit, or underscore
                        if (!(Char.IsLetter(chars[i]) || Char.IsDigit(chars[i])))
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
        /// Implemented as documented in "AbstractSpreadsheet"
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public override string GetSavedVersion(string filename)
        {
            throw new NotImplementedException();
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
