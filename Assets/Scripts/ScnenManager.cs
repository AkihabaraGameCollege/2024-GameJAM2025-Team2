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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
    }

    // タイトルをアクティブ、メニューを非アクティブにするメソッド
    public void ShowTitleAndHideMenu()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (titleUI != null) titleUI.SetActive(true);
    }

    // ステージ選択画面UIを表示するメソッド
    public void ShowStageSelectAndHideMenu()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (stageSelectUI != null) stageSelectUI.SetActive(true);
    }

    // ステージ1へ遷移する処理
    public void OnStage1ButtonClicked()
    {
        // ロードシーンへ遷移
        SceneManager.LoadScene("PlayerStageTestScene");
    }

    // ステージ2へ遷移する処理
    public void OnStage2ButtonClicked()
    {
        // ステージ2のシーン名に合わせて変更してください
        SceneManager.LoadScene("PlayerStage2Scene");
    }

    // ステージ3へ遷移する処理
    public void OnStage3ButtonClicked()
    {
        // ステージ3のシーン名に合わせて変更してください
        SceneManager.LoadScene("PlayerStage3Scene");
    }

    // ステージ選択画面からメニュー画面に戻るメソッド
    public void ShowMenuAndHideStageSelect()
    {
        if (stageSelectUI != null) stageSelectUI.SetActive(false);
        if (menuUI != null) menuUI.SetActive(true);
    }

    // リザルト画面から次のステージシーンに遷移する仮実装メソッド
    public void GoToNextStage()
    {
        // 次のステージのシーン名を仮で指定
        SceneManager.LoadScene("NextStageScene");
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
