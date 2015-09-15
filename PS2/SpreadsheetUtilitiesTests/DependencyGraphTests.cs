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
    public class DependencyGraphTests
    {
        [TestMethod()]
        public void DependencyGraphTest()
        {
            DependencyGraph test = new DependencyGraph();
            Assert.IsNotNull(test);
            Assert.IsTrue(test.Size == 0);
        }

        [TestMethod()]
        public void HasDependentsTest()
        {
            DependencyGraph test = new DependencyGraph();
            Assert.IsFalse(test.HasDependents("a"));
            Assert.IsFalse(test.HasDependents("b"));
            Assert.IsFalse(test.HasDependents("c"));

            // Testing adding

            test.AddDependency("a", "t");
            Assert.IsTrue(test.HasDependents("a"));
            Assert.IsFalse(test.HasDependents("b"));
            Assert.IsFalse(test.HasDependents("c"));

            test.AddDependency("b", "t");
            Assert.IsTrue(test.HasDependents("a"));
            Assert.IsTrue(test.HasDependents("b"));
            Assert.IsFalse(test.HasDependents("c"));

            test.AddDependency("c", "t");
            Assert.IsTrue(test.HasDependents("a"));
            Assert.IsTrue(test.HasDependents("b"));
            Assert.IsTrue(test.HasDependents("c"));

            // Testing removing

            //test.RemoveDependency("a", "t");
            //Assert.IsFalse(test.HasDependents("a"));
            //Assert.IsTrue(test.HasDependents("b"));
            //Assert.IsTrue(test.HasDependents("c"));

            //test.RemoveDependency("b", "t");
            //Assert.IsFalse(test.HasDependents("a"));
            //Assert.IsFalse(test.HasDependents("b"));
            //Assert.IsTrue(test.HasDependents("c"));

            //test.RemoveDependency("c", "t");
            //Assert.IsFalse(test.HasDependents("a"));
            //Assert.IsFalse(test.HasDependents("b"));
            //Assert.IsFalse(test.HasDependents("c"));
        }

        [TestMethod()]
        public void HasDependeesTest()
        {
            // Assert there are no dependees in test
            DependencyGraph test = new DependencyGraph();
            Assert.IsFalse(test.HasDependees("a"));
            Assert.IsFalse(test.HasDependees("b"));
            Assert.IsFalse(test.HasDependees("c"));

            // Testing adding

            // Add one dependency and check if the add and reporting worked
            test.AddDependency("t", "a");
            Assert.IsTrue(test.HasDependees("a"));
            Assert.IsFalse(test.HasDependees("b"));
            Assert.IsFalse(test.HasDependees("c"));

            // Add one dependency and check if the add and reporting worked
            test.AddDependency("t", "b");
            Assert.IsTrue(test.HasDependees("a"));
            Assert.IsTrue(test.HasDependees("b"));
            Assert.IsFalse(test.HasDependees("c"));

            // Add one dependency and check if the add and reporting worked
            test.AddDependency("t", "c");
            Assert.IsTrue(test.HasDependees("a"));
            Assert.IsTrue(test.HasDependees("b"));
            Assert.IsTrue(test.HasDependees("c"));

            // Testing removing

            // Remove one dependency and check if the remove and reporting worked
            test.RemoveDependency("t", "a");
            Assert.IsFalse(test.HasDependees("a"));
            Assert.IsTrue(test.HasDependees("b"));
            Assert.IsTrue(test.HasDependees("c"));

            // Remove one dependency and check if the remove and reporting worked
            test.RemoveDependency("t", "b");
            Assert.IsFalse(test.HasDependees("a"));
            Assert.IsFalse(test.HasDependees("b"));
            Assert.IsTrue(test.HasDependees("c"));

            // Remove one dependency and check if the remove and reporting worked
            test.RemoveDependency("t", "c");
            Assert.IsFalse(test.HasDependees("a"));
            Assert.IsFalse(test.HasDependees("b"));
            Assert.IsFalse(test.HasDependees("c"));
        }

        [TestMethod()]
        public void GetDependentsTest()
        {
            DependencyGraph test = new DependencyGraph();

            // Populate the expected results
            LinkedList<String> correctS = new LinkedList<String>();
            correctS.AddLast("a");
            correctS.AddLast("b");
            correctS.AddLast("c");
            correctS.AddLast("d");

            LinkedList<String> wrongS = new LinkedList<String>();
            wrongS.AddLast("a");
            wrongS.AddLast("b");
            wrongS.AddLast("c");
            wrongS.AddLast("d");
            wrongS.AddLast("e");

            LinkedList<String> correctT = new LinkedList<String>();
            correctT.AddLast("a");
            correctT.AddLast("s");
            correctT.AddLast("d");
            correctT.AddLast("f");

            // Populate the graph
            test.AddDependency("s", "a");
            test.AddDependency("s", "b");
            test.AddDependency("s", "c");
            test.AddDependency("s", "d");

            test.AddDependency("t", "a");
            test.AddDependency("t", "s");
            test.AddDependency("t", "d");
            test.AddDependency("t", "f");
            LinkedList<String> resultS = (LinkedList<String>) test.GetDependents("s");
            LinkedList<String> resultT = (LinkedList<String>)test.GetDependents("t");

            // Run asserts
            CollectionAssert.AreEqual(correctS, resultS);
            CollectionAssert.AreEqual(correctT, resultT);
            CollectionAssert.AreNotEqual(wrongS, resultS);
        }

        [TestMethod()]
        public void GetDependeesTest()
        {
            DependencyGraph test = new DependencyGraph();

            // Populate the graph
            test.AddDependency("s", "a");
            test.AddDependency("s", "b");
            test.AddDependency("s", "c");
            test.AddDependency("s", "d");

            test.AddDependency("t", "a");
            test.AddDependency("t", "s");
            test.AddDependency("t", "d");
            test.AddDependency("t", "f");

            // Populate the expected results
            LinkedList<String> correctD = new LinkedList<String>();
            correctD.AddLast("s");
            correctD.AddLast("t");

            LinkedList<String> correctA = new LinkedList<String>();
            correctA.AddLast("s");
            correctA.AddLast("t");

            LinkedList<String> correctB = new LinkedList<String>();
            correctB.AddLast("s");

            LinkedList<String> correctS = new LinkedList<String>();
            correctS.AddLast("t");

            // Run method to make testable lists
            LinkedList<String> resultD = (LinkedList<String>)test.GetDependees("d");
            LinkedList<String> resultA = (LinkedList<String>)test.GetDependees("a");
            LinkedList<String> resultB = (LinkedList<String>)test.GetDependees("b");
            LinkedList<String> resultS = (LinkedList<String>)test.GetDependees("s");

            // Run asserts
            CollectionAssert.AreEqual(correctD, resultD);
            CollectionAssert.AreEqual(correctA, resultA);
            CollectionAssert.AreEqual(correctB, resultB);
            CollectionAssert.AreEqual(correctS, resultS);

        }

        [TestMethod()]
        public void AddDependencyTest()
        {
            // Assert there are no dependees in test
            DependencyGraph test = new DependencyGraph();
            Assert.IsFalse(test.HasDependees("a"));
            Assert.IsFalse(test.HasDependees("b"));
            Assert.IsFalse(test.HasDependees("c"));

            // Testing adding

            // Check dependents
            Assert.IsFalse(test.HasDependents("a"));
            Assert.IsFalse(test.HasDependents("b"));
            Assert.IsFalse(test.HasDependents("c"));

            test.AddDependency("a", "t");
            Assert.IsTrue(test.HasDependents("a"));
            Assert.IsFalse(test.HasDependents("b"));
            Assert.IsFalse(test.HasDependents("c"));

            test.AddDependency("b", "t");
            Assert.IsTrue(test.HasDependents("a"));
            Assert.IsTrue(test.HasDependents("b"));
            Assert.IsFalse(test.HasDependents("c"));

            test.AddDependency("c", "t");
            Assert.IsTrue(test.HasDependents("a"));
            Assert.IsTrue(test.HasDependents("b"));
            Assert.IsTrue(test.HasDependents("c"));
            
            // Check dependees
            // Add one dependency and check if the add and reporting worked
            test.AddDependency("t", "a");
            Assert.IsTrue(test.HasDependees("a"));
            Assert.IsFalse(test.HasDependees("b"));
            Assert.IsFalse(test.HasDependees("c"));

            // Add one dependency and check if the add and reporting worked
            test.AddDependency("t", "b");
            Assert.IsTrue(test.HasDependees("a"));
            Assert.IsTrue(test.HasDependees("b"));
            Assert.IsFalse(test.HasDependees("c"));

            // Add one dependency and check if the add and reporting worked
            test.AddDependency("t", "c");
            Assert.IsTrue(test.HasDependees("a"));
            Assert.IsTrue(test.HasDependees("b"));
            Assert.IsTrue(test.HasDependees("c"));

            // Populate the graph
            test.AddDependency("s", "a");
            test.AddDependency("s", "b");
            test.AddDependency("s", "c");
            test.AddDependency("s", "d");

            test.AddDependency("t", "a");
            test.AddDependency("t", "s");
            test.AddDependency("t", "d");
            test.AddDependency("t", "f");

            Assert.IsTrue(test.HasDependents("s"));
            Assert.IsTrue(test.HasDependents("t"));
        }

        [TestMethod()]
        public void RemoveDependencyTest()
        {
            // Assert there are no dependees in test
            DependencyGraph test = new DependencyGraph();
            Assert.IsFalse(test.HasDependees("a"));
            Assert.IsFalse(test.HasDependees("b"));
            Assert.IsFalse(test.HasDependees("c"));

            test.AddDependency("t", "a");
            test.AddDependency("t", "b");
            test.AddDependency("t", "c");
            
            // Testing removing

            // Remove one dependency and check if the remove and reporting worked
            test.RemoveDependency("t", "a");
            Assert.IsFalse(test.HasDependees("a"));
            Assert.IsTrue(test.HasDependees("b"));
            Assert.IsTrue(test.HasDependees("c"));

            // Remove one dependency and check if the remove and reporting worked
            test.RemoveDependency("t", "b");
            Assert.IsFalse(test.HasDependees("a"));
            Assert.IsFalse(test.HasDependees("b"));
            Assert.IsTrue(test.HasDependees("c"));

            // Remove one dependency and check if the remove and reporting worked
            test.RemoveDependency("t", "c");
            Assert.IsFalse(test.HasDependees("a"));
            Assert.IsFalse(test.HasDependees("b"));
            Assert.IsFalse(test.HasDependees("c"));
        }

        [TestMethod()]
        public void ReplaceDependentsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReplaceDependeesTest()
        {
            Assert.Fail();
        }
    }


    /// <summary>
    ///  This is a test class for DependencyGraphTest
    /// 
    ///  These tests should help guide you on your implementation.  Warning: you can not "test" yourself
    ///  into correctness.  Tests only show incorrectness.  That being said, a large test suite will go a long
    ///  way toward ensuring correctness.
    /// 
    ///  You are strongly encouraged to write additional tests as you think about the required
    ///  functionality of yoru library.
    /// 
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {
        // ************************** TESTS ON EMPTY DGs ************************* //

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest1()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest2()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsFalse(t.HasDependees("a"));
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest3()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsFalse(t.HasDependents("a"));
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest4()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsFalse(t.GetDependees("a").GetEnumerator().MoveNext());
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest5()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.IsFalse(t.GetDependents("a").GetEnumerator().MoveNext());
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyTest6()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t["a"]);
        }

        /// <summary>
        ///Removing from an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void EmptyTest7()
        {
            DependencyGraph t = new DependencyGraph();
            t.RemoveDependency("a", "b");
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        ///Adding an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void EmptyTest8()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
        }

        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void EmptyTest9()
        {
            DependencyGraph t = new DependencyGraph();
            t.ReplaceDependents("a", new HashSet<string>());
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void EmptyTest10()
        {
            DependencyGraph t = new DependencyGraph();
            t.ReplaceDependees("a", new HashSet<string>());
            Assert.AreEqual(0, t.Size);
        }


        /**************************** SIMPLE NON-EMPTY TESTS ****************************/

        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest1()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            Assert.AreEqual(2, t.Size);
        }

        /// <summary>
        ///Slight variant
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest2()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "b");
            Assert.AreEqual(1, t.Size);
        }

        /// <summary>
        ///Nonempty graph should contain something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest3()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("d", "c");
            Assert.IsFalse(t.HasDependees("a"));
            Assert.IsTrue(t.HasDependees("b"));
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsTrue(t.HasDependees("c"));
        }

        /// <summary>
        ///Nonempty graph should contain something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest4()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("d", "c");
            HashSet<String> aDents = new HashSet<String>(t.GetDependents("a"));
            HashSet<String> bDents = new HashSet<String>(t.GetDependents("b"));
            HashSet<String> cDents = new HashSet<String>(t.GetDependents("c"));
            HashSet<String> dDents = new HashSet<String>(t.GetDependents("d"));
            HashSet<String> eDents = new HashSet<String>(t.GetDependents("e"));
            HashSet<String> aDees = new HashSet<String>(t.GetDependees("a"));
            HashSet<String> bDees = new HashSet<String>(t.GetDependees("b"));
            HashSet<String> cDees = new HashSet<String>(t.GetDependees("c"));
            HashSet<String> dDees = new HashSet<String>(t.GetDependees("d"));
            HashSet<String> eDees = new HashSet<String>(t.GetDependees("e"));
            Assert.IsTrue(aDents.Count == 2 && aDents.Contains("b") && aDents.Contains("c"));
            Assert.IsTrue(bDents.Count == 0);
            Assert.IsTrue(cDents.Count == 0);
            Assert.IsTrue(dDents.Count == 1 && dDents.Contains("c"));
            Assert.IsTrue(eDents.Count == 0);
            Assert.IsTrue(aDees.Count == 0);
            Assert.IsTrue(bDees.Count == 1 && bDees.Contains("a"));
            Assert.IsTrue(cDees.Count == 2 && cDees.Contains("a") && cDees.Contains("d"));
            Assert.IsTrue(dDees.Count == 0);
            Assert.IsTrue(dDees.Count == 0);
        }

        /// <summary>
        ///Nonempty graph should contain something
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest5()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("d", "c");
            Assert.AreEqual(0, t["a"]);
            Assert.AreEqual(1, t["b"]);
            Assert.AreEqual(2, t["c"]);
            Assert.AreEqual(0, t["d"]);
            Assert.AreEqual(0, t["e"]);
        }

        /// <summary>
        ///Removing from a DG 
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest6()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("d", "c");
            t.RemoveDependency("a", "b");
            Assert.AreEqual(2, t.Size);
        }

        /// <summary>
        ///Replace on a DG
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest7()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("d", "c");
            t.ReplaceDependents("a", new HashSet<string>() { "x", "y", "z" });
            HashSet<String> aPends = new HashSet<string>(t.GetDependents("a"));
            Assert.IsTrue(aPends.SetEquals(new HashSet<string>() { "x", "y", "z" }));
        }

        /// <summary>
        ///Replace on a DG
        ///</summary>
        [TestMethod()]
        public void NonEmptyTest8()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("d", "c");
            t.ReplaceDependees("c", new HashSet<string>() { "x", "y", "z" });
            HashSet<String> cDees = new HashSet<string>(t.GetDependees("c"));
            Assert.IsTrue(cDees.SetEquals(new HashSet<string>() { "x", "y", "z" }));
        }

        // ************************** STRESS TESTS ******************************** //
        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest1()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 100;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 2; j < SIZE; j += 2)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }



        // ********************************** ANOTHER STESS TEST ******************** //
        /// <summary>
        ///Using lots of data with replacement
        ///</summary>
        [TestMethod()]
        public void StressTest8()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 100;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 2; j < SIZE; j += 2)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Replace a bunch of dependents
            for (int i = 0; i < SIZE; i += 4)
            {
                HashSet<string> newDents = new HashSet<String>();
                for (int j = 0; j < SIZE; j += 7)
                {
                    newDents.Add(letters[j]);
                }
                t.ReplaceDependents(letters[i], newDents);

                foreach (string s in dents[i])
                {
                    dees[s[0] - 'a'].Remove(letters[i]);
                }

                foreach (string s in newDents)
                {
                    dees[s[0] - 'a'].Add(letters[i]);
                }

                dents[i] = newDents;
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

        // ********************************** A THIRD STESS TEST ******************** //
        /// <summary>
        ///Using lots of data with replacement
        ///</summary>
        [TestMethod()]
        public void StressTest15()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 100;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 2; j < SIZE; j += 2)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Replace a bunch of dependees
            for (int i = 0; i < SIZE; i += 4)
            {
                HashSet<string> newDees = new HashSet<String>();
                for (int j = 0; j < SIZE; j += 7)
                {
                    newDees.Add(letters[j]);
                }
                t.ReplaceDependees(letters[i], newDees);

                foreach (string s in dees[i])
                {
                    dents[s[0] - 'a'].Remove(letters[i]);
                }

                foreach (string s in newDees)
                {
                    dents[s[0] - 'a'].Add(letters[i]);
                }

                dees[i] = newDees;
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }
    }
}