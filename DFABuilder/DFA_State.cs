using System.Collections.Generic;

namespace DFABuilder
{
    public class DFA_State
    {
        public DFA Parent { get; private set; }
        public string Name { get; private set; }
        public Dictionary<char, DFA_State> Transitions { get; private set; }
        /// <summary>
        /// Creates a new instance of a DFA_State with the specified parent and name
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public DFA_State(DFA parent, string name)
        {
            this.Parent = parent;
            this.Name = name;
            this.Transitions = new Dictionary<char, DFA_State>();
        }
        /// <summary>
        /// Adds a transition to the specified state, on the specified character.
        /// </summary>
        /// <param name="character">The character on which to transition</param>
        /// <param name="destState">The destination state to which this state transitions</param>
        /// <returns>True if this transition could be added (the character is in the parent
        /// DFA's alphabet, and a transition for this character did not exist, else False.</returns>
        public bool AddTransition(char character, DFA_State destState)
        {
            if (false == this.Parent.Alphabet.Contains(character))
            {
                return false;
            }
            if (this.Transitions.ContainsKey(character))
            {
                return false;
            }
            this.Transitions.Add(character, destState);
            return true;
        }
        /// <summary>
        /// Returns the DFA_State to which this state transitions on the specified
        /// character.
        /// </summary>
        /// <param name="character">The character on which to transition.</param>
        /// <returns>The DFA_State to which this state transitions, or null if
        /// there is no transition defined.</returns>
        public DFA_State TransitionOn(char character)
        {
            if (false == this.Transitions.ContainsKey(character))
            {
                return null;
            }
            else
            {
                return this.Transitions[character];
            }
        }
    }
}
