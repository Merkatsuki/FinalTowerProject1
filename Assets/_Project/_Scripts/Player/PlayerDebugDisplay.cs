using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDebugDisplay : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    private PlayerController controller;
    private StateMachine stateMachine;
    private Rigidbody2D rb;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        stateMachine = GetComponent<StateMachine>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (debugText == null || controller == null || stateMachine == null) return;

        debugText.text = $"<b>DEBUG INFO</b>\n" +
                         $"State: <color=yellow>{stateMachine.CurrentState?.GetType().Name}</color>\n" +
                         $"Velocity: <color=cyan>({rb.linearVelocity.x:F2}, {rb.linearVelocity.y:F2})</color>\n" +
                         $"IsGrounded: {controller.IsGrounded()}\n" +
                         $"LastGroundedTime: {controller.lastGroundedTime:F2}\n" +
                         $"JumpBuffered: {controller.HasBufferedJump()}\n" +
                         $"LastJumpInputTime: {Time.time - controller.lastJumpInputTime:F2}\n";
    }
}
