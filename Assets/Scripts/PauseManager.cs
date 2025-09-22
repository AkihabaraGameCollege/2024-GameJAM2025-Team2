using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームのポーズ管理を行うクラス。
/// ポーズUI、操作説明UI、サウンド設定UIの表示切替や、ゲームの一時停止・再開、リトライ・タイトル画面への遷移を制御する。
/// </summary>
public class PauseManager : MonoBehaviour
{
    private PlayerInput playerInput; // プレイヤーの入力管理
    private InputAction pauseAction; // ポーズ用の入力アクション
    private bool isPaused = false;   // ゲームがポーズ中かどうか

    [SerializeField] private GameObject pauseUI;        // ポーズメニューUI
    [SerializeField] private GameObject operationUI;    // 操作説明UI
    [SerializeField] private GameObject soundSettingsUI;// サウンド設定UI
    [SerializeField] private string retrySceneName;     // リトライ時にロードするシーン名

    /// <summary>
    /// 初期化処理。PlayerInputの取得とポーズアクションのイベント登録、各UIの非表示化を行う。
    /// </summary>
    void Awake()
    {
        playerInput = Object.FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            pauseAction = playerInput.actions["Pause"];
            pauseAction.performed += OnPause; // ポーズ入力時のイベント登録
        }
        if (pauseUI != null) pauseUI.SetActive(false); // ポーズUI非表示
        if (operationUI != null) operationUI.SetActive(false); // 操作説明UI非表示
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false); // サウンド設定UI非表示
    }

    /// <summary>
    /// 終了処理。ポーズアクションのイベント登録解除。
    /// </summary>
    void OnDestroy()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPause;
        }
    }

    /// <summary>
    /// ポーズ入力時の処理。ゲームの一時停止・再開とUIの表示切替を行う。
    /// </summary>
    /// <param name="context">入力アクションのコンテキスト</param>
    private void OnPause(InputAction.CallbackContext context)
    {
        // エスケープキー判定
        var keyboard = Keyboard.current;
        if (isPaused && keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            ResumeGame(); // ポーズ中にエスケープキーで再開
            return;
        }

        isPaused = !isPaused; // ポーズ状態を反転
        Time.timeScale = isPaused ? 0f : 1f; // 時間の停止・再開
        if (pauseUI != null) pauseUI.SetActive(isPaused); // ポーズUI表示/非表示
        if (operationUI != null) operationUI.SetActive(false); // 操作説明UI非表示
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false); // サウンド設定UI非表示
    }

    /// <summary>
    /// ゲームを再開する。ポーズ状態解除と全UIの非表示化。
    /// </summary>
    public void ResumeGame()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pauseUI != null) pauseUI.SetActive(false);
            if (operationUI != null) operationUI.SetActive(false);
            if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
        }
    }

    /// <summary>
    /// 操作説明UIを表示し、他のUIを非表示にする。
    /// </summary>
    public void ShowOperationUI()
    {
        if (pauseUI != null) pauseUI.SetActive(false);
        if (operationUI != null) operationUI.SetActive(true);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
    }

    /// <summary>
    /// 操作説明UIを非表示にし、ポーズUIを再表示する。
    /// </summary>
    public void HideOperationUI()
    {
        if (operationUI != null) operationUI.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(true);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
    }

    /// <summary>
    /// ポーズUIからサウンド設定UIを表示し、他のUIを非表示にする。
    /// </summary>
    public void ShowSoundSettingsUI()
    {
        if (pauseUI != null) pauseUI.SetActive(false);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(true);
        if (operationUI != null) operationUI.SetActive(false);
    }

    /// <summary>
    /// サウンド設定UIを非表示にし、ポーズUIを再表示する。
    /// </summary>
    public void HideSoundSettingsUI()
    {
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
        if (pauseUI != null) pauseUI.SetActive(true);
        if (operationUI != null) operationUI.SetActive(false);
    }

    /// <summary>
    /// ゲームをリトライする。ポーズ解除・UI非表示後、指定シーンまたは現在のシーンを再ロードする。
    /// </summary>
    public void RetryGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseUI != null) pauseUI.SetActive(false);
        if (operationUI != null) operationUI.SetActive(false);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);

        var scnenManager = Object.FindFirstObjectByType<ScnenManager>();
        if (scnenManager != null && !string.IsNullOrEmpty(retrySceneName))
        {
            scnenManager.RetryScene(retrySceneName); // 独自のシーン管理があればそちらを使用
        }
        else if (!string.IsNullOrEmpty(retrySceneName))
        {
            SceneManager.LoadScene(retrySceneName); // 指定シーン名でリトライ
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 現在のシーンを再ロード
        }
    }

    /// <summary>
    /// ポーズメニューからタイトル画面に戻る。ポーズ解除・UI非表示後、タイトルシーンへ遷移。
    /// </summary>
    public void ReturnToTitle()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseUI != null) pauseUI.SetActive(false);
        if (operationUI != null) operationUI.SetActive(false);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);

        var scnenManager = Object.FindFirstObjectByType<ScnenManager>();
        if (scnenManager != null)
        {
            scnenManager.GoToTitleScene(); // 独自のタイトル遷移
        }
        else
        {
            SceneManager.LoadScene("Title"); // デフォルトのタイトルシーン名
        }
    }
}