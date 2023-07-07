using Stateless;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TesterScript : MonoBehaviour
{
    public enum Trigger
    {
        Stop,
        Walk,
        Run,
        Jump,
        DoneJumping
    }

    public enum State
    {
        Standing,
        Walking,
        Running,
        Jumping
    }

    public Color Color;

    private State _state = State.Standing;
    private StateMachine<State, Trigger> _machine;
    public string state;
    public string message;

    private void SetupStateMachine()
    {
        _machine = new StateMachine<State, Trigger>(() => _state, s => { _state = s; state = s.ToString(); });
        _machine.OnUnhandledTrigger((state, trigger) => { });

        _machine.Configure(State.Standing)
            .Permit(Trigger.Walk, State.Walking)
            .Permit(Trigger.Run, State.Running)
            .Permit(Trigger.Jump, State.Jumping);

        _machine.Configure(State.Walking)
            .Permit(Trigger.Stop, State.Standing)
            .Permit(Trigger.Run, State.Running)
            .Permit(Trigger.Jump, State.Jumping);

        _machine.Configure(State.Running)
            .Permit(Trigger.Walk, State.Walking)
            .Permit(Trigger.Stop, State.Standing)
            .Permit(Trigger.Jump, State.Jumping);

        State jumpSource = State.Standing;
        _machine.Configure(State.Jumping)
            .OnEntry(transition => { jumpSource = transition.Source; })
            .PermitDynamic(Trigger.DoneJumping, () => { return jumpSource; });

    }


    // Start is called before the first frame update
    void Start()
    {
        SetupStateMachine();
    }

    // Update is called once per frame
    void Update()
    {
        state = _machine.State.ToString();
    }


    [CustomEditor(typeof(TesterScript))]
    public class TesterScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            

            TesterScript t = (TesterScript)target;

            if (GUILayout.Button("Stop"))
            {
                t._machine.Fire(Trigger.Stop);
            }
            if (GUILayout.Button("Walk"))
            {
                t._machine.Fire(Trigger.Walk);
            }
            if (GUILayout.Button("Run"))
            {
                t._machine.Fire(Trigger.Run);
            }
            if (GUILayout.Button("Jump"))
            {
                t._machine.Fire(Trigger.Jump);
            }
            if (GUILayout.Button("Done Jumping"))
            {
                t._machine.Fire(Trigger.DoneJumping);
            }


        }

    }

}
