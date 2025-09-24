using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

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
    private GameObject firstSelectedResultButton;

    // ネクストステージボタンの参照をInspectorで設定
    [SerializeField] private GameObject nextStageButton;

    // SoundManagerの参照
    private SoundManager soundManager;

    // UIとボタンを一元管理するDictionary
    private Dictionary<string, GameObject> uiDict;
    private Dictionary<string, GameObject> buttonDict;

    // 直前のステージ名を記憶する変数をstaticに変更（シーン間で値を保持するため）
    private static string lastStageSceneName;

    void Awake()
    {
        uiDict = new Dictionary<string, GameObject>
        {
            { "Menu", menuUI },
            { "Title", titleUI },
            { "HowToPlay", howToPlayUI },
            { "SoundSettings", soundSettingsUI },
            { "StageSelect", stageSelectUI },
            { "Result", resultUI }
        };
        buttonDict = new Dictionary<string, GameObject>
        {
            { "Menu", firstSelectedMenuButton },
            { "Title", firstSelectedTitleButton },
            { "HowToPlay", firstSelectedHowToPlayButton },
            { "SoundSettings", firstSelectedSoundSettingsButton },
            { "StageSelect", firstSelectedStageSelectButton },
            { "Result", firstSelectedResultButton }
        };
    }

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

        // リザルト画面の場合はShowResultUI()を呼ぶ
        if (resultUI != null && resultUI.activeSelf)
        {
            ShowResultUI();
        }
    }

    void Update()
    {
    }

    // 指定したGameObjectを選択する共通メソッド
    private void SelectFirstButton(GameObject buttonObj)
    {
        if (buttonObj == null)
        {
            Debug.LogWarning("FirstSelectedButtonが未設定です: " + buttonObj);
            return;
        }
        if (!buttonObj.activeInHierarchy)
        {
            Debug.LogWarning("FirstSelectedButtonが非アクティブです: " + buttonObj.name);
            return;
        }
        EventSystem.current.SetSelectedGameObject(buttonObj);
    }

    // 画面名でUIとボタンを切り替える汎用メソッド
    public void ShowUI(string name)
    {
        foreach (var ui in uiDict.Values) ui?.SetActive(false);
        if (uiDict.TryGetValue(name, out var targetUI)) targetUI?.SetActive(true);

        GameObject button = null;
        if (buttonDict.TryGetValue(name, out button) && button == null && name == "Result")
        {
            // Result画面のファーストセレクトボタンが未設定の場合、firstSelectedResultButtonを設定
            button = firstSelectedResultButton;
        }
        SelectFirstButton(button);

        // BGM再生（画面名で分岐）
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            switch (name)
            {
                case "Title": soundManager.PlayTitleBGM(); break;
                case "Menu": soundManager.PlayMenuBGM(); break;
                case "StageSelect": soundManager.PlayStageSelectBGM(); break;
                case "HowToPlay": soundManager.PlayHowToPlayBGM(); break;
                case "SoundSettings": soundManager.PlayMenuBGM(); break; // サウンド設定画面でBGM再生
                case "Result":
                    soundManager.StopAutoMoveAudio();
                    soundManager.PlayResultBGM();
                    break;
                default: break;
            }
        }
    }

    // Menuをアクティブ、Titleを非アクティブにするメソッド
    public void ShowMenuAndHideTitle()
    {
        ShowUI("Menu");
    }

    // タイトルをアクティブ、メニューを非アクティブにするメソッド
    public void ShowTitleAndHideMenu()
    {
        ShowUI("Title");
    }

    // ステージ選択画面UIを表示するメソッド
    public void ShowStageSelectAndHideMenu()
    {
        ShowUI("StageSelect");
    }

    // ステージ1へ遷移する処理
    public void OnStage1ButtonClicked()
    {
        lastStageSceneName = "PlayerStage1Scene"; // ステージ名を記憶
        SceneManager.LoadScene(lastStageSceneName);

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
        lastStageSceneName = "PlayerStage2Scene"; // ステージ名を記憶
        SceneManager.LoadScene(lastStageSceneName);

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
        lastStageSceneName = "PlayerStage3Scene"; // ステージ名を記憶
        SceneManager.LoadScene(lastStageSceneName);

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
        ShowUI("Menu");
    }

    // リザルト画面を表示するメソッド（修正）
    public void ShowResultUI()
    {
        Debug.Log("lastStageSceneName: " + lastStageSceneName);
        ShowUI("Result");

        // PlayerStage2Sceneをゴールした場合はネクストステージボタンを非表示
        if (nextStageButton != null)
        {
            if (lastStageSceneName == "PlayerStage2Scene")
                nextStageButton.SetActive(false);
            else
                nextStageButton.SetActive(true);
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

    // リザルト画面のリトライボタンから呼び出す用
    public void OnRetryButtonClicked()
    {
        RetryScene(lastStageSceneName);
    }

    // 操作説明画面を表示し、メニューを非表示にするメソッド
    public void ShowHowToPlayAndHideMenu()
    {
        ShowUI("HowToPlay");
    }

    // 操作説明画面からメニュー画面に戻るメソッド
    public void ShowMenuAndHideHowToPlay()
    {
        ShowUI("Menu");
    }

    // サウンド設定画面を表示し、メニューを非表示にするメソッド
    public void ShowSoundSettingsAndHideMenu()
    {
        ShowUI("SoundSettings");
    }

    // サウンド設定画面からメニュー画面に戻るメソッド
    public void ShowMenuAndHideSoundSettings()
    {
        Debug.Log("ShowMenuAndHideSoundSettingsが呼ばれました");
        ShowUI("Menu");
    }

    // ボタンから呼び出すメソッド
    public void QuitGame()
    {
        Debug.Log("QuitGameボタンが押されました");
        Application.Quit();
    }
}