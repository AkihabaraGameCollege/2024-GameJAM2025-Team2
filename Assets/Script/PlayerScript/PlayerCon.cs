using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCon : MonoBehaviour
{
    [Header("レーン設定")]
    public float laneDistance = 2.5f;
    private int currentLane = 1;
    private float targetX;

    [Header("レーン移動設定")]
    public float laneChangeSpeed = 10f;

    [Header("前進移動設定")]
    public float forwardSpeed = 1.0f;

    [Header("ジャンプ設定")]
    public float jumpForce = 7f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;
    private bool isGrounded;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetX = transform.position.x;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // --- 地面判定 (Raycast) ---
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);

        //レーン移動,前進
        float newX = Mathf.Lerp(rb.position.x, targetX, Time.fixedDeltaTime * laneChangeSpeed);
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;
        Vector3 move = new Vector3(newX, rb.position.y, rb.position.z) + forwardMove;

        rb.MovePosition(move);
    }

    #region INputSystem
    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentLane = Mathf.Max(0, currentLane - 1);
            targetX = (currentLane - 1) * laneDistance;
        }
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentLane = Mathf.Min(2, currentLane + 1);
            targetX = (currentLane - 1) * laneDistance;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        // デバッグ用に地面判定Rayを可視化
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
