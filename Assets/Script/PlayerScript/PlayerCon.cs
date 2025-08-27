using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCon : MonoBehaviour
{
    [Header("レーン設定")]
    public float laneDistance = 2.5f;
    private int currentLane = 1;
    private Vector3 targetPos;

    [Header("移動設定")]
    public float laneChangeSpeed = 10f;

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
    }

    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
            currentLane = Mathf.Max(0, currentLane - 1);
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
            currentLane = Mathf.Min(2, currentLane + 1);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //レーン移動
        targetPos = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * laneChangeSpeed);

        // --- 地面判定 (Raycast) ---
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }

    private void OnDrawGizmosSelected()
    {
        // デバッグ用に地面判定Rayを可視化
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
