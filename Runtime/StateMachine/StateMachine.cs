// Copyright 2020 Felix Kahle. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace FelixKahle.UnityProjectsCore
{
    /// <summary>
    /// A state machine.
    /// </summary>
    /// <typeparam name="T">The enumeration of states.</typeparam>
    public class StateMachine<T> where T : System.Enum
    {
        /// <summary>
        /// This delegate is used to call the Enter, Update and Leave functions for states.
        /// </summary>
        public delegate void StateFunction();

        /// <summary>
        /// Dictionary with T as keys and states as values.
        /// </summary>
        private Dictionary<T, State> States = new Dictionary<T, State>();

        /// <summary>
        /// The current state.
        /// </summary>
        private State ActualCurrentState;

        /// <summary>
        /// State representation.
        /// </summary>
        private class State
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="id">The state.</param>
            /// <param name="enter">The function to call when entering the state.</param>
            /// <param name="update">The function to call when the state stays persistent.</param>
            /// <param name="leave">The function to call when leaving the state.</param>
            public State(T id, StateFunction enter, StateFunction update, StateFunction leave)
            {
                Id = id;
                Enter = enter;
                Update = update;
                Leave = leave;
            }

            /// <summary>
            /// The state.
            /// </summary>
            public T Id;

            /// <summary>
            /// The function to call when entering the state.
            /// </summary>
            public StateFunction Enter;

            /// <summary>
            /// The function to call when the state stays persistent.
            /// </summary>
            public StateFunction Update;

            /// <summary>
            /// The function to call when leaving the state.
            /// </summary>
            public StateFunction Leave;
        }

        /// <summary>
        /// Adds a new state.
        /// </summary>
        /// <param name="id">The state to add.</param>
        /// <param name="enter">The function to call when entering the state.</param>
        /// <param name="update">The function to call when the state stays persistent.</param>
        /// <param name="leave">The function to call when leaving the state.</param>
        public void Add(T id, StateFunction enter, StateFunction update, StateFunction leave)
        {
            States.Add(id, new State(id, enter, update, leave));
        }

        /// <summary>
        /// Updates the StateMachine.
        /// </summary>
        public void Update()
        {
            ActualCurrentState.Update?.Invoke();
        }

        /// <summary>
        /// Switches to a different state.
        /// </summary>
        /// <param name="state">The state to switch to.</param>
        public void SwitchTo(T state)
        {
#if UNITY_EDITOR
            Debug.Assert(States.ContainsKey(state), "Trying to switch to unknown state " + state.ToString());
            Debug.Assert(ActualCurrentState == null || !ActualCurrentState.Id.Equals(state), "Trying to switch to " + state.ToString() + " but that is already current state");
#endif
            State newState = States[state];
#if UNITY_EDITOR
            Debug.Log("Switching state: " + (ActualCurrentState != null ? ActualCurrentState.Id.ToString() : "null") + " -> " + state.ToString());
#endif
            if (ActualCurrentState != null && ActualCurrentState.Leave != null)
            {
                ActualCurrentState.Leave();
            }
            if (newState.Enter != null)
            {
                newState.Enter();
            }
            ActualCurrentState = newState;
        }

        /// <summary>
        /// Shuts down the StateMachine.
        /// </summary>
        public void Shutdown()
        {
            if (ActualCurrentState != null && ActualCurrentState.Leave != null)
            {
                ActualCurrentState.Leave();
            }
            ActualCurrentState = null;
        }

        /// <summary>
        /// Returns the current state.
        /// </summary>
        /// <returns>The current state.</returns>
        public T CurrentState()
        {
            return ActualCurrentState.Id;
        }
    }
}
