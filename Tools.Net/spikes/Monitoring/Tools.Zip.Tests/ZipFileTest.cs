﻿using Tools.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace Tools.Zip.Tests
{
    
    
    /// <summary>
    ///This is a test class for ZipFileTest and is intended
    ///to contain all ZipFileTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ZipFileTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Entries
        ///</summary>
        [TestMethod()]
        public void EntriesTest()
        {
            string name = string.Empty; // TODO: Initialize to an appropriate value
            ZipFile target = new ZipFile(name); // TODO: Initialize to an appropriate value
            List<ZipEntry> actual;
            actual = target.Entries;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetOffset
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Tools.Zip.dll")]
        public void GetOffsetTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ZipFile_Accessor target = new ZipFile_Accessor(param0); // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            long expected = 0; // TODO: Initialize to an appropriate value
            long actual;
            actual = target.GetOffset(index);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CompressionMethod
        ///</summary>
        [TestMethod()]
        public void CompressionMethodTest()
        {
            string name = string.Empty; // TODO: Initialize to an appropriate value
            ZipFile target = new ZipFile(name); // TODO: Initialize to an appropriate value
            byte expected = 0; // TODO: Initialize to an appropriate value
            byte actual;
            actual = target.CompressionMethod();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Close
        ///</summary>
        [TestMethod()]
        public void CloseTest()
        {
            string name = string.Empty; // TODO: Initialize to an appropriate value
            ZipFile target = new ZipFile(name); // TODO: Initialize to an appropriate value
            target.Close();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CheckFileExists
        ///</summary>
        [TestMethod()]
        public void CheckFileExistsTest()
        {
            string name = string.Empty; // TODO: Initialize to an appropriate value
            ZipFile target = new ZipFile(name); // TODO: Initialize to an appropriate value
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.CheckFileExists(fileName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod()]
        [DeploymentItem("ZipData\\test.txt")]
        public void AddTest()
        {
            string name = "ZipData\\test.txt";
            string outName = "ZipData\\test.zip";
            if (File.Exists(outName)) File.Delete(outName);

            ZipFile target = new ZipFile(outName, ZipConstants.GZIP, FileMode.CreateNew);
            string fileName = "ZipData\\test.txt";

            target.Add(fileName);

            target.Close();
        }

        /// <summary>
        ///A test for ZipFile Constructor
        ///</summary>
        [TestMethod()]
        public void ZipFileConstructorTest1()
        {
            string name = string.Empty; // TODO: Initialize to an appropriate value
            byte method = 0; // TODO: Initialize to an appropriate value
            FileMode mode = new FileMode(); // TODO: Initialize to an appropriate value
            ZipFile target = new ZipFile(name, method, mode);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ZipFile Constructor
        ///</summary>
        [TestMethod()]
        public void ZipFileConstructorTest()
        {
            string name = string.Empty; // TODO: Initialize to an appropriate value
            ZipFile target = new ZipFile(name);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
