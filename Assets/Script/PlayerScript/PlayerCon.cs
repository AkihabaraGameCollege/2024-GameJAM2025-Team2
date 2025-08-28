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
    public float fallSpeed = 1.0f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("仮の空中アクション")]
    [Tooltip("前転")]
    public float spinSpeed = 360.0f;
    public float spinDuration = 0.5f;
    private bool isSpinning = false;
    private float spinTimer = 0f;


    [Header("被弾処理")]
    [Tooltip("全体の慣性")]
    public float knockBackForce = 5.0f;
    [Tooltip("上の慣性")]
    public float knockBackUpForce = 1.0f;

    public float invincibleTime = 3.0f;
    private bool isInvincible = false;
    private bool canControl = true;

    private Animator playerAnimator;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        targetX = transform.position.x;

    }

    private void Update()
    {
        playerAnimator.SetBool("IsGrounded", isGrounded);

        //空中アクション
        if (isSpinning)
        {
            spinTimer += Time.deltaTime;

            transform.Rotate(Vector3.right * spinSpeed * Time.deltaTime, Space.Self);
        
            // 着地したらリセット
            if (spinTimer >= spinDuration)
            {
                isSpinning = false;
                spinTimer = 0f;
                transform.rotation = Quaternion.identity; // 角度を元に戻す
            }
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

        if(rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallSpeed - 1) * Time.fixedDeltaTime;
        }

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

    #region INputSystem
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
            Debug.Log("ジャンプ中ジャンプアクション実行可");
            playerAnimator.SetTrigger("Jump");
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (canControl && context.performed && !isGrounded && !isSpinning)
        {
            Debug.Log("ジャンプアクション実行した");
            isSpinning = true;
            spinTimer = 0f;
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
