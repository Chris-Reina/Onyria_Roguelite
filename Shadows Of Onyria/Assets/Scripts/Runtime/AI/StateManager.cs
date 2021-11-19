using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT.AI
{
    public class StateManager
    {
        private Dictionary<Type, State> _availableStates;

        public State CurrentState => _currentState;

        private State _currentState;

        public void SetStates(Dictionary<Type,State> states, State initialState)
        {
            _availableStates = states;

            if (states.ContainsKey(initialState.GetType()))
            {
                _currentState = states[initialState.GetType()];            
                CurrentState.Awake();
            }
            else
                DebugManager.LogError("The initial state was not in the states dictionary;");
        }

        public void Update()
        {
            if (CurrentState != null)
                _currentState.Execute();
        }

        public void SetState<T>() where T : State
        {
            if (!_availableStates.ContainsKey(typeof(T)))
            {
                DebugManager.LogWarning($"No State of type {typeof(T)} found in this StateManager.");
                return;
            }
            if (IsActualState<T>()) return;

            CurrentState.Sleep();
            _currentState = _availableStates[typeof(T)];
            CurrentState.Awake();
        }

        public void ResetState<T>() where T : State
        {
            if (!_availableStates.ContainsKey(typeof(T)))
            {
                DebugManager.LogWarning($"No State of type {typeof(T)} found in this StateManager.");
                return;
            }

            if (!IsActualState<T>()) return;
            
            CurrentState.Sleep();
            CurrentState.Awake();
        }

        public void AddState(State state)
        {
            _availableStates.Add(state.GetType(), state);
        }

        public bool IsActualState<T>() where T : State
        {
            return CurrentState.GetType() == typeof(T);
        }
    }
}