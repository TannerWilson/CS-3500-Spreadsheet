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
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContentsOfCellTest10()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("+", (string) null);

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
            test.SetContentsOfCell("A5", "= C2+B5");
            test.SetContentsOfCell("A6", "test");
            test.SetContentsOfCell("A7", "2.54");
            test.SetContentsOfCell("A4", "=A4+A1"); // A4 dpends on A4, circular

        }

        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest2()
        {
            Spreadsheet test = new Spreadsheet();
            // populate the test spread sheet
            test.SetContentsOfCell("A1", " =1+1");
            test.SetContentsOfCell("A2", "1+1");
            test.SetContentsOfCell("A3", "134.6554");
            test.SetContentsOfCell("A4567", "=f4+m1");
            test.SetContentsOfCell("A5", "=C2+B5");
            test.SetContentsOfCell("A6", "test");
            test.SetContentsOfCell("A7", "2.54");

            // populate test result list
            LinkedList<string> expected = new LinkedList<string>();
            expected.AddLast("A1");
            expected.AddLast("A2");
            expected.AddLast("A3");
            expected.AddLast("A4567");
            expected.AddLast("A5");
            expected.AddLast("A6");
            expected.AddLast("A7");

            LinkedList<string> cells = (LinkedList<string>)test.GetNamesOfAllNonemptyCells();
            CollectionAssert.AreEqual(expected, cells);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNamesOfAllNonemptyCellsTest3()
        {
            Spreadsheet test = new Spreadsheet();
            // populate the test spread sheet
           
            test.SetContentsOfCell("A2", "1+1");
            test.SetContentsOfCell("A3", "134.6554");
            test.SetContentsOfCell("A4", "=M4+B3"); // A4 dpends on A4, circular
            test.SetContentsOfCell("A5", "= C2+B5");
            test.SetContentsOfCell("A6", "test");
            test.SetContentsOfCell("A7", "2.54");
           
            // Exception throwing code
            test.SetContentsOfCell(null, (string)null);
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

        [TestMethod()]
        public void SaveTest1()
        {
            Spreadsheet test = new Spreadsheet();
            // populate the test spread sheet
            test.SetContentsOfCell("A1", "=1+1");
            test.SetContentsOfCell("A2", "1+1");
            test.SetContentsOfCell("A3", "134.6554");
            test.SetContentsOfCell("A4", "=f4+m1");
            test.SetContentsOfCell("A5", "=C2+B5");
            test.SetContentsOfCell("A6", "test");
            test.SetContentsOfCell("A7", "2.54");
            test.Save("spreadsheet_test1");

            // populate test result list
            LinkedList<string> expected = new LinkedList<string>();
            expected.AddLast("A1");
            expected.AddLast("A2");
            expected.AddLast("A3");
            expected.AddLast("A4");
            expected.AddLast("A5");
            expected.AddLast("A6");
            expected.AddLast("A7");

            // Create a new spreadsheet from the previously saved file
            Spreadsheet result = new Spreadsheet("spreadsheet_test1", s => true, s => s, "default");

            // Should be the same spreadsheet as before
            LinkedList<string> cells = (LinkedList<string>)result.GetNamesOfAllNonemptyCells();
            CollectionAssert.AreEqual(expected, cells);
        }

        [TestMethod()]
        public void SaveTest2()
        {
            Spreadsheet test = new Spreadsheet();
            // populate the test spread sheet
            test.SetContentsOfCell("B1", "=1+1");
            
            test.Save("spreadsheet_test2");

            // populate test result list
            LinkedList<string> expected = new LinkedList<string>();
            expected.AddLast("B1");

            // Create a new spreadsheet from the previously saved file
            Spreadsheet result = new Spreadsheet("spreadsheet_test2", s => true, s => s, "default");

            // Should be the same spreadsheet as before
            LinkedList<string> cells = (LinkedList<string>)result.GetNamesOfAllNonemptyCells();
            CollectionAssert.AreEqual(expected, cells);
        }

        [TestMethod()]
        public void GetCellValueTest1()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("A1", "=1+1");
            test.SetContentsOfCell("A2", "1+1");
            test.SetContentsOfCell("A3", "5");
            test.SetContentsOfCell("A4", "=5+1");
            test.SetContentsOfCell("A5", "10");
            test.SetContentsOfCell("A6", "test");
            test.SetContentsOfCell("A7", "2.54");
            test.SetContentsOfCell("A8", "=A1+A3");
            test.SetContentsOfCell("A9", "=A3+A1+A4+A8"); // == 5+2+6+7 = 20
            test.SetContentsOfCell("B1", "=2/0");

            Assert.IsTrue((double) test.GetCellValue("A1") == 2.0);
            Assert.IsTrue((string)test.GetCellValue("A2") == "1+1");
            Assert.IsTrue((double)test.GetCellValue("A3") == 5.0);
            Assert.IsTrue((double)test.GetCellValue("A4") == 6.0);
            Assert.IsTrue((double)test.GetCellValue("A5") == 10.0);
            Assert.IsTrue((string)test.GetCellValue("A6") == "test");
            Assert.IsTrue((double)test.GetCellValue("A7") == 2.54);
            Assert.IsTrue((double)test.GetCellValue("A8") == 7.0);
            Assert.IsTrue((double)test.GetCellValue("A9") == 20.0);
            Assert.IsTrue(test.GetCellValue("B1") is FormulaError);

        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValueTest2()
        {
            Spreadsheet test = new Spreadsheet();
            test.GetCellValue("--*&");
        }

        [TestMethod()]
        public void GetSavedVersionTest()
        {
            Spreadsheet test = new Spreadsheet();
            Assert.IsTrue("default" == test.GetSavedVersion("spreadsheet_test1") );

        }
    }
}