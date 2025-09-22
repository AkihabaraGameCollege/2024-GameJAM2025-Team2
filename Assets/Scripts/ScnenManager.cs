using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    // リザルト画面の参照をInspectorで設定
    [SerializeField] private GameObject resultUI;

    // 各画面の最初に選択するボタンをInspectorで設定
    [SerializeField] private GameObject firstSelectedMenuButton;
    [SerializeField] private GameObject firstSelectedTitleButton;
    [SerializeField] private GameObject firstSelectedHowToPlayButton;
    [SerializeField] private GameObject firstSelectedSoundSettingsButton;
    [SerializeField] private GameObject firstSelectedStageSelectButton;
    [SerializeField] private GameObject firstSelectedResultButton;

    // SoundManagerの参照
    private SoundManager soundManager;

    void Start()
    {
        // SoundManagerをシーン内から取得
        soundManager = Object.FindFirstObjectByType<SoundManager>();

        // 最初のタイトル表示時にBGM再生（タイトル画面がアクティブな場合のみ再生）
        if (titleUI != null && titleUI.activeSelf)
        {
            soundManager?.StopAllBgmAudio();
            soundManager?.PlayTitleBGM();
        }
        // タイトル画面表示時に最初のボタンを選択
        SelectFirstButton(firstSelectedTitleButton);
    }

    void Update()
    {
    }

    // 指定したGameObjectを選択する共通メソッド
    private void SelectFirstButton(GameObject buttonObj)
    {
        if (buttonObj == null) return;
        EventSystem.current.SetSelectedGameObject(buttonObj);
    }

    // Menuをアクティブ、Titleを非アクティブにするメソッド
    public void ShowMenuAndHideTitle()
    {
        if (menuUI != null) menuUI.SetActive(true);
        if (titleUI != null) titleUI.SetActive(false);

        // コントローラー対応: 最初のボタンを選択
        SelectFirstButton(firstSelectedMenuButton);

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

        // コントローラー対応: 最初のボタンを選択
        SelectFirstButton(firstSelectedTitleButton);

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

        // コントローラー対応: 最初のボタンを選択
        SelectFirstButton(firstSelectedStageSelectButton);

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
        SceneManager.LoadScene("PlayerStage1Scene");

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

        // コントローラー対応: 最初のボタンを選択
        SelectFirstButton(firstSelectedMenuButton);

        // メニューBGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayMenuBGM();
        }
    }

    // リザルト画面を表示するメソッド（追加）
    public void ShowResultUI()
    {
        if (resultUI != null) resultUI.SetActive(true);

        // コントローラー対応: 最初のボタンを選択
        SelectFirstButton(firstSelectedResultButton);

        // リザルトBGM再生
        if (soundManager != null)
        {
            soundManager.StopAutoMoveAudio();
            soundManager.StopAllBgmAudio();
            soundManager.PlayResultBGM();
        }
    }

    // リザルト画面から次のステージシーンに遷移する仮実装メソッド
    public void GoToResultScene()
    {
        SceneManager.LoadScene("ResultScene");
        // リザルトBGM再生
        if (soundManager != null)
        {
            soundManager.StopAutoMoveAudio();
            soundManager.StopAllBgmAudio();
            soundManager.PlayResultBGM();
        }
    }

    // タイトルシーンへ遷移し、元のシーン名を保存
    public void GoToTitleScene()
    {
        SceneManager.LoadScene("Title");

        // コントローラー対応: 最初のボタンを選択
        SelectFirstButton(firstSelectedTitleButton);

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
        if (string.IsNullOrEmpty(sceneName))//とりあえずnullに
        {
            Debug.LogWarning("RetryScene:遷移できません");
            return;
        }

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

        // コントローラー対応: 最初のボタンを選択
        SelectFirstButton(firstSelectedHowToPlayButton);

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

        // コントローラー対応: 最初のボタンを選択
        SelectFirstButton(firstSelectedMenuButton);

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

        // コントローラー対応: 最初のボタンを選択
        SelectFirstButton(firstSelectedSoundSettingsButton);

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

        // コントローラー対応: 最初のボタンを選択
        SelectFirstButton(firstSelectedMenuButton);

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