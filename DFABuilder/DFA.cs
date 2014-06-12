using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DFABuilder
{
    public class DFA
    {
        /// <summary>
        /// The finite set of states in this DFA
        /// </summary>
        public HashSet<DFA_State> States { get; private set; }
        /// <summary>
        /// The valid alphabet for this DFA
        /// </summary>
        public HashSet<char> Alphabet { get; private set; }
        /// <summary>
        /// The current state of this DFA
        /// </summary>
        public DFA_State CurrentState { get; private set; }
        /// <summary>
        /// The 
        /// </summary>
        public DFA_State StartState { get; private set; }
        public HashSet<DFA_State> AcceptStates { get; private set; }
        /// <summary>
        /// Creates a new DFA with the specified alphabet, parsed from the specified
        /// string.
        /// </summary>
        /// <param name="alphabet">The alphabet of this DFA</param>
        /// <param name="dfa_string">An encoding of this DFA</param>
        public DFA(HashSet<char> alphabet, List<string> dfa_string)
        {
            this.Alphabet = alphabet;
            this._parse(dfa_string);
            this.CurrentState = this.StartState;
        }
        /// <summary>
        /// Steps on the specified character and determines whether this is a valid
        /// transition
        /// </summary>
        /// <param name="character">The character on which to transition</param>
        /// <returns>Whether this step was valid</returns>
        public bool StepOn(char character)
        {
            this.CurrentState = CurrentState.TransitionOn(character);
            if (null == this.CurrentState)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Runs the given DFA on a specified input and determines whether this
        /// input is accepted.
        /// </summary>
        /// <param name="input">The string to be run on.</param>
        /// <returns>Whether the input was accepted.</returns>
        public bool RunOn(string input)
        {
            bool accepted;
            foreach (char c in input)
            {
                if (false == this.StepOn(c))
                {
                    accepted = false;
                }
            }
            if (this.AcceptStates.Contains(this.CurrentState))
            {
                accepted = true;
            }
            else
            {
                accepted = false;
            }
            this.CurrentState = this.StartState;
            return accepted;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DFA_State getStateBy(string name)
        {
            foreach (DFA_State state in this.States)
            {
                if (state.Name == name)
                {
                    return state;
                }
            }
            return null;
        }
        private void _parse(List<string> dfa_string)
        {
            if (dfa_string.Count > 5)
            {
                throw new ArgumentException("DFA String too long");
            }
            this._parse(dfa_string);
            if (false == this._validate())
            {
                throw new ArgumentException("Illegal DFA");
            }

        }
        private void _parseStates(string state_string)
        {
            StringBuilder sb = new StringBuilder();
            HashSet<string> newStateNames = new HashSet<string>();
            foreach (char c in state_string)
            {
                if (c == ',')
                {
                    if (false == newStateNames.Add(sb.ToString()))
                    {
                        throw new ArgumentException("Repeat state added");
                    }
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            foreach (string newStateName in newStateNames)
            {
                this.States.Add(new DFA_State(
                    this,
                    newStateName));
            }
        }
        private void _parseAlphabet(string alphabet_string)
        {
            HashSet<char> alphabetCharacters = new HashSet<char>();
            foreach (char c in alphabet_string)
            {
                if (false == alphabetCharacters.Add(c))
                {
                    throw new ArgumentException("Repeat alphabet letters added");
                }
            }
            this.Alphabet = alphabetCharacters;
        }
        private void _parseTransitions(string transition_string)
        {
            StringBuilder sb = new StringBuilder();
            DFA_State workingOnState;
            char transChar;
            char prevChar = Char.MinValue;
            DFA_State transDest;
            foreach (char c in transition_string)
            {
                if (c == ':')
                {
                    workingOnState = this.getStateBy(sb.ToString());
                    sb.Clear();
                }
                if (c == ',')
                {
                    if (prevChar == Char.MinValue)
                    {
                        throw new ArgumentException("Illegal transition string");
                    }
                    transChar = prevChar;
                    sb.Clear();
                }
                if (c == ';')
                {
                    transDest = this.getStateBy(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }

                prevChar = c;
            }
        }
        private void _parseStartState(string start_string)
        {
            this.StartState = this.getStateBy(start_string);
            if (StartState == null)
            {
                throw new ArgumentException("Illegal start state string");
            }
        }
        private void _parseAcceptStates(string accept_string)
        {
            StringBuilder sb = new StringBuilder();
            HashSet<DFA_State> acceptStates = new HashSet<DFA_State>();
            foreach (char c in accept_string)
            {
                if (c == ',')
                {
                    if (false == acceptStates.Add(
                        this.getStateBy(sb.ToString())))
                    {
                        throw new ArgumentException("Illegal accept state string");
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
        }
        private bool _validate()
        {
            // Start state must be in set of states
            if (false == this.States.Contains(this.StartState))
            {
                return false;
            }
            // All accept stataes must be in set of states
            foreach (DFA_State f in this.AcceptStates)
            {
                if (false == this.States.Contains(f))
                {
                    return false;
                }
            }
            
            foreach (DFA_State s in this.States)
            {
                // Every state must have a transition for every
                // letter in the alphabet
                foreach (char c in this.Alphabet)
                {
                    if (false == s.Transitions.ContainsKey(c))
                    {
                        return false;
                    }
                }
                // Additionally, every transition must be a letter
                // of the alphabet
                foreach (char c in s.Transitions.Keys)
                {
                    if (false == this.Alphabet.Contains(c))
                    {
                        return false;
                    }
                }

            }
            return true;
        }
    }
}
