﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            CollectionAssert.AreEqual(new List<String>() { "x1", "y4" }, (List<String>)test.GetVariables());
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
            Assert.AreEqual("x1+y4", test.ToString());
            CollectionAssert.AreEqual(new List<String>() { "X1", "Y4" }, (List<String>)test.GetVariables());
        }

        [TestMethod()]
        public void Public_EvaluateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Public_GetVariablesTest1()
        {
            Formula test = new Formula("x1+ y4  ", toUp, s => true);
            CollectionAssert.AreEqual(new List<String>() { "X1", "Y4" }, (List<String>)test.GetVariables());
        }

        [TestMethod()]
        public void Public_GetVariablesTest2()
        {
            Formula test = new Formula("x1+ y4  ");
            CollectionAssert.AreEqual(new List<String>() { "X1", "Y4" }, (List<String>)test.GetVariables());
        }

        [TestMethod()]
        public void Public_GetVariablesTest3()
        {
            Formula test = new Formula("Y_1 * x + X - Y1");
            CollectionAssert.AreEqual(new List<String>() { "Y_1", "x", "X", "Y1" }, (List<String>)test.GetVariables());
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
        public void Public_EqualsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            Formula test1 = new Formula("2+x+y");
            Formula test2 = new Formula("2+x+y");
            Formula test3 = new Formula("2+m+y");
            Formula test4 = new Formula("3 + 5");
            Formula test5 = new Formula("2+x+o");
            Formula test6 = new Formula("2+t+q");

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