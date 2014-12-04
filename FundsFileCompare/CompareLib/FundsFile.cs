using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CompareLib
{
    public class FundsFile : IEquatable<FundsFile>
    {
        private const int MaximumBufferSize = 5;
        private const string OpeningToken = "ID [<";
        private const string ClosingToken = ">]";
        private readonly string filePath;
        private byte[] fileData;
        private string fileStringData;

        public FundsFile(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException("filePath","Please provide a valid file path");
            this.filePath = filePath;
        }

        /// This method accepts a FundsFile which is going to be compared with the current object.
        /// A return value of true indicates that the contents of the files are the same. 
        /// A return value of false indicates that the files are not the same.
        /// Files are not compared for file size equality as the ID token is not guaranteed to be 
        /// the same size for valid files based on the /ID pattern
        public bool Equals(FundsFile otherFundsFile)
        {
            int file1Byte;
            int file2Byte;
            bool isFile1Valid = false;
            bool isFile2Valid = false;
            // Determine if the same file was referenced two times.
            if (filePath == otherFundsFile.filePath)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            using (var file1Stream = new FileStream(filePath, FileMode.Open))
            using (var file2Stream = new FileStream(otherFundsFile.filePath, FileMode.Open))
            {
                var file1Queue = new Queue(MaximumBufferSize);
                var file2Queue = new Queue(MaximumBufferSize);

                // Read and compare a byte from each file until either a
                // non-matching set of bytes is found or until the end of
                // file1 is reached. Ignore ID token if found in the file
                // by skipping to the end of the token.
                do
                {
                    // Read one byte from each file.
                    file1Byte = file1Stream.ReadByte();
                    file2Byte = file2Stream.ReadByte();

                    // Process bytes for each file and move the read cursor
                    // forward when the ID token is found
                    var tokenFoundInFile1 = SkipTokenData(file1Stream, file1Byte, file1Queue);
                    var tokenFoundInFile2 = SkipTokenData(file2Stream, file2Byte, file2Queue);

                    // If the ID token is found flag the files as valid
                    if (!isFile1Valid && tokenFoundInFile1)
                    {
                        isFile1Valid = true;
                    }

                    if (!isFile2Valid && tokenFoundInFile2)
                    {
                        isFile2Valid = true;
                    }
                } while ((file1Byte == file2Byte) && (file1Byte != -1));
            }

            // Return the success of the comparison. "file1Byte" is 
            // equal to "file2Byte" at this point only if the files are 
            // the same.
            return ((file1Byte - file2Byte) == 0 && isFile1Valid && isFile2Valid);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (obj is FundsFile)
            {
                return Equals((FundsFile)obj);
            }
            return false;
        }

        private bool SkipTokenData(Stream fileStream, int byteData, Queue dataBuffer)
        {
            bool tokenFound = false;
            string bufferContents = GetBufferContents(byteData != -1, byteData, dataBuffer);
            if (!bufferContents.StartsWith(OpeningToken)) return tokenFound;
            do
            {
                tokenFound = true;
                if (dataBuffer.Count > MaximumBufferSize)
                {
                    dataBuffer.Dequeue();
                }
                var fileByte = fileStream.ReadByte();

                dataBuffer.Enqueue(Convert.ToChar(fileByte));
                bufferContents = String.Join("", dataBuffer.ToArray());
            } while (!bufferContents.EndsWith(ClosingToken));

            return tokenFound;
        }


        /// <summary>
        ///     Maintains the queue to the maximum length as specified by <see cref="MaximumBufferSize" /> and
        ///     converts the contents of the queue into a string to allow for pattern matching comparisons
        /// </summary>
        /// <param name="isNotEof">indicates whether the byte read is the EOF byte</param>
        /// <param name="data">the byte data to covert to a character</param>
        /// <param name="dataBuffer">the queue object to process</param>
        /// <returns>the content of the queue as a string</returns>
        private string GetBufferContents(bool isNotEof, int data, Queue dataBuffer)
        {
            if (isNotEof)
            {
                dataBuffer.Enqueue(Convert.ToChar(data));
                // Maintain the mininum amount of data to allow us to match the tokens 
                if (dataBuffer.Count > MaximumBufferSize)
                {
                    dataBuffer.Dequeue();
                }
                return string.Join("", dataBuffer.ToArray());
            }
            return string.Empty;
        }

        /// <summary>
        ///     Validate that the file contains the ID pattern
        /// </summary>
        /// <returns>true if the file a valid FundsFile</returns>
        public bool IsValid()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format("File not found at the specified path: {0}", filePath));
            fileData = File.ReadAllBytes(filePath);
            LoadFileStringData();
            const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            var regex = new Regex(@"/ID \[<.*>\]", options);
            return regex.IsMatch(fileStringData);
        }

        private void LoadFileStringData()
        {
            fileStringData = Encoding.Default.GetString(fileData);
        }
    }
}