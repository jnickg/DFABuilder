using System;
using System.Collections.Generic;
using System.Text;

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
        /// The starting state of this DFA
        /// </summary>
        public DFA_State StartState { get; private set; }
        /// <summary>
        /// The set of accept states of this DFA
        /// </summary>
        public HashSet<DFA_State> AcceptStates { get; private set; }
        /// <summary>
        /// Creates a new DFA with the specified alphabet, parsed from the specified
        /// string.
        /// </summary>
        /// <param name="alphabet">The alphabet of this DFA</param>
        /// <param name="dfa_string_list">An encoding of this DFA</param>
        public DFA(List<string> dfa_string_list)
        {
            this.States = new HashSet<DFA_State>();
            this.Alphabet = new HashSet<char>();
            this.AcceptStates = new HashSet<DFA_State>();
            this._parse(dfa_string_list);
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
            DFA_State nextState = this.CurrentState.TransitionOn(character);
            this.CurrentState = nextState;
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
            if (false == this._validate())
            {
                throw new Exception("This DFA is illegal and cannot be run");
            }
            bool accepted;
            foreach (char c in input)
            {
                if (false == this.StepOn(c))
                {
                    accepted = false;
                    break;
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
        /// Returns this DFA's state of the specified name
        /// </summary>
        /// <param name="name">The name of the state to get</param>
        /// <returns>The state matching the specified name</returns>
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
        /// <summary>
        /// Parses the dfa_string and applies its contents to this instance.
        /// </summary>
        /// <param name="dfa_string">The string to parse.</param>
        private void _parse(List<string> dfa_string)
        {
            if (dfa_string.Count != 5)
            {
                throw new ArgumentException("DFA String too long");
            }
            this._parseStates(dfa_string[0]);
            this._parseAlphabet(dfa_string[1]);
            this._parseTransitions(dfa_string[2]);
            this._parseStartState(dfa_string[3]);
            this._parseAcceptStates(dfa_string[4]);
            if (false == this._validate())
            {
                throw new ArgumentException("Illegal DFA");
            }

        }
        /// <summary>
        /// Creates DFA_State objects for each item in the string
        /// </summary>
        /// <param name="state_string"></param>
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
        /// <summary>
        /// Generates the DFA's alphabet from the specified string
        /// </summary>
        /// <param name="alphabet_string"></param>
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
        /// <summary>
        /// Parses the transitions from the given string
        /// </summary>
        /// <param name="transition_string"></param>
        private void _parseTransitions(string transition_string)
        {
            StringBuilder sb = new StringBuilder();
            DFA_State workingOnState = null;
            DFA_State transDest = null;
            char transChar = Char.MinValue;
            char prevChar = Char.MinValue;
            foreach (char c in transition_string)
            {
                if (c == ':')
                {
                    workingOnState = this.getStateBy(sb.ToString());
                    if (workingOnState == null)
                    {
                        throw new ArgumentException("Specified transition for nonexistent state");
                    }
                    sb.Clear();
                }
                else if (c == ',')
                {
                    if (prevChar == Char.MinValue)
                    {
                        throw new ArgumentException("Illegal transition string");
                    }
                    transChar = prevChar;
                    sb.Clear();
                }
                else if (c == ';')
                {
                    transDest = this.getStateBy(sb.ToString());
                    if ((null == workingOnState) ||
                        (null == transDest) ||
                        (char.MinValue == prevChar))
                    {
                        throw new ArgumentException("Illegal transition string");
                    }
                    workingOnState.AddTransition(transChar, transDest);
                    sb.Clear();
                }
                else if (c == '.')
                {
                    transDest = this.getStateBy(sb.ToString());
                    if ((null == workingOnState) ||
                            (null == transDest) ||
                            (char.MinValue == prevChar))
                    {
                        throw new ArgumentException("Illegal transition string");
                    }
                    workingOnState.AddTransition(transChar, transDest);
                    workingOnState = null;
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }

                prevChar = c;
            }
        }
        /// <summary>
        /// Parses the start state from the given string
        /// </summary>
        /// <param name="start_string"></param>
        private void _parseStartState(string start_string)
        {
            this.StartState = this.getStateBy(start_string);
            if (StartState == null)
            {
                throw new ArgumentException("Illegal start state string");
            }
        }
        /// <summary>
        /// Parses the accept states from the given string
        /// </summary>
        /// <param name="accept_string"></param>
        private void _parseAcceptStates(string accept_string)
        {
            StringBuilder sb = new StringBuilder();
            HashSet<DFA_State> acceptStates = new HashSet<DFA_State>();
            foreach (char c in accept_string)
            {
                if (c == ',')
                {
                    DFA_State newAccept = this.getStateBy(sb.ToString());
                    if (false == acceptStates.Add(newAccept))
                    {
                        throw new ArgumentException("Illegal accept state string");
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            this.AcceptStates = acceptStates;
        }
        /// <summary>
        /// Determines whether this DFA follows all the rules of being a DFA
        /// </summary>
        /// <returns></returns>
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
