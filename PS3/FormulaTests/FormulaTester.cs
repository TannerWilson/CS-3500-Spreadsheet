using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtilities.Tests
{
    [TestClass()]
    public class FormulaTests
    {
        [TestMethod()]
        public void Public_ConstructorTest1()
        {
            Formula test = new Formula("x1+ y4  ");
            Assert.AreEqual("x1+y4", test.ToString());

            HashSet<string> result = (HashSet<String>)test.GetVariables();
            Assert.IsTrue(result.Contains("x1"));
            Assert.IsTrue(result.Contains("y4"));

            //CollectionAssert.AreEqual(new List<String>() { "x1", "y4" }, (List<String>)test.GetVariables());
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Public_ConstructorTest2()
        {
            Formula test = new Formula("(8a - M1  ");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Public_ConstructorTest3()
        {
            Formula test = new Formula("x1 - Y2 + 1bn");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Public_ConstructorTest4()
        {
            Formula test = new Formula("");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Public_ConstructorTest5()
        {
            Formula test = new Formula(" 2 + 5) ");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Public_ConstructorTest6()
        {
            Formula test = new Formula(" (((2 + 5)-2) + 2 ");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Public_ConstructorTest7()
        {
            Formula test = new Formula("- X1 + 2");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Public_ConstructorTest8()
        {
            Formula test = new Formula("X1 + 2 *");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Public_ConstructorTest9()
        {
            Formula test = new Formula("(-) + 2");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Public_ConstructorTest10()
        {
            Formula test = new Formula("(2(3 + ) ");
        }

        [TestMethod()]
        public void Public_SpecialConstructorTest1()
        {
            Formula test = new Formula("x1+ y4  ", toUp, s =>true);
            Assert.AreEqual("X1+Y4", test.ToString());
            HashSet<string> result = (HashSet<String>)test.GetVariables();
            Assert.IsTrue(result.Contains("X1"));
            Assert.IsTrue(result.Contains("Y4"));

            //CollectionAssert.AreEqual(new HashSet<string>() { "X1", "Y4" }, (HashSet<String>) test.GetVariables());
        }

        [TestMethod()]
        public void Public_EvaluateTest1()
        {
            Formula test = new Formula("a4-a4*a4/a4");
            Double result = (Double) test.Evaluate(s => 3);
            Assert.IsTrue(result == 0); 
        }

        [TestMethod()]
        public void Public_EvaluateTest2()
        {
            Formula test = new Formula("2*(3+5)");
            Double result = (Double)test.Evaluate(s => 0);
            Assert.IsTrue(result == 16);  
        }

        [TestMethod()]
        public void Public_EvaluateTest3()
        {
            Formula test = new Formula("2+(3+5*5)");
            Double result = (Double)test.Evaluate(s => 0);
            Assert.IsTrue(result == 30);
        }

        [TestMethod()]
        public void Public_EvaluateTest4()
        {
            Formula test = new Formula("2+3*5+(3+4*8)*5+2");
            Double result = (Double)test.Evaluate(s => 0);
            Assert.IsTrue(result == 194);
        }

        [TestMethod()]
        public void Public_EvaluateTest5()
        {
            Formula test = new Formula("2*5");
            Double result = (Double)test.Evaluate(s => 0);
            Assert.IsTrue(result == 10);
        }

        [TestMethod()]
        public void Public_EvaluateTest6()
        {
            Formula test = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Double result = (Double)test.Evaluate(s => 2);
            Assert.IsTrue(result == 12);
        }

        [TestMethod()]
        public void Public_EvaluateTest7()
        {
            Formula test = new Formula("2/0");
            object result = test.Evaluate(s => 0);
            Assert.IsTrue(result is FormulaError);
        }

        [TestMethod()]
        public void Public_EvaluateTest8()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Public_EvaluateTest9()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Public_GetVariablesTest1()
        {
            Formula test = new Formula("x1+ y4  ", toUp, s => true);
            HashSet<string> result = (HashSet<String>)test.GetVariables();
            Assert.IsTrue(result.Contains("X1"));
            Assert.IsTrue(result.Contains("Y4"));

            //CollectionAssert.AreEqual(new List<String>() { "X1", "Y4" }, (List<String>)test.GetVariables());
        }

        [TestMethod()]
        public void Public_GetVariablesTest2()
        {
            Formula test = new Formula("x1+ y4  " , toUp, s => true);
            HashSet<string> result = (HashSet<String>)test.GetVariables();
            Assert.IsTrue(result.Contains("X1"));
            Assert.IsTrue(result.Contains("Y4"));

            //CollectionAssert.AreEqual(new List<String>() { "X1", "Y4" }, (List<String>)test.GetVariables());
        }

        [TestMethod()]
        public void Public_GetVariablesTest3()
        {
            Formula test = new Formula("Y_1 * x + X - Y1");
            HashSet<string> result = (HashSet<String>)test.GetVariables();
            Assert.IsTrue(result.Contains("Y_1"));
            Assert.IsTrue(result.Contains("X"));
            Assert.IsTrue(result.Contains("Y1"));
            
            //CollectionAssert.AreEqual(new List<String>() { "Y_1", "x", "X", "Y1" }, (List<String>)test.GetVariables());
        }

        [TestMethod()]
        public void Public_ToStringTest1()
        {
            Formula form1 = new Formula("x1+ y4  ");
            Assert.AreEqual("x1+y4", form1.ToString());

        }

        [TestMethod()]
        public void Public_ToStringTest2()
        {
            Formula test = new Formula("x * Y +(b* T) ", toUp, s=>true);
            Assert.AreEqual("X*Y+(B*T)", test.ToString());
        }

        [TestMethod()]
        public void Public_ToStringTest3()
        {
            Formula test = new Formula("(((_x*y)+2)/   4) - test_varible  ");
            Assert.AreEqual("(((_x*y)+2)/4)-test_varible", test.ToString());
        }

        [TestMethod()]
        public void Public_EqualsTest1()
        {
            Formula test1 = new Formula("X1+2-Y");
            Formula test2 = new Formula("X1+2-Y");
            Formula test3 = new Formula("X1 +   2 - Y");
            Formula test4 = new Formula("2 + 5");

            Assert.IsTrue(test1.Equals(test2));
            Assert.IsTrue(test1.Equals(test3));
            Assert.IsTrue(test2.Equals(test3));

            Assert.IsFalse(test1.Equals(test4));
            Assert.IsFalse(test3.Equals(test4));
        }

        [TestMethod()]
        public void Public_GetHashCodeTest()
        {
            Formula test1 = new Formula("2+x+y");
            Formula test2 = new Formula("2+x+y");
            Formula test3 = new Formula("2+m+y");
            Formula test4 = new Formula("3 + 5");
            Formula test5 = new Formula("2+x+o");
            Formula test6 = new Formula("2+t+q");

            Formula test7 = new Formula("2.0500000 + 3");
            Formula test8 = new Formula("2.05 + 3");
            Assert.IsTrue(test7.GetHashCode() == test8.GetHashCode());

            Assert.IsTrue(test1.GetHashCode() == test2.GetHashCode());
            Assert.IsFalse(test1.GetHashCode() == test3.GetHashCode());
            Assert.IsFalse(test4.GetHashCode() == test2.GetHashCode());
            Assert.IsFalse(test1.GetHashCode() == test5.GetHashCode());
            Assert.IsFalse(test1.GetHashCode() == test6.GetHashCode());
        }

        /// <summary>
        /// Used as a normalizer function to test the second constructor
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string toUp (String s)
        {
            s = s.ToUpper();
            return s;
        }
    }
}