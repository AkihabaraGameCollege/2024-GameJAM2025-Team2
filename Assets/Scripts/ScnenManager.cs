using UnityEngine;
using UnityEngine.SceneManagement;

public class ScnenManager : MonoBehaviour
{
    // MenuとTitleの参照をInspectorで設定
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject titleUI;
    // 操作説明画面の参照をInspectorで設定
    [SerializeField] private GameObject howToPlayUI;
    // サウンド設定画面の参照をInspectorで設定
    [SerializeField] private GameObject soundSettingsUI;
    // ステージ選択画面UIを表示し、メニューを非表示にするメソッドに変更
    [SerializeField] private GameObject stageSelectUI;

    // サウンド管理用のSoundManager参照
    private SoundManager soundManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundManager = Object.FindFirstObjectByType<SoundManager>();
        // 初期状態に応じてBGMを再生
        if (titleUI != null && titleUI.activeSelf)
        {
            soundManager?.PlayTitleBGM();
            soundManager?.StopMenuBGM();
        }
        else if (menuUI != null && menuUI.activeSelf)
        {
            soundManager?.PlayMenuBGM();
            soundManager?.StopTitleBGM();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Menuをアクティブ、Titleを非アクティブにするメソッド
    public void ShowMenuAndHideTitle()
    {
        if (menuUI != null) menuUI.SetActive(true);
        if (titleUI != null) titleUI.SetActive(false);

        soundManager?.PlayMenuBGM();
        soundManager?.StopTitleBGM();
    }

    // タイトルをアクティブ、メニューを非アクティブにするメソッド
    public void ShowTitleAndHideMenu()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (titleUI != null) titleUI.SetActive(true);

        soundManager?.PlayTitleBGM();
        soundManager?.StopMenuBGM();
    }

    // ステージ選択画面UIを表示するメソッド
    public void ShowStageSelectAndHideMenu()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (stageSelectUI != null) stageSelectUI.SetActive(true);
    }

    // ステージ1へ遷移する処理
    // ステージ1へ遷移する処理
    public void OnStage1ButtonClicked()
    {
        soundManager?.PlayStageBGM();
        soundManager?.StopMenuBGM();
        soundManager?.StopTitleBGM();
        SceneManager.LoadScene("PlayerStageTestScene");
    }

    // ステージ2へ遷移する処理
    public void OnStage2ButtonClicked()
    {
        soundManager?.PlayStageBGM();
        soundManager?.StopMenuBGM();
        soundManager?.StopTitleBGM();
        SceneManager.LoadScene("PlayerStage2Scene");
    }

    // ステージ3へ遷移する処理
    public void OnStage3ButtonClicked()
    {
        soundManager?.PlayStageBGM();
        soundManager?.StopMenuBGM();
        soundManager?.StopTitleBGM();
        SceneManager.LoadScene("PlayerStage3Scene");
    }

    // ステージ選択画面からメニュー画面に戻るメソッド
    public void ShowMenuAndHideStageSelect()
    {
        if (stageSelectUI != null) stageSelectUI.SetActive(false);
        if (menuUI != null) menuUI.SetActive(true);
    }

    // リザルト画面から次のステージシーンに遷移する仮実装メソッド
    public void GoToResultScene()
    {
        soundManager?.StopStageBGM();      // まず停止
        soundManager?.PlayStageBGM();      // 再生（必ず頭から）
        soundManager?.StopMenuBGM();
        soundManager?.StopTitleBGM();
        SceneManager.LoadScene("ResultScene");
    }

    // タイトルシーンへ遷移し、元のシーン名を保存
    public void GoToTitleScene()
    {
        // タイトルシーンへ遷移
        SceneManager.LoadScene("Title");
    }

    // 任意のシーン名でリトライできるメソッドを追加
    public void RetryScene(string sceneName)
    {
        SceneManager.LoadScene("PlayerStageTestScene");
    }

    // 操作説明画面を表示し、メニューを非表示にするメソッド
    public void ShowHowToPlayAndHideMenu()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (howToPlayUI != null) howToPlayUI.SetActive(true);
    }

    // 操作説明画面からメニュー画面に戻るメソッド
    public void ShowMenuAndHideHowToPlay()
    {
        if (howToPlayUI != null) howToPlayUI.SetActive(false);
        if (menuUI != null) menuUI.SetActive(true);
    }

    // サウンド設定画面を表示し、メニューを非表示にするメソッド
    public void ShowSoundSettingsAndHideMenu()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(true);
    }

    // サウンド設定画面からメニュー画面に戻るメソッド
    public void ShowMenuAndHideSoundSettings()
    {
        Debug.Log("ShowMenuAndHideSoundSettingsが呼ばれました"); // ← 追加
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
        if (menuUI != null) menuUI.SetActive(true);
    }

    // ボタンから呼び出すメソッド
    public void QuitGame()
    {
        // ボタンが押されたことをデバッグログに出力
        Debug.Log("QuitGameボタンが押されました");
        // ビルド版でアプリケーションを終了
        Application.Quit();
    }
}
