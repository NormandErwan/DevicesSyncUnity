using DevicesSyncUnity.Examples.Messages;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevicesSyncUnity.Examples.Debug
{
    public class StateSyncDisplay : MonoBehaviour
    {
        // Editor fields

        [SerializeField]
        private StateSync stateSync;

        [SerializeField]
        private Button state1Button;

        [SerializeField]
        private Button state2Button;

        [SerializeField]
        private Button state3Button;

        [SerializeField]
        private Text currentStateText;

        // Variables

        private Dictionary<State, string> stateStrings = new Dictionary<State, string>()
        {
            { State.State1, "State1" },
            { State.State2, "State2" },
            { State.State3, "State3" },
        };

        // Methods

        protected virtual void Awake()
        {
            stateSync.CurrentStateUpdated += (stateMessage) => { currentStateText.text = stateStrings[stateMessage.state]; };

            state1Button.onClick.AddListener(() => { ChangeCurrentState(State.State1); });
            state2Button.onClick.AddListener(() => { ChangeCurrentState(State.State2); });
            state3Button.onClick.AddListener(() => { ChangeCurrentState(State.State3); });
        }

        protected virtual void ChangeCurrentState(State newState)
        {
            stateSync.CurrentState = newState;
        }
    }
}