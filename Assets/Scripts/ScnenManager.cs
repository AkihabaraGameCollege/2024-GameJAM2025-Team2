using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
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
    // リザルト画面：ネクストステージボタン（Inspectorでアサイン）
    [SerializeField] private GameObject nextStageButton;

    // 各画面の最初に選択するボタンをInspectorで設定
    [SerializeField] private GameObject firstSelectedMenuButton;
    [SerializeField] private GameObject firstSelectedTitleButton;
    [SerializeField] private GameObject firstSelectedHowToPlayButton;
    [SerializeField] private GameObject firstSelectedSoundSettingsButton;
    [SerializeField] private GameObject firstSelectedStageSelectButton;

    // リザルト画面の最初に選択するボタン（ステージ1/2用、ステージ3用を別々に設定可能）
    [SerializeField] private GameObject firstSelectedResultButtonForNormalStages; // ステージ1・2
    [SerializeField] private GameObject firstSelectedResultButtonForFinalStage;   // ステージ3（最終）

    // 各シーンのUIに配置したスライダーをInspectorでアサイン
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider seVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;

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
            { "StageSelect", firstSelectedStageSelectButton }
            // Result は直前ステージに応じて動的に決定
        };
    }

    void Start()
    {
        // SoundManagerをシーン内から取得
        soundManager = Object.FindFirstObjectByType<SoundManager>();

        // 音量スライダーの初期値とイベント登録
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            masterVolumeSlider.onValueChanged.AddListener(soundManager.SetMasterVolume);
            masterVolumeSlider.onValueChanged.AddListener(_ => soundManager.PlayUIVolumeKnobAudio());
        }
        if (seVolumeSlider != null)
        {
            seVolumeSlider.value = PlayerPrefs.GetFloat("SEVolume", 1f);
            seVolumeSlider.onValueChanged.AddListener(soundManager.SetSEVolume);
            seVolumeSlider.onValueChanged.AddListener(_ => soundManager.PlayUIVolumeKnobAudio());
        }
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
            bgmVolumeSlider.onValueChanged.AddListener(soundManager.SetBGMVolume);
            bgmVolumeSlider.onValueChanged.AddListener(_ => soundManager.PlayUIVolumeKnobAudio());
        }

        // 最初のタイトル表示時
        if (titleUI != null && titleUI.activeSelf)
        {
            soundManager?.StopAllBgmAudio();
            soundManager?.PlayTitleBGM();
            SelectFirstButton(firstSelectedTitleButton);
        }

        // Resultがアクティブなら初期選択と可視性制御
        if (resultUI != null && resultUI.activeSelf)
        {
            UpdateNextStageButtonVisibility();
            SelectFirstButton(GetResultFirstSelectedButton());
        }
    }

    void Update()
    {
    }

    // 指定したGameObjectを選択する共通メソッド（1フレーム遅延）
    private void SelectFirstButton(GameObject buttonObj)
    {
        if (buttonObj == null)
        {
            Debug.LogWarning("FirstSelectedButtonが未設定です");
            return;
        }
        if (!buttonObj.activeInHierarchy)
        {
            Debug.LogWarning("FirstSelectedButtonが非アクティブです: " + buttonObj.name);
            return;
        }
        if (EventSystem.current == null)
        {
            Debug.LogWarning("EventSystemが見つかりません。選択できません。");
            return;
        }
        StartCoroutine(SetSelectedNextFrame(buttonObj));
    }

    private IEnumerator SetSelectedNextFrame(GameObject buttonObj)
    {
        yield return null; // UIのアクティブ化後に1フレーム待つ
        if (buttonObj != null && buttonObj.activeInHierarchy && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonObj);
        }
    }

    // 画面名でUIとボタンを切り替える汎用メソッド
    public void ShowUI(string name)
    {
        foreach (var ui in uiDict.Values) ui?.SetActive(false);
        if (uiDict.TryGetValue(name, out var targetUI)) targetUI?.SetActive(true);

        GameObject button = null;
        if (name == "Result")
        {
            button = GetResultFirstSelectedButton();
        }
        else
        {
            buttonDict.TryGetValue(name, out button);
        }

        SelectFirstButton(button);

        // BGM再生
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            switch (name)
            {
                case "Title": soundManager.PlayTitleBGM(); break;
                case "Menu": soundManager.PlayMenuBGM(); break;
                case "StageSelect": soundManager.PlayStageSelectBGM(); break;
                case "HowToPlay": soundManager.PlayHowToPlayBGM(); break;
                case "SoundSettings": soundManager.PlayMenuBGM(); break;
                case "Result":
                    soundManager.StopAutoMoveAudio();
                    soundManager.PlayResultBGM();
                    UpdateNextStageButtonVisibility();
                    break;
                default: break;
            }
        }
        else
        {
            if (name == "Result")
            {
                UpdateNextStageButtonVisibility();
            }
        }
    }

    // ステージに応じてResultの初期選択ボタンを返す
    private GameObject GetResultFirstSelectedButton()
    {
        // 最終ステージなら最終用、1/2なら通常用。片方未設定時はもう片方をフォールバック。
        if (lastStageSceneName == "PlayerStage3Scene")
        {
            return firstSelectedResultButtonForFinalStage ?? firstSelectedResultButtonForNormalStages;
        }
        return firstSelectedResultButtonForNormalStages ?? firstSelectedResultButtonForFinalStage;
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
        lastStageSceneName = "PlayerStage1Scene";
        SceneManager.LoadScene(lastStageSceneName);
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayStageBGM();
        }
    }

    // ステージ2へ遷移する処理
    public void OnStage2ButtonClicked()
    {
        lastStageSceneName = "PlayerStage2Scene";
        SceneManager.LoadScene(lastStageSceneName);
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayStageBGM();
        }
    }

    // ステージ3へ遷移する処理（最終）
    public void OnStage3ButtonClicked()
    {
        lastStageSceneName = "PlayerStage3Scene";
        SceneManager.LoadScene(lastStageSceneName);
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

    // リザルト画面を表示するメソッド
    public void ShowResultUI()
    {
        ShowUI("Result");
    }

    // リザルト画面から次のステージシーンに遷移する仮実装メソッド
    public void GoToResultScene()
    {
        SceneManager.LoadScene("ResultScene");
        if (soundManager != null)
        {
            soundManager.StopAutoMoveAudio();
            soundManager.StopAllBgmAudio();
            soundManager.PlayResultBGM();
        }
        UpdateNextStageButtonVisibility();
    }

    // タイトルシーンへ遷移
    public void GoToTitleScene()
    {
        SceneManager.LoadScene("Title");
        SelectFirstButton(firstSelectedTitleButton);
        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayTitleBGM();
        }
    }

    // 任意のシーン名でリトライ
    public void RetryScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("RetryScene:遷移できません");
            return;
        }

        SceneManager.LoadScene(sceneName);
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

    // 操作説明画面の表示/非表示
    public void ShowHowToPlayAndHideMenu()
    {
        ShowUI("HowToPlay");
    }

    public void ShowMenuAndHideHowToPlay()
    {
        ShowUI("Menu");
    }

    // サウンド設定画面の表示/非表示
    public void ShowSoundSettingsAndHideMenu()
    {
        ShowUI("SoundSettings");
    }

    public void ShowMenuAndHideSoundSettings()
    {
        Debug.Log("ShowMenuAndHideSoundSettingsが呼ばれました");
        ShowUI("Menu");
    }

    // 終了
    public void QuitGame()
    {
        Debug.Log("QuitGameボタンが押されました");
        Application.Quit();
    }

    // ネクストステージボタンの表示制御
    private void UpdateNextStageButtonVisibility()
    {
        if (nextStageButton == null)
        {
            return;
        }

        bool showNext = lastStageSceneName == "PlayerStage1Scene"
                        || lastStageSceneName == "PlayerStage2Scene";
        nextStageButton.SetActive(showNext);
    }

    public void OnNextStageButtonClicked()
    {
        string next = null;
        switch (lastStageSceneName)
        {
            case "PlayerStage1Scene":
                next = "PlayerStage2Scene";
                break;
            case "PlayerStage2Scene":
                next = "PlayerStage3Scene";
                break;
            default:
                Debug.LogWarning("次のステージが定義されていません: " + lastStageSceneName);
                return;
        }

        SceneManager.LoadScene(next);

        if (soundManager != null)
        {
            soundManager.StopAllBgmAudio();
            soundManager.PlayStageBGM();
        }
    }
}