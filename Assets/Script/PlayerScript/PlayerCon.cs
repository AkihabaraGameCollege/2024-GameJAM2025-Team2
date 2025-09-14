using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCon : MonoBehaviour
{
    //[Header("敵")]

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
    [Header("敵を踏みつけた後のジャンプ設定")]
    public float doubleJump = 1.2f;
    public float forwardForce = 5.0f;
    [SerializeField] AudioSource successJumpActionSe;

    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;
    [SerializeField] bool isGrounded;

    [Header("被弾処理")]
    [SerializeField] AudioSource takeHitSe;
    [Tooltip("全体の慣性")]
    public float knockBackForce = 5.0f;
    [Tooltip("上の慣性")]
    public float knockBackUpForce = 1.0f;

    public float invincibleTime = 3.0f;
    private bool isInvincible = false;
    private bool canControl = true;
    [SerializeField] private bool isAttackMode = false;    //ジャンプ中攻撃状態

    [SerializeField]
    private Animator playerAnimator = null;
    private Rigidbody rb;

    // 連続ジャンプアクション管理用変数
    private int consecutiveJumpActions = 0;

    // SoundManager参照用
    private SoundManager soundManager;
    private bool isAutoMoveSEPlaying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //playerAnimator = GetComponent<Animator>();
        targetX = transform.position.x;

        // SoundManagerインスタンス取得
        soundManager = Object.FindFirstObjectByType<SoundManager>();
    }

    private void Update()
    {
        playerAnimator.SetBool("IsGrounded", isGrounded);

        //ジャンプアクション
        if (isGrounded && isAttackMode)
        {
            isAttackMode = false;
            playerAnimator.ResetTrigger("JumpAction");
        }
    }

    void FixedUpdate()
    {
        if (!canControl) return;

        // --- 地面判定 (Raycast) ---
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);

        //レーン移動,前進
        float newX = Mathf.Lerp(rb.position.x, targetX, Time.fixedDeltaTime * laneChangeSpeed);
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;
        Vector3 move = new Vector3(newX, rb.position.y, rb.position.z) + forwardMove;

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallSpeed - 1) * Time.fixedDeltaTime;
        }

        rb.MovePosition(move);

        // --- 自動前進SE再生・停止 ---
        if (soundManager != null && isGrounded && forwardSpeed > 0)
        {
            if (!isAutoMoveSEPlaying)
            {
                soundManager.PlayAutoMoveAudio();
                isAutoMoveSEPlaying = true;
            }
        }
        else
        {
            if (isAutoMoveSEPlaying)
            {
                soundManager.StopAutoMoveAudio();
                isAutoMoveSEPlaying = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!isGrounded && isAttackMode)
            {
                Debug.Log("敵を踏んだ");
                successJumpActionSe?.Play();

                playerAnimator.Play("JumpAction", 0, 0f);

                // 敵撃破SE再生
                if (soundManager != null)
                {
                    soundManager.PlayEnemyDefeatAudio();
                }

                Destroy(other.gameObject);

                //追加ジャンプ
                Vector3 forwardBoost = transform.forward * forwardForce;

                rb.linearVelocity = new Vector3(
                    forwardBoost.x,
                    jumpForce * doubleJump,
                    forwardBoost.z
                );

                // --- 連続ジャンプアクション判定・SE再生 ---
                consecutiveJumpActions++;
                if (soundManager != null)
                {
                    if (consecutiveJumpActions >= 2)
                    {
                        soundManager.PlayTrickAction2Audio();
                        Debug.Log("トリックアクション2のSEを再生");
                    }
                    else
                    {
                        soundManager.PlayTrickAction1Audio();
                        Debug.Log("トリックアクション1のSEを再生");
                    }
                }

                isAttackMode = false;
            }
            else if (!isInvincible)
            {
                Debug.Log($"{name} が {other.name} に当たった");
                StartCoroutine(HitRoutine());
            }
        }
        else if (other.CompareTag("Obstacle"))
        {
            if (!isInvincible)
            {
                Debug.Log($"{name} が障害物 {other.name} に激突");
                if (soundManager != null)
                {
                    soundManager.PlayPlayerCrashAudio();
                }
                StartCoroutine(HitRoutine());
            }
        }
    }

    private IEnumerator HitRoutine()
    {
        isInvincible = true;
        canControl = false;

        takeHitSe?.Play();
        playerAnimator.SetTrigger("TakeHit");
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
            if (isGrounded && !isAttackMode)
            {
                playerAnimator.Play("MoveLeft", 0, 0f);
            }

            currentLane = Mathf.Max(0, currentLane - 1);
            targetX = (currentLane - 1) * laneDistance;

            // レーン移動SE再生
            if (soundManager != null)
            {
                soundManager.PlayLaneMoveAudio();
            }
        }
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (canControl && context.performed)
        {
            if (isGrounded && !isAttackMode)
            {
                playerAnimator.Play("MoveRight", 0, 0f);
            }

            currentLane = Mathf.Min(2, currentLane + 1);
            targetX = (currentLane - 1) * laneDistance;

            // レーン移動SE再生
            if (soundManager != null)
            {
                soundManager.PlayLaneMoveAudio();
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (canControl && context.performed && isGrounded)
        {
            Debug.Log("ジャンプ中ジャンプアクション実行可");
            playerAnimator.SetTrigger("Jump");
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);

            // ジャンプSE再生
            if (soundManager != null)
            {
                soundManager.PlayJumpAudio();
            }
        }
    }

    // トリックアクション時
    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (canControl && context.performed && !isGrounded)
        {
            Debug.Log("ジャンプアクション実行 → 攻撃モードON");
            isAttackMode = true;

            // トリックアクションSE再生
            if (soundManager != null)
            {
                soundManager.PlayTrickAction1Audio();
            }
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        // デバッグ用に地面判定Rayを可視化
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

}