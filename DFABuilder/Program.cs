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
            DFA dfa;
            TextReader dfa_reader;
            List<string> dfa_strings;
            string input;
            string line=String.Empty;
            try
            {
                if (args.ToList().Count < 2)
                {
                    throw new ArgumentException("Passed too many arguments");
                }
                output.WriteLine("Opening DFA file...");
                dfa_reader = File.OpenText(args[0]);
                output.WriteLine("Reading contents...");
                dfa_strings = new List<string>();
                while (null != (line = dfa_reader.ReadLine()))
                {
                    output.WriteLine("\tread line: {0}", line);
                    dfa_strings.Add(line);
                }
                output.WriteLine("Closing file...");
                dfa_reader.Close();
                output.WriteLine("Generating DFA from description...");
                dfa = new DFA(dfa_strings);
                output.WriteLine("Success. Printing DFA...");
                output.Write("Alphabet: ");
                foreach (char letter in dfa.Alphabet) { output.Write(letter); }
                output.WriteLine();
                output.WriteLine("Start state: {0}", dfa.StartState.Name);
                output.Write("Accept states: ");
                foreach (DFA_State state in dfa.AcceptStates) { output.Write(state.Name); }
                output.WriteLine();
                output.WriteLine("Transitions:");
                foreach (DFA_State state in dfa.States)
                {
                    output.WriteLine("\t{0} transitions:", state.Name);
                    foreach (char k in state.Transitions.Keys)
                    {
                        output.WriteLine("\t\t{0}->{1}", k, state.Transitions[k].Name);
                    }
                }
                for (int i = 1; i < args.ToList().Count; i++)
                {
                    output.WriteLine("\nRunning DFA on input string: {0}", args[i]);
                    if (dfa.RunOn(args[i]))
                    {
                        output.WriteLine("\t...ACCEPTED by the DFA.");
                    }
                    else
                    {
                        output.WriteLine("\t...REJECTED by the DFA.");
                    }
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
                output.WriteLine("Whoops, you broke me. Way to go.");
                output.WriteLine("Caught exception type: {0}", ex.GetType().ToString());
                output.WriteLine("Message: {0}", ex.Message);
                output.WriteLine("\n\nAborting program...");
                return;
            }
            finally
            {
                if (output == Console.Out) { Console.ReadKey(); }
            }
        }
    }
}
