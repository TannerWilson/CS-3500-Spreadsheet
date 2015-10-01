using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace SS.Tests
{
    [TestClass()]
    public class SpreadsheetTests
    {
        [TestMethod()]
        public void SpreadsheetTest1()
        {
            Spreadsheet test = new Spreadsheet();
           LinkedList<string> result = (LinkedList<string>) test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(result.Count == 0);
        }

        [TestMethod()]
        public void SetCellContentsTest1()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", "contents");
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsTrue((string) test.GetCellContents("A1") == "contents");

            test.SetCellContents("A1", "next");
            Assert.IsTrue((string)test.GetCellContents("A1") == "next");
            
        }

        [TestMethod()]
        public void SetCellContentsTest2()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", new Formula("X1 + Y1"));
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsTrue(test.GetCellContents("A1").ToString() == "X1+Y1");

            test.SetCellContents("A1", new Formula("2+2"));
            Assert.IsTrue(test.GetCellContents("A1").ToString() == "2+2");

        }

        [TestMethod()]
        public void SetCellContentsTest3()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", 1.03);
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsTrue((double)test.GetCellContents("A1") == 1.03);

            test.SetCellContents("A1", 130.787);
            Assert.IsTrue((double)test.GetCellContents("A1") == 130.787);

        }

        [TestMethod()]
        public void SetCellContentsTest4()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", "text");
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue((string) test.GetCellContents("A1") == "text");
            Assert.IsTrue(result.Contains("A1"));


        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsTest5()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("1*23A", new Formula("1+1"));

        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsTest6()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("&", 2.31);

        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsTest7()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents(null, 2.43);

        }

        [TestMethod()]
        public void SetCellContentsTest8()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("____b", 2.56);
            Assert.IsTrue((double) test.GetCellContents("____b") == 2.56);

        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsTest9()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("+", "contents");

        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void GetNamesOfAllNonemptyCellsTest1()
        {
            Spreadsheet test = new Spreadsheet();
            // populate the test spread sheet
            test.SetCellContents("A1", new Formula("1+1"));
            test.SetCellContents("A2", "1+1");
            test.SetCellContents("A3", 134.6554);
            test.SetCellContents("A4", new Formula("A4+A1")); // A4 dpends on A4, circular
            test.SetCellContents("A5", new Formula("C2+B5"));
            test.SetCellContents("A6", "test");
            test.SetCellContents("A7", 2.54);
            
            // populate test result list
            LinkedList<string> expected = new LinkedList<string>();
            expected.AddLast("A1");
            expected.AddLast("A2");
            expected.AddLast("A3");
            expected.AddLast("A4");
            expected.AddLast("A5");
            expected.AddLast("A6");
            expected.AddLast("A7");

            LinkedList<string> cells = (LinkedList<string>) test.GetNamesOfAllNonemptyCells();
            CollectionAssert.AreEqual(expected, cells);
        }

        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest2()
        {
            Spreadsheet test = new Spreadsheet();
            // populate the test spread sheet
            test.SetCellContents("A1", new Formula("1+1"));
            test.SetCellContents("A2", "1+1");
            test.SetCellContents("A3", 134.6554);
            test.SetCellContents("A4", new Formula("f4+m1"));
            test.SetCellContents("A5", new Formula("C2+B5"));
            test.SetCellContents("A6", "test");
            test.SetCellContents("A7", 2.54);

            // populate test result list
            LinkedList<string> expected = new LinkedList<string>();
            expected.AddLast("A1");
            expected.AddLast("A2");
            expected.AddLast("A3");
            expected.AddLast("A4");
            expected.AddLast("A5");
            expected.AddLast("A6");
            expected.AddLast("A7");

            LinkedList<string> cells = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            CollectionAssert.AreEqual(expected, cells);
        }

        [TestMethod()]
        public void GetCellContentsTest()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", "contents");
            Assert.IsTrue((string)test.GetCellContents("A1") == "contents");

        }

        [TestMethod()]
        public void GetCellContentsTest1()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", new Formula("X1 + Y1"));
            Assert.IsTrue((string)test.GetCellContents("A1").ToString() == "X1+Y1");

        }

        [TestMethod()]
        public void GetCellContentsTest2()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", 1.03);
            Assert.IsTrue((double)test.GetCellContents("A1") == 1.03);

        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest3()
        {
            Spreadsheet test = new Spreadsheet();
            test.GetCellContents("1B");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest4()
        {
            Spreadsheet test = new Spreadsheet();
            test.GetCellContents(null);
        }
    }
}