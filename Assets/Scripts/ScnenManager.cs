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

    // SoundManagerの参照
    private SoundManager soundManager;

    void Start()
    {
        // SoundManagerをシーン内から取得
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Update()
    {
    }

    // Menuをアクティブ、Titleを非アクティブにするメソッド
    public void ShowMenuAndHideTitle()
    {
        if (menuUI != null) menuUI.SetActive(true);
        if (titleUI != null) titleUI.SetActive(false);

        // メニューBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayMenuBGM();
        }
    }

    // タイトルをアクティブ、メニューを非アクティブにするメソッド
    public void ShowTitleAndHideMenu()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (titleUI != null) titleUI.SetActive(true);

        // タイトルBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayTitleBGM();
        }
    }

    // ステージ選択画面UIを表示するメソッド
    public void ShowStageSelectAndHideMenu()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (stageSelectUI != null) stageSelectUI.SetActive(true);

        // ステージセレクトBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayStageSelectBGM();
        }
    }

    // ステージ1へ遷移する処理
    public void OnStage1ButtonClicked()
    {
        SceneManager.LoadScene("PlayerStageTestScene");

        // ステージBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayStageBGM();
        }
    }

    // ステージ2へ遷移する処理
    public void OnStage2ButtonClicked()
    {
        SceneManager.LoadScene("PlayerStage2Scene");

        // ステージBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayStageBGM();
        }
    }

    // ステージ3へ遷移する処理
    public void OnStage3ButtonClicked()
    {
        SceneManager.LoadScene("PlayerStage3Scene");

        // ステージBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayStageBGM();
        }
    }

    // ステージ選択画面からメニュー画面に戻るメソッド
    public void ShowMenuAndHideStageSelect()
    {
        if (stageSelectUI != null) stageSelectUI.SetActive(false);
        if (menuUI != null) menuUI.SetActive(true);

        // メニューBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayMenuBGM();
        }
    }

    // リザルト画面から次のステージシーンに遷移する仮実装メソッド
    public void GoToResultScene()
    {
        SceneManager.LoadScene("ResultScene");

        // リザルトBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayResultBGM();
        }
    }

    // タイトルシーンへ遷移し、元のシーン名を保存
    public void GoToTitleScene()
    {
        SceneManager.LoadScene("Title");

        // タイトルBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayTitleBGM();
        }
    }

    // 任意のシーン名でリトライできるメソッドを追加
    public void RetryScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

        // ステージBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayStageBGM();
        }
    }

    // 操作説明画面を表示し、メニューを非表示にするメソッド
    public void ShowHowToPlayAndHideMenu()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (howToPlayUI != null) howToPlayUI.SetActive(true);

        // メニューBGM再生（専用BGMがなければメニューBGM）
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayMenuBGM();
        }
    }

    // 操作説明画面からメニュー画面に戻るメソッド
    public void ShowMenuAndHideHowToPlay()
    {
        if (howToPlayUI != null) howToPlayUI.SetActive(false);
        if (menuUI != null) menuUI.SetActive(true);

        // メニューBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayMenuBGM();
        }
    }

    // サウンド設定画面を表示し、メニューを非表示にするメソッド
    public void ShowSoundSettingsAndHideMenu()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (soundSettingsUI != null) soundSettingsUI.SetActive(true);

        // メニューBGM再生（専用BGMがなければメニューBGM）
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayMenuBGM();
        }
    }

    // サウンド設定画面からメニュー画面に戻るメソッド
    public void ShowMenuAndHideSoundSettings()
    {
        Debug.Log("ShowMenuAndHideSoundSettingsが呼ばれました");
        if (soundSettingsUI != null) soundSettingsUI.SetActive(false);
        if (menuUI != null) menuUI.SetActive(true);

        // メニューBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayMenuBGM();
        }
    }

    // ボタンから呼び出すメソッド
    public void QuitGame()
    {
        Debug.Log("QuitGameボタンが押されました");
        Application.Quit();
    }
}