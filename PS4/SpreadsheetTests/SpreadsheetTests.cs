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
        public void GetCellContentsTest1()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", "contents");
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsTrue((string) test.GetCellContents("A1") == "contents");
            
        }

        [TestMethod()]
        public void GetCellContentsTest2()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", new Formula("X1 + Y1"));
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsTrue((string)test.GetCellContents("A1") == "X1 + Y1");
            
        }

        [TestMethod()]
        public void GetCellContentsTest3()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", 1);
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue(result.Contains("A1"));
            Assert.IsTrue((double)test.GetCellContents("A1") == 1);


        }

        [TestMethod()]
        public void GetCellContentsTest4()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", "text");
            LinkedList<string> result = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            Assert.IsTrue((string) test.GetCellContents("A1") == "text");
            Assert.IsTrue(result.Contains("A1"));


        }

        //[TestMethod()]
        //public void GetCellContentsTest5()
        //{
        //    Spreadsheet test = new Spreadsheet();
        //    test.SetCellContents()

        //}

        //[TestMethod()]
        //public void GetCellContentsTest6()
        //{
        //    Spreadsheet test = new Spreadsheet();
        //    test.SetCellContents()

        //}

        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetCellContentsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetCellContentsTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetCellContentsTest2()
        {
            Assert.Fail();
        }
    }
}