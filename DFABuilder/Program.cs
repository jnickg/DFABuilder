using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DFABuilder
{
    class Program
    {
        static TextWriter output = Console.Out;
        static void Main(string[] args)
        {
            DFA deterministicfiniteautomata_isalongname;
            TextReader dfa_reader;
            List<string> dfa_strings;
            string input;
            string line=String.Empty;
            try
            {
                if (args.ToList().Count > 2)
                {
                    throw new ArgumentException("Passed too many arguments");
                }
                output.WriteLine("Opening DFA file...");
                dfa_reader = File.OpenText(args[0]);
                output.WriteLine("Reading contents...");
                dfa_strings = new List<string>();
                while (null != (line=dfa_reader.ReadLine()))
                {
                    output.WriteLine("\tread line: {0}", line);
                    dfa_strings.Add(line);
                }
                output.WriteLine("Closing file...");
                dfa_reader.Close();
                output.WriteLine("Generating DFA from description...");
                deterministicfiniteautomata_isalongname = new DFA(dfa_strings);
                output.WriteLine("Success. Running DFA on input string: {0}", args[1]);
                if (deterministicfiniteautomata_isalongname.RunOn(args[1]))
                {
                    output.WriteLine("That string is accepted by the DFA.");
                }
                else
                {
                    output.WriteLine("That string is rejected by the DFA.");
                }
                output.WriteLine("\n\nClosing program.");

            }
            catch (FileNotFoundException)
            {
                output.WriteLine("Bad filename: {0}", args[0]);
            }
            catch (ArgumentException ex)
            {
                output.WriteLine(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                output.WriteLine("Whoopsa, you broke me. Way to go.");
                output.WriteLine("Caught exception type: {0}", ex.GetType().ToString());
                output.WriteLine("Message: {0}", ex.Message);
                output.WriteLine("\n\nAborting program...");
                return;
            }
        }
    }
}
