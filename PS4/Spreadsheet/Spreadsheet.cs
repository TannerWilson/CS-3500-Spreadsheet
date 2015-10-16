using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Xml;

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
        /// Used to save the version information of a SpreadSheet
        /// </variable>
        private string version;

        /// <variable>
        /// Used to tell if the document has been changed.
        /// </variable>
        public override bool Changed
        {
            get
            {
                return changed;
            }

            protected set
            {
                
            }
        }

        /// <summary>
        /// Returns the version string of the current spreadsheet.
        /// </summary>
        /// <returns></returns>
        private string getVersion()
        {
            return Version;
        }

        /// <summary>
        /// Constructs an empty SpreadSheet object.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            cells = new Dictionary<string, cell>();
            dependencies = new DependencyGraph();
            changed = false;
        }

        /// <summary>
        /// Constructs an empty SpreadSheet using the base constructor
        /// with the validator "isValid" the normalizer "normalize" and the version "version"
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, cell>();
            dependencies = new DependencyGraph();
            changed = false;
        }

        /// <summary>
        /// Constructs an empty SpreadSheet from a saved XML file "filePath" using the base constructor
        /// with the validator "isValid"the normalizer "normalize" and the version "version"
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(String filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            this.filePath = filePath;
            cells = new Dictionary<string, cell>();
            dependencies = new DependencyGraph();
            changed = false;

            // Used to create the actual cell objects from the input XML
            string name = "";
            string contents = "";
            XmlReader reader = XmlReader.Create(filePath);
            reader.Read();
            reader.Read(); // Step over the first XML version lable to the "spreadsheet" name
            while (reader.Read())
            {
                // Get version info from the start element
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "spreadsheet")
                {
                    this.Version = reader.GetAttribute(0); 
                }
                // Loop into the next element in the reader
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.Read();
                    // Enter the cell element
                    if(reader.Name == "cell")
                    {
                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();
                            // Enter into name element
                            if (reader.Name == "name")
                            {
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    reader.Read();
                                    if (reader.NodeType == XmlNodeType.Text)
                                    {
                                        // Save name info
                                        name = reader.Value;
                                    }
                                }
                                reader.Read();
                            } 
                            // Enter into contents element
                            if(reader.Name == "contents")
                            {
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    reader.Read();
                                    if (reader.NodeType == XmlNodeType.Text)
                                    {
                                        // Save contents info
                                        contents = reader.Value;
                                    }
                                }
                                reader.Read();
                            }
                        }
                        // Name and Contents have each been passed over
                        SetContentsOfCell(name, contents); // Add cell to spreadsheet
                    }
                }
            }
            reader.Close();
        }

        /// <summary>
        /// Implemented as documented in "AbstractSpreadsheet"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            if (name == null || !isName(name))
                throw new InvalidNameException();

            cell outVal;
            // If the cell is in changed cells, return its value
            if (cells.TryGetValue(name, out outVal))
            {
                return outVal.getValue();
            }
            // Empty string is default cell contents
            else return "";
        }


        /// <summary>
        /// Implemented as documented in "AbstractSpreadsheet"
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public override string GetSavedVersion(string filename)
        {
            // Creates a spreadsheet from given filename 
            Spreadsheet sheet = new Spreadsheet(filename, s => true, s => s, "default");

            return sheet.getVersion();
        }

        /// <summary>
        /// Implemented as documented in "AbstractSpreadsheet"
        /// 
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public override void Save(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            try
            {
                XmlWriter writer = XmlWriter.Create(filename, settings);
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", Version); // Add version info to start of file

                // Loop through each changed cell
                foreach (KeyValuePair<string, cell> pair in cells)
                {
                    // Start cell element
                    writer.WriteStartElement("cell");

                    // Write name element
                    writer.WriteStartElement("name");
                    writer.WriteString(pair.Value.getName()); // Get cell and write its name
                    writer.WriteEndElement();

                    // Write contents element
                    writer.WriteStartElement("contents");
                    writer.WriteString(pair.Value.getContents().ToString()); // Get cell, get contents and write it as a string
                    writer.WriteEndElement();

                    // End cell element
                    writer.WriteEndElement();
                }
                // End and close the writer.
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
            catch(Exception)
            {
                throw new SpreadsheetReadWriteException("There was an error writing the XML document");
            }
           
            
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
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
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            // Normalize cell name before handled
            name = Normalize(name);
            // Error checking
            if (content == null)
                throw new ArgumentNullException("Content was null");
            if (name == null || !isName(name) || !IsValid(name))
                throw new InvalidNameException();

            double value;
            // Content is a double
            if (Double.TryParse(content, out value))
            {
                Changed = true;
                return SetCellContents(name, value);
            }

            // Content is a formula
            else if (content.ToCharArray()[0] == '=')
            {
                // Trim off the '=' char
                char[] toTrim = new char[] { '=' };
               content = content.Trim(toTrim);

                Changed = true;
                return SetCellContents(name, new Formula(content));
            }
            else // Content is a string
            {
                Changed = true;
                return SetCellContents(name, content);
            }
                
        }

        /// <summary>
        /// Follows the description from the AbstractSpreadsheet parent class.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            // Check if name is in the changed cells
            cell outVal;
            if (cells.TryGetValue(name, out outVal))
            {
                outVal.setContents(formula);
                outVal.setValue(formula.Evaluate(lookUp));
                LinkedList<string> reCalc = (LinkedList<string>)GetCellsToRecalculate(name);
                HashSet<string> result = new HashSet<string>();

                foreach (string s in reCalc)
                    result.Add(s);
                return result;
            }
            else // No cell named "name" was found
            {
                // Add cell to changed cells
                cell adding = new cell(name, formula, formula.Evaluate(lookUp));
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
            // Check if name is in the changed cells
            cell outVal;
            if (cells.TryGetValue(name, out outVal))
            {
                // Set value and contents
                outVal.setContents(text);
                outVal.setValue(text);

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
            cell outVal;
            // Check if cell named "name" is in our non-empty cells
            if (cells.TryGetValue(name, out outVal))
            {
                // Set contents and value
                outVal.setContents(number);
                outVal.setValue(number);

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
        /// Used to as a lookUp function for the Formula.Evaluate method.
        /// Searches for the cell "cellName" and returns its value as a double.
        /// If the cell has a string or formula error as a value, it returns 0.
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns></returns>
        protected double lookUp(string cellName)
        {
            // Get the value of input cell name
            object value = GetCellValue(cellName);
            // If cell's value is a doiuble, return it
            if (value is double)
                return (double)value;
            // else the cell has no numerical value so its value is 0
            else return 0;
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
            public string getName()
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
               // if (obj is string || obj is double || obj is FormulaError)
               // {
                    value = obj;
                    return true;
                //}
                //else return false;
            }
        }
    }
}
