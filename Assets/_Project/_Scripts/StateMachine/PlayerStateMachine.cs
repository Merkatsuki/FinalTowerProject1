using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [HideInInspector] public PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        SetState(new GroundedState(this));
    }
}
