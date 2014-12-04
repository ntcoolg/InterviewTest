using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompareLib;

namespace FundsFileCompare
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                try
                {
                    var fundsFile0 = new FundsFile(args[0]);
                    var fundsFile1 = new FundsFile(args[1]);

                    var filesAreEqual = fundsFile0.Equals(fundsFile1);

                    Console.WriteLine(filesAreEqual ? "Files are equal." : "Files are not equal.");
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Please provide two files to compare for equality.");
            }
        }
    }
}
