using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// ゲームのポーズ管理を行うクラス。
/// ポーズUIの表示・非表示、操作説明・サウンド設定UIの切り替え、リトライ・タイトル戻りなどを制御する。
/// </summary>
public class PauseManager : MonoBehaviour
{
    // プレイヤーの入力管理
    private PlayerInput playerInput;
    // ポーズ用の入力アクション
    private InputAction pauseAction;
    // ポーズ状態かどうか
    private bool isPaused = false;

    // ポーズUIのGameObject
    [SerializeField] private GameObject pauseUI;
    // 操作説明UIのGameObject
    [SerializeField] private GameObject operationUI;
    // サウンド設定UIのGameObject
    [SerializeField] private GameObject soundSettingsUI;
    // リトライ時に遷移するシーン名
    [SerializeField] private string retrySceneName;
    // UIのスライドアニメーション時間
    [SerializeField] private float slideDuration = 0.5f;

    // ポーズUIのCanvasGroup（フェード制御用）
    private CanvasGroup pauseCanvasGroup;
    // ポーズUIのRectTransform（位置制御用）
    private RectTransform pauseRectTransform;
    // ポーズUIのスライド開始位置
    private Vector2 pauseUIStartPos;
    // ポーズUIの表示位置
    private Vector2 pauseUIEndPos;
    // ポーズUIの非表示位置
    private Vector2 pauseUIHidePos;

    /// <summary>
    /// 初期化処理。各UIの参照取得と初期状態設定。
    /// </summary>
    void Awake()
    {
        // PlayerInputの取得とPauseアクションのイベント登録
        playerInput = Object.FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            pauseAction = playerInput.actions["Pause"];
            pauseAction.performed += OnPause;
        }
        // ポーズUIのCanvasGroupとRectTransformの取得・初期化
        if (pauseUI != null)
        {
            pauseCanvasGroup = pauseUI.GetComponent<CanvasGroup>();
            if (pauseCanvasGroup == null)
                pauseCanvasGroup = pauseUI.AddComponent<CanvasGroup>();

            pauseRectTransform = pauseUI.GetComponent<RectTransform>();
            pauseUIEndPos = pauseRectTransform.anchoredPosition;
            pauseUIStartPos = pauseUIEndPos + new Vector2(-pauseRectTransform.rect.width, 0); // 左外
            pauseUIHidePos = pauseUIEndPos + new Vector2(pauseRectTransform.rect.width, 0);   // 右外
            pauseUI.SetActive(false);
        }
        // 操作説明UI・サウンド設定UIの非表示
        if (operationUI != null) operationUI.SetActive(false);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
    }

    /// <summary>
    /// 終了時のイベント登録解除
    /// </summary>
    void OnDestroy()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPause;
        }
    }

    /// <summary>
    /// ポーズ入力時の処理。ポーズ状態の切り替えとUI表示制御。
    /// </summary>
    /// <param name="context">入力アクションのコンテキスト</param>
    private void OnPause(InputAction.CallbackContext context)
    {
        var keyboard = Keyboard.current;
        // ポーズ中にESCキーで復帰
        if (isPaused && keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("エスケープキーが押されました。ポーズ解除します。");
            ResumeGame();
            return;
        }

        // ポーズ状態の切り替え
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        // ポーズUIの表示・非表示（スライドアニメーション）
        if (pauseUI != null)
        {
            if (isPaused)
            {
                StartCoroutine(SlideInPauseUI());
            }
            else
            {
                StartCoroutine(SlideOutPauseUI());
            }
        }
        // 操作説明UI・サウンド設定UIの非表示
        if (operationUI != null) operationUI.SetActive(false);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
    }

    /// <summary>
    /// ポーズUIをスライドイン表示するコルーチン
    /// </summary>
    private IEnumerator SlideInPauseUI()
    {
        pauseUI.SetActive(true);
        pauseCanvasGroup.alpha = 0f;
        pauseRectTransform.anchoredPosition = pauseUIStartPos;

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            pauseRectTransform.anchoredPosition = Vector2.Lerp(pauseUIStartPos, pauseUIEndPos, t);
            pauseCanvasGroup.alpha = t;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        pauseRectTransform.anchoredPosition = pauseUIEndPos;
        pauseCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// ポーズUIをスライドアウト非表示にするコルーチン
    /// </summary>
    private IEnumerator SlideOutPauseUI()
    {
        float elapsed = 0f;
        Vector2 startPos = pauseUIEndPos;
        Vector2 endPos = pauseUIHidePos;
        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            pauseRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            pauseCanvasGroup.alpha = 1f - t;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        pauseRectTransform.anchoredPosition = endPos;
        pauseCanvasGroup.alpha = 0f;
        pauseUI.SetActive(false);
    }

    /// <summary>
    /// ゲームを再開する（ポーズ解除）
    /// </summary>
    public void ResumeGame()
    {
        Debug.Log("ResumeGameボタンが押されました");
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pauseUI != null) StartCoroutine(SlideOutPauseUI());
            if (operationUI != null) operationUI.SetActive(false);
            if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
        }
    }

    /// <summary>
    /// 操作説明UIを表示する
    /// </summary>
    public void ShowOperationUI()
    {
        Debug.Log("ShowOperationUIボタンが押されました");
        if (pauseUI != null) pauseUI.SetActive(false);
        if (operationUI != null) operationUI.SetActive(true);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
    }

    /// <summary>
    /// 操作説明UIを非表示にし、ポーズUIを表示する
    /// </summary>
    public void HideOperationUI()
    {
        Debug.Log("HideOperationUIボタンが押されました");
        if (operationUI != null) operationUI.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(true);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
    }

    /// <summary>
    /// サウンド設定UIを表示する
    /// </summary>
    public void ShowSoundSettingsUI()
    {
        Debug.Log("ShowSoundSettingsUIボタンが押されました");
        if (pauseUI != null) pauseUI.SetActive(false);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(true);
        if (operationUI != null) operationUI.SetActive(false);
    }

    /// <summary>
    /// サウンド設定UIを非表示にし、ポーズUIを表示する
    /// </summary>
    public void HideSoundSettingsUI()
    {
        Debug.Log("HideSoundSettingsUIボタンが押されました");
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(true);
        if (operationUI != null) operationUI.SetActive(false);
    }

    /// <summary>
    /// ゲームをリトライする（シーン再読み込み）
    /// </summary>
    public void RetryGame()
    {
        Debug.Log("RetryGameボタンが押されました");
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseUI != null) pauseUI.SetActive(false);
        if (operationUI != null) operationUI.SetActive(false);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);

        // ScnenManagerがあればそちらでリトライ、なければSceneManagerで遷移
        var scnenManager = Object.FindFirstObjectByType<ScnenManager>();
        if (scnenManager != null && !string.IsNullOrEmpty(retrySceneName))
        {
            scnenManager.RetryScene(retrySceneName);
        }
        else if (!string.IsNullOrEmpty(retrySceneName))
        {
            SceneManager.LoadScene(retrySceneName);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// タイトル画面に戻る
    /// </summary>
    public void ReturnToTitle()
    {
        Debug.Log("ReturnToTitleボタンが押されました");
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseUI != null) pauseUI.SetActive(false);
        if (operationUI != null) operationUI.SetActive(false);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);

        // ScnenManagerがあればそちらでタイトル遷移、なければSceneManagerで遷移
        var scnenManager = Object.FindFirstObjectByType<ScnenManager>();
        if (scnenManager != null)
        {
            scnenManager.GoToTitleScene();
        }
        else
        {
            SceneManager.LoadScene("Title");
        }
    }
}