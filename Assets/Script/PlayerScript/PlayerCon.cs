using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCon : MonoBehaviour
{
    // --------------------
    // レーン移動設定
    // --------------------
    [Header("レーン設定")]
    [SerializeField] private float laneDistance = 2.5f;
    private int currentLane = 1;
    private float targetX;

    [Header("レーン移動設定")]
    [SerializeField] private float laneChangeSpeed = 10f;

    // --------------------
    // プレイヤーカラー設定
    // --------------------
    [Header("ArrowBoardの色")]
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Renderer playerBoardRenderer;
    [Header("色を切り換えた時のエフェクト")]
    [SerializeField] private GameObject laneChangeEffectPrefab; // 色切り換えるときのエフェクト
    [SerializeField] private Transform laneEffectPoint;

    // --------------------
    // 前進・ジャンプ設定
    // --------------------
    [Header("前進移動設定")]
    [SerializeField] private float forwardSpeed = 1.0f;

    [Header("ジャンプ設定")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float fallSpeed = 1.0f;

    [Header("敵を踏みつけた後のジャンプ設定")]
    [SerializeField] private float doubleJump = 1.2f;
    [SerializeField] private float forwardForce = 5.0f;
    [SerializeField] private AudioSource successJumpActionSe;

    // --------------------
    // エフェクト
    // --------------------
    [Header("エフェクト")]
    [SerializeField] private GameObject stompEffectPrefab; // 敵に出すエフェクト
    [SerializeField] private Transform playerEffectPoint; //　プレイヤーがエフェクトをだすポイント
    [SerializeField] private GameObject playerEffectPrefab; // プレイヤーに出すエフェクト

    // --------------------
    // 地面判定
    // --------------------
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGrounded;

    // --------------------
    // 被弾処理
    // --------------------
    [Header("被弾処理")]
    [SerializeField] private AudioSource takeHitSe;
    [Tooltip("全体の慣性")]
    [SerializeField] private float knockBackForce = 5.0f;
    [Tooltip("上方向の慣性")]
    [SerializeField] private float knockBackUpForce = 1.0f;
    [SerializeField] private float invincibleTime = 3.0f;
    private bool isInvincible = false;
    private bool canControl = true;

    // --------------------
    // 攻撃・アニメーション
    // --------------------
    [SerializeField] private bool isAttackMode = false; // ジャンプ中攻撃状態
    [SerializeField] private Animator playerAnimator = null;

    // --------------------
    // 内部管理用
    // --------------------
    private Rigidbody rb;
    private int consecutiveJumpActions = 0; // 連続ジャンプアクション管理
    private SoundManager soundManager;      // SoundManager参照用
    private bool isAutoMoveSEPlaying = false;
    private UIManager uiManager;            // UIManager参照用

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //playerAnimator = GetComponent<Animator>();
        targetX = transform.position.x;

        // SoundManagerインスタンス取得
        soundManager = Object.FindFirstObjectByType<SoundManager>();

        // UIManagerインスタンス取得
        uiManager = Object.FindFirstObjectByType<UIManager>();
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

        HandleMovement();//移動処理

        HandleAutoMoveAudio();//前進SE
    }

    private void HandleMovement()
    {
        float newX = Mathf.Lerp(rb.position.x, targetX, Time.fixedDeltaTime * laneChangeSpeed);
        rb.position = new Vector3(newX, rb.position.y, rb.position.z);

        // --- 前進方向を地形の法線に沿わせる ---
        Vector3 forwardDir = Vector3.forward;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f, groundMask))
        {
            // 地形の傾斜に沿った前進方向を計算
            forwardDir = Vector3.ProjectOnPlane(Vector3.forward, hit.normal).normalized;
        }

        // Rigidbody の velocity を使って移動（坂も登れる）
        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0; // XはLane移動で制御
        velocity.z = forwardDir.z * forwardSpeed;
        if (forwardDir.x != 0) velocity.x = forwardDir.x * forwardSpeed;

        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        //落下
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallSpeed - 1) * Time.fixedDeltaTime;
        }
    }

    private void HandleAutoMoveAudio()
    {
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
            if (!isGrounded && isAttackMode && Time.timeScale > 0f)
            {
                Debug.Log("敵を踏んだ");
                successJumpActionSe?.Play();

                playerAnimator.Play("JumpAction", 0, 0f);

                // 敵撃破SE再生
                if (soundManager != null)
                {
                    soundManager.PlayEnemyDefeatAudio();
                }

                //敵にエフェクト生成
                if (stompEffectPrefab != null)
                {
                    Instantiate(stompEffectPrefab, other.transform.position, Quaternion.identity);
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

                // スコア+250加算
                if (uiManager != null)
                {
                    uiManager.AddScore(250);
                }

                isAttackMode = false;
            }
            else if (!isInvincible)
            {
                Debug.Log($"{name} が {other.name} に当たった");

                // スコア-200減算
                if (uiManager != null)
                {
                    uiManager.AddScore(-200);
                }

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

    private void UpdatePlayerBoardColor(bool withEffect = true)
    {
        if (playerBoardRenderer == null) return;


        switch (currentLane)
        {
            case 0: // 左
                playerBoardRenderer.material = blueMaterial;
                break;
            case 1: // 中央
                playerBoardRenderer.material = greenMaterial;
                break;
            case 2: // 右
                playerBoardRenderer.material = redMaterial;
                break;
        }

        if (withEffect && Time.timeScale > 0f)
        {
            if (laneChangeEffectPrefab != null && laneEffectPoint != null)
            {
                GameObject effect = Instantiate(laneChangeEffectPrefab, laneEffectPoint.position, Quaternion.identity);
                effect.transform.SetParent(laneEffectPoint); // プレイヤーに追従させたいなら
                Destroy(effect, 2.0f);
            }
        }
    }

    #region INputSystem
    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (canControl && context.performed && Time.timeScale > 0f)
        {
            int nextLane = Mathf.Max(0, currentLane - 1);
            float nextX = (nextLane - 1) * laneDistance;

            Vector3 checkDir = Vector3.left;//障害物がなかったら移動
            if (!Physics.Raycast(transform.position, checkDir, laneDistance, groundMask))
            {
                if (isGrounded && !isAttackMode)
                {
                    playerAnimator.Play("MoveLeft", 0, 0f);
                }

                currentLane = nextLane;
                targetX = nextX;
                UpdatePlayerBoardColor();

                // レーン移動SE再生
                if (soundManager != null)
                {
                    soundManager.PlayLaneMoveAudio();
                }
            }
        }
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (canControl && context.performed && Time.timeScale > 0f)
        {
            int nextLane = Mathf.Min(2, currentLane + 1);
            float nextX = (nextLane - 1) * laneDistance;

            //障害物がなかったら
            Vector3 checkDir = Vector3.right;//右移動
            if (!Physics.Raycast(transform.position, checkDir, laneDistance, groundMask))
            {
                if (isGrounded && !isAttackMode)
                {
                    playerAnimator.Play("MoveRight", 0, 0f);
                }

                currentLane = nextLane;
                targetX = nextX;
                UpdatePlayerBoardColor();

                // レーン移動SE再生
                if (soundManager != null)
                {
                    soundManager.PlayLaneMoveAudio();
                }
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
        if (canControl && context.performed && !isGrounded && Time.timeScale > 0f)
        {
            Debug.Log("ジャンプアクション実行 → 攻撃モードON");
            isAttackMode = true;

            //プレイヤーにエフェクト生成
            if (playerEffectPrefab != null)
            {
                GameObject effect = Instantiate(playerEffectPrefab, playerEffectPoint.position, Quaternion.identity);
                effect.transform.SetParent(playerEffectPoint);
                Destroy(effect, 2.0f);
            }

            // トリックアクションSE再生
            if (soundManager != null)
            {
                soundManager.PlayTrickAction1Audio();
            }

            // スコア+50加算
            if (uiManager != null)
            {
                uiManager.AddScore(50);
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

    public void SetControlEnabled(bool enabled)
    {
        canControl = enabled;
    }

}