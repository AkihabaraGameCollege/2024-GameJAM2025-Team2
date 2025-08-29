using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    [Header("仮の空中アクション")]
    [Tooltip("前転")]
    public float spinSpeed = 360.0f;
    private bool isSpinning = false;

    [Header("被弾処理")]
    [Tooltip("全体の慣性")]
    public float knockBackForce = 5.0f;
    [Tooltip("上の慣性")]
    public float knockBackUpForce = 1.0f;

    public float invincibleTime = 3.0f;
    private bool isInvincible = false;
    private bool canControl = true;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetX = transform.position.x;

    }

    private void Update()
    {
        if (isSpinning)
        {
            transform.Rotate(Vector3.right * spinSpeed * Time.deltaTime, Space.Self);
        }
            // 着地したらリセット
            if (isGrounded)
            {
                isSpinning = false;
                transform.rotation = Quaternion.identity; // 角度を元に戻す
            }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canControl) return;

        // --- 地面判定 (Raycast) ---
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);

        //レーン移動,前進
        float newX = Mathf.Lerp(rb.position.x, targetX, Time.fixedDeltaTime * laneChangeSpeed);
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;
        Vector3 move = new Vector3(newX, rb.position.y, rb.position.z) + forwardMove;

        rb.MovePosition(move);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !isInvincible)
        {
            Debug.Log($"{name} が {other.name} に当たった");
            StartCoroutine(HitRoutine());
        }
    }

    private IEnumerator HitRoutine()
    {
        isInvincible = true;
        canControl = false;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(new Vector3(0, knockBackUpForce, -1f) * knockBackForce, ForceMode.VelocityChange);
    
        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
        canControl = true;
    }

    #region InputSystem
    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (canControl && context.performed)
        {
            currentLane = Mathf.Max(0, currentLane - 1);
            targetX = (currentLane - 1) * laneDistance;
        }
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (canControl && context.performed)
        {
            currentLane = Mathf.Min(2, currentLane + 1);
            targetX = (currentLane - 1) * laneDistance;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (canControl && context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (canControl && context.performed && !isGrounded)
        { 
            isSpinning = true;
        }
    }
    #endregion

    #region  Debug
    private void OnDrawGizmosSelected()
    {
        // デバッグ用に地面判定Rayを可視化
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
    #endregion
}
