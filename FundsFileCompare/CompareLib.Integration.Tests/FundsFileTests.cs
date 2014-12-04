using System;
using System.IO;
using NUnit.Framework;

namespace CompareLib.Integration.Tests
{
    [TestFixture]
    public class FundsFileTests
    {
        private string file0Path = Path.Combine(Environment.CurrentDirectory, @"DataFiles\file_0.bin");
        private string file1Path = Path.Combine(Environment.CurrentDirectory, @"DataFiles\file_1.bin");
        private string file2Path = Path.Combine(Environment.CurrentDirectory, @"DataFiles\file_2.bin");
        private string file3Path = Path.Combine(Environment.CurrentDirectory, @"DataFiles\README.md");
        private string file4Path = Path.Combine(Environment.CurrentDirectory, @"DataFiles\LICENSE.txt");
        private string myFile1Path = Path.Combine(Environment.CurrentDirectory, @"DataFiles\mybinfile1.bin");
        private string myFile2Path = Path.Combine(Environment.CurrentDirectory, @"DataFiles\mybinfile2.bin");
        

        [Test]
        public void BinaryEquals_ValidFundFiles_AreEqual()
        {
            // Arrange
            var fundsFile0 = new FundsFile(file0Path);
            var fundsFile1 = new FundsFile(file1Path);

            // Act
            var filesAreEqual = fundsFile0.Equals(fundsFile1);
            
            // Asserts
            Assert.That(filesAreEqual);
        }

        [Test]
        public void BinaryEquals_ValidFundFiles_AreNotEqual()
        {
            // Arrange
            var fundsFile0 = new FundsFile(file0Path);
            var fundsFile2 = new FundsFile(file2Path);

            // Act
            var filesAreEqual = fundsFile0.Equals(fundsFile2);

            // Asserts
            Assert.That(filesAreEqual == false);
        }

        [Test]
        public void BinaryEquals_InValidFundFiles_AreNotEqual()
        {
            // Arrange
            var fundsFile3 = new FundsFile(file3Path);
            var fundsFile4 = new FundsFile(file4Path);

            // Act
            var filesAreEqual = fundsFile3.Equals(fundsFile4);

            // Asserts
            Assert.That(filesAreEqual == false);
        }


        [Test]
        public void BinaryEquals_ValidFundFilesWithUnEqualIDTokens_AreEqual()
        {
            // Arrange
            var funds1File = new FundsFile(myFile1Path);
            var funds2File = new FundsFile(myFile2Path);
            // Act
            var filesAreEqual = funds1File.Equals(funds2File);

            // Asserts
            Assert.That(filesAreEqual);
        }
      
    }
}
