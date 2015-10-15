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
        public void SetContentsOfCellTest1()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("A1", "contents");
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsTrue((string) test.GetCellContents("A1") == "contents");

            test.SetContentsOfCell("A1", "next");
            Assert.IsTrue((string)test.GetCellContents("A1") == "next");
            
        }

        [TestMethod()]
        public void SetContentsOfCellTest2()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("A1", "= X1 + Y1");
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsTrue(test.GetCellContents("A1").ToString() == "X1+Y1");

            test.SetContentsOfCell("A1", "=2+2");
            Assert.IsTrue(test.GetCellContents("A1").ToString() == "2+2");

        }

        [TestMethod()]
        public void SetContentsOfCellTest3()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("A1", "1.03");
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsTrue((double)test.GetCellContents("A1") == 1.03);

            test.SetContentsOfCell("A1", "130.787");
            Assert.IsTrue((double)test.GetCellContents("A1") == 130.787);

        }

        [TestMethod()]
        public void SetContentsOfCellTest4()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("A1", "text");
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue((string) test.GetCellContents("A1") == "text");
            Assert.IsTrue(result.Contains("A1"));


        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTest5()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("1*23A", "= 1+1");

        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTest6()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("&", "2.31");

        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTest7()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell(null, "2.43");

        }

        [TestMethod()]
        public void SetContentsOfCellTest8()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("B4", "2.56");
            Assert.IsTrue((double) test.GetCellContents("B4") == 2.56);

        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTest9()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("+", "contents");

        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void GetNamesOfAllNonemptyCellsTest1()
        {
            Spreadsheet test = new Spreadsheet();
            // populate the test spread sheet
            test.SetContentsOfCell("A1", "=1+1");
            test.SetContentsOfCell("A2", "1+1");
            test.SetContentsOfCell("A3", "134.6554");
            test.SetContentsOfCell("A4", "=A4+A1"); // A4 dpends on A4, circular
            test.SetContentsOfCell("A5", "= C2+B5");
            test.SetContentsOfCell("A6", "test");
            test.SetContentsOfCell("A7", "2.54");
            
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
            test.SetContentsOfCell("A1", " =1+1");
            test.SetContentsOfCell("A2", "1+1");
            test.SetContentsOfCell("A3", "134.6554");
            test.SetContentsOfCell("A4", "=f4+m1");
            test.SetContentsOfCell("A5", "=C2+B5");
            test.SetContentsOfCell("A6", "test");
            test.SetContentsOfCell("A7", "2.54");

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
            test.SetContentsOfCell("A1", "contents");
            Assert.IsTrue((string)test.GetCellContents("A1") == "contents");

        }

        [TestMethod()]
        public void GetCellContentsTest1()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("A1", "= X1 + Y1");
            Assert.IsTrue((string)test.GetCellContents("A1").ToString() == "X1+Y1");

        }

        [TestMethod()]
        public void GetCellContentsTest2()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("A1", "1.03");
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