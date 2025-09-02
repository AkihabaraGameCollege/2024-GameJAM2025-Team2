using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Audio;

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

    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;
    [SerializeField] bool isGrounded;

    [Header("被弾処理")]
    [Tooltip("全体の慣性")]
    public float knockBackForce = 5.0f;
    [Tooltip("上の慣性")]
    public float knockBackUpForce = 1.0f;
    
    [Header("無敵時間")]
    public float invincibleTime = 3.0f;

    [Header("音量設定")]
    [SerializeField] AudioMixer audioMixer = null;
    [SerializeField] AudioSource moveBoolSe;
    [SerializeField] AudioSource laneMoveSe;
    [SerializeField] AudioSource takeHitSe;
    [SerializeField] AudioSource jumpSe;
    [SerializeField] AudioSource jumpActionSe;


    private bool isInvincible = false;    //無敵かどうか
    private bool canControl = true;       //ヒットした時の硬直
    [SerializeField] private bool isAttackMode = false;    //ジャンプ中攻撃状態

    [SerializeField]
    private Animator playerAnimator = null;
    private Rigidbody rb;

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

        if (isGrounded && isAttackMode)
        {
            isAttackMode = false;
            playerAnimator.ResetTrigger("SuccessJumpAction");
        }

        MoveSE();
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

        // --- 自動前進SE再生 ---
        if (soundManager != null && isGrounded && forwardSpeed > 0)
        {
            if (!isAutoMoveSEPlaying)
            {
                soundManager.PlayPlayerAutoMoveAudio();
                isAutoMoveSEPlaying = true;
            }
        }
        else
        {
            if (isAutoMoveSEPlaying)
            {
                // SoundManager内でAudioSourceをStopする実装がなければ、必要に応じてStop用メソッドを追加してください
                // 例: soundManager.StopPlayerAutoMoveAudio();
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
                jumpActionSe?.Play();
                playerAnimator.SetTrigger("SuccessJumpAction");

                // 敵撃破SE再生
                if (soundManager != null)
                {
                    soundManager.PlayEnemyDefeatAudio();
                }

                Destroy(other.gameObject);

                //追加ジャンプ
                Vector3 forwardBoost = transform.forward * forwardForce; // ← 前方向加速の強さ（調整可）

                // ジャンプ + 前方向に勢いを与える
                rb.linearVelocity = new Vector3(
                    forwardBoost.x,                  // 前方向X成分
                    jumpForce * doubleJump,          // 上方向ジャンプ
                    forwardBoost.z                   // 前方向Z成分
                );

                isAttackMode = false;
            }
            else if (!isInvincible)
            {
                Debug.Log($"{name} が {other.name} に当たった");
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
            laneMoveSe?.Play();
            playerAnimator.SetTrigger("MoveLeft");
            currentLane = Mathf.Max(0, currentLane - 1);
            targetX = (currentLane - 1) * laneDistance;

            // レーン移動SE再生
            if (soundManager != null)
            {
                soundManager.PlayPlayerLaneMoveAudio();
            }
        }
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (canControl && context.performed)
        {
            laneMoveSe?.Play();
            playerAnimator.SetTrigger("MoveRight");
            currentLane = Mathf.Min(2, currentLane + 1);
            targetX = (currentLane - 1) * laneDistance;

            // レーン移動SE再生
            if (soundManager != null)
            {
                soundManager.PlayPlayerLaneMoveAudio();
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (canControl && context.performed && isGrounded)
        {
            Debug.Log("ジャンプ中ジャンプアクション実行可");
            jumpSe?.Play();
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
            Debug.Log("ジャンプアクション実行した");
            playerAnimator.SetTrigger("JumpAction");

            // トリックアクションSE再生
            if (soundManager != null)
            {
                soundManager.PlayTrickActionAudio();
            }
        }
    }
    #endregion

    public void MoveSE()
    {
        if (isGrounded)
        {
            if (!moveBoolSe.isPlaying)
            {
                moveBoolSe.Play();
            }
        }
        else
        {
            if (moveBoolSe.isPlaying)
            {
                moveBoolSe.Stop();
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

}