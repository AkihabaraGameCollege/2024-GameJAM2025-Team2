using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("オーディオミキサーの設定")]
    [SerializeField]
    private AudioMixer audioMixer; // オーディオミキサー

    // タイトル画面用BGMのAudioSource
    [Header("タイトル画面BGM")]
    [SerializeField]
    private AudioSource titleBgmAudioSource;

    // メニュー画面用BGMのAudioSource
    [Header("メニュー画面BGM")]
    [SerializeField]
    private AudioSource menuBgmAudioSource;

    // ステージセレクト画面用BGMのAudioSource
    [Header("ステージセレクトBGM")]
    [SerializeField]
    private AudioSource stageSelectBgmAudioSource;

    // ステージ画面用BGMのAudioSource
    [Header("ステージ画面BGM")]
    [SerializeField]
    private AudioSource stageBgmAudioSource;

    // ポーズ画面用BGMのAudioSource
    [Header("ポーズ画面BGM")]
    [SerializeField]
    private AudioSource pauseBgmAudioSource;

    // リザルト画面用BGMのAudioSource
    [Header("リザルト画面BGM")]
    [SerializeField]
    private AudioSource resultBgmAudioSource;

    [Header("ゲームクリアのSE")]
    [SerializeField]
    private AudioSource gameClearAudioSource; // ゲームクリアの効果音

    [Header("クリック音のSE")]
    [SerializeField]
    private AudioSource clickAudioSource; // クリック音の効果音

    // 自動前進時のSE
    [Header("自動前進時のSE")]
    [SerializeField]
    private AudioSource autoMoveAudioSource; // 自動前進時の効果音

    // レーン移動時のSE
    [Header("レーン移動時のSE")]
    [SerializeField]
    private AudioSource laneMoveAudioSource; // レーン移動時の効果音

    // ジャンプ時のSE
    [Header("ジャンプ時のSE")]
    [SerializeField]
    private AudioSource jumpAudioSource; // ジャンプ時の効果音

    [Header("敵撃破時のSE")]
    [SerializeField]
    private AudioSource enemyDefeatAudioSource; // 敵撃破時の効果音

    // プレイヤーが障害物に激突（ミス）時のSE
    [Header("障害物激突時のSE")]
    [SerializeField]
    private AudioSource playerCrashAudioSource; // 障害物激突時の効果音

    // トリックアクションSE
    [Header("トリックアクション1のSE")]
    [SerializeField]
    private AudioSource trickAction1AudioSource; // トリックアクション1の効果音

    [Header("トリックアクション2のSE")]
    [SerializeField]
    private AudioSource trickAction2AudioSource; // トリックアクション2の効果音

    // スタート時カウントSE
    [Header("スタートカウント321のSE")]
    [SerializeField]
    private AudioSource startCount321AudioSource; // スタート時カウント321の効果音

    [Header("スタートカウントSTARTのSE")]
    [SerializeField]
    private AudioSource startCountStartAudioSource; // スタート時カウントSTARTの効果音

    // ゴール時のSE
    [Header("ゴール時のSE")]
    [SerializeField]
    private AudioSource goalAudioSource; // ゴール時の効果音

    // スコアアイテム獲得時のSE
    [Header("スコアアイテム獲得時のSE")]
    [SerializeField]
    private AudioSource scoreItemAudioSource; // スコアアイテム獲得時の効果音

    // 加速パネル判定時のSE
    [Header("加速パネル判定時のSE")]
    [SerializeField]
    private AudioSource accelPanelAudioSource; // 加速パネル判定時の効果音

    // UI選択音（クリック・コントローラー）
    [Header("UI選択音（クリック・コントローラー）")]
    [SerializeField]
    private AudioSource uiSelectAudioSource; // UI選択時の効果音

    // UI決定音
    [Header("UI決定音")]
    [SerializeField]
    private AudioSource uiDecideAudioSource; // UI決定時の効果音

    // UI音量設定つまみ移動時（SE確認）
    [Header("UI音量設定つまみ移動時（SE確認）")]
    [SerializeField]
    private AudioSource uiVolumeKnobAudioSource; // UI音量設定つまみ移動時の効果音

    [Header("マスター音量スライダー")]
    [SerializeField]
    private Slider masterVolumeSlider; // マスター音量スライダー

    [Header("SE音量スライダー")]
    [SerializeField]
    private Slider seVolumeSlider; // 効果音音量スライダー

    [Header("BGM音量スライダー")]
    [SerializeField]
    private Slider bgmVolumeSlider; // BGM音量スライダー


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        // 各AudioSourceの初期設定
        SetupAudioSource(titleBgmAudioSource);
        SetupAudioSource(menuBgmAudioSource);
        SetupAudioSource(stageSelectBgmAudioSource);
        SetupAudioSource(stageBgmAudioSource);
        SetupAudioSource(pauseBgmAudioSource);
        SetupAudioSource(resultBgmAudioSource);
        SetupAudioSource(gameClearAudioSource);
        SetupAudioSource(clickAudioSource);
        SetupAudioSource(autoMoveAudioSource);
        SetupAudioSource(laneMoveAudioSource);
        SetupAudioSource(jumpAudioSource);
        SetupAudioSource(enemyDefeatAudioSource);
        SetupAudioSource(playerCrashAudioSource);
        SetupAudioSource(trickAction1AudioSource);
        SetupAudioSource(trickAction2AudioSource);
        SetupAudioSource(startCount321AudioSource);
        SetupAudioSource(startCountStartAudioSource);
        SetupAudioSource(goalAudioSource);
        SetupAudioSource(scoreItemAudioSource);
        SetupAudioSource(accelPanelAudioSource);
        SetupAudioSource(uiSelectAudioSource);
        SetupAudioSource(uiDecideAudioSource);
        SetupAudioSource(uiVolumeKnobAudioSource);

        // マスター音量スライダーの設定
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            masterVolumeSlider.onValueChanged.AddListener(_ => PlayUIVolumeKnobAudio()); // 追加
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        }

        // 効果音音量スライダーの設定
        if (seVolumeSlider != null)
        {
            seVolumeSlider.onValueChanged.AddListener(SetSEVolume);
            seVolumeSlider.onValueChanged.AddListener(_ => PlayUIVolumeKnobAudio()); // 追加
            seVolumeSlider.value = PlayerPrefs.GetFloat("SEVolume", 1f);
        }

        // BGM音量スライダーの設定
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
            bgmVolumeSlider.onValueChanged.AddListener(_ => PlayUIVolumeKnobAudio()); // 追加
            bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        }

        // シーンをまたいで音量設定を適用
        ApplySavedVolumes();
    }

    private void ApplySavedVolumes()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetMasterVolume(masterVolume);

        float seVolume = PlayerPrefs.GetFloat("SEVolume", 1f);
        SetSEVolume(seVolume);

        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        SetBGMVolume(bgmVolume);
    }

    // タイトル画面BGMを再生
    public void PlayTitleBGM()
    {
        if (titleBgmAudioSource != null)
        {
            titleBgmAudioSource.loop = true;
            titleBgmAudioSource.Play();
        }
    }

    // メニュー画面BGMを再生
    public void PlayMenuBGM()
    {
        if (menuBgmAudioSource != null)
        {
            menuBgmAudioSource.loop = true;
            menuBgmAudioSource.Play();
        }
    }

    // ステージセレクト画面BGMを再生
    public void PlayStageSelectBGM()
    {
        if (stageSelectBgmAudioSource != null)
        {
            stageSelectBgmAudioSource.loop = true;
            stageSelectBgmAudioSource.Play();
        }
    }

    // ステージ画面BGMを再生
    public void PlayStageBGM()
    {
        if (stageBgmAudioSource != null)
        {
            stageBgmAudioSource.loop = true;
            stageBgmAudioSource.Play();
        }
    }

    // ポーズ画面BGMを再生
    public void PlayPauseBGM()
    {
        if (pauseBgmAudioSource != null)
        {
            pauseBgmAudioSource.loop = true;
            pauseBgmAudioSource.Play();
        }
    }

    // リザルト画面BGMを再生
    public void PlayResultBGM()
    {
        // ここにBGM停止処理
        if (resultBgmAudioSource != null)
        {
            resultBgmAudioSource.loop = true;
            resultBgmAudioSource.Play();
        }
    }

    // クリア時のSEを再生
    public void PlayGameClearAudio()
    {
        if (gameClearAudioSource != null)
        {
            gameClearAudioSource.Play();
        }
    }

    // BGMを停止（全BGM停止）
    public void StopAllBgmAudio()
    {
        
        {
            // 各BGM AudioSource を停止（nullチェック付き）
            if (titleBgmAudioSource != null) titleBgmAudioSource.Stop();
            if (menuBgmAudioSource != null) menuBgmAudioSource.Stop();
            if (stageSelectBgmAudioSource != null) stageSelectBgmAudioSource.Stop();
            if (stageBgmAudioSource != null) stageBgmAudioSource.Stop();
            if (pauseBgmAudioSource != null) pauseBgmAudioSource.Stop();
            if (resultBgmAudioSource != null) resultBgmAudioSource.Stop();
        }
    }

    // SEをすべて停止するメソッド
    public void StopAllSeAudio()
    {
        if (gameClearAudioSource != null) gameClearAudioSource.Stop();
        if (clickAudioSource != null) clickAudioSource.Stop();
        if (autoMoveAudioSource != null) autoMoveAudioSource.Stop();
        if (laneMoveAudioSource != null) laneMoveAudioSource.Stop();
        if (jumpAudioSource != null) jumpAudioSource.Stop();
        if (enemyDefeatAudioSource != null) enemyDefeatAudioSource.Stop();
        if (playerCrashAudioSource != null) playerCrashAudioSource.Stop();
        if (trickAction1AudioSource != null) trickAction1AudioSource.Stop();
        if (trickAction2AudioSource != null) trickAction2AudioSource.Stop();
        if (startCount321AudioSource != null) startCount321AudioSource.Stop();
        if (startCountStartAudioSource != null) startCountStartAudioSource.Stop();
        if (goalAudioSource != null) goalAudioSource.Stop();
        if (scoreItemAudioSource != null) scoreItemAudioSource.Stop();
        if (accelPanelAudioSource != null) accelPanelAudioSource.Stop();
        if (uiSelectAudioSource != null) uiSelectAudioSource.Stop();
        if (uiDecideAudioSource != null) uiDecideAudioSource.Stop();
        if (uiVolumeKnobAudioSource != null) uiVolumeKnobAudioSource.Stop();
    }

    // クリック音を再生
    public void PlayClickAudio()
    {
        if (clickAudioSource != null)
        {
            clickAudioSource.Play();
        }
    }

    // 自動前進時のSEを再生（ループ防止）
    public void PlayAutoMoveAudio()
    {
        if (autoMoveAudioSource != null && !autoMoveAudioSource.isPlaying)
        {
            autoMoveAudioSource.Play();
        }
    }

    // 自動前進時のSEを停止
    public void StopAutoMoveAudio()
    {
        if (autoMoveAudioSource != null && autoMoveAudioSource.isPlaying)
        {
            autoMoveAudioSource.Stop();
        }
    }

    // レーン移動時のSEを再生
    public void PlayLaneMoveAudio()
    {
        if (laneMoveAudioSource != null)
        {
            laneMoveAudioSource.Play();
        }
    }

    // ジャンプ時のSEを再生
    public void PlayJumpAudio()
    {
        if (jumpAudioSource != null)
        {
            jumpAudioSource.Play();
        }
    }

    // 敵撃破時のSEを再生
    public void PlayEnemyDefeatAudio()
    {
        if (enemyDefeatAudioSource != null)
        {
            enemyDefeatAudioSource.Play();
        }
    }

    // 障害物激突時のSEを再生
    public void PlayPlayerCrashAudio()
    {
        if (playerCrashAudioSource != null)
        {
            playerCrashAudioSource.Play();
        }
    }

    // トリックアクション1のSEを再生
    public void PlayTrickAction1Audio()
    {
        if (trickAction1AudioSource != null)
        {
            trickAction1AudioSource.Play();
        }
    }

    // トリックアクション2のSEを再生
    public void PlayTrickAction2Audio()
    {
        if (trickAction2AudioSource != null)
        {
            trickAction2AudioSource.Play();
        }
    }

    // スタート時カウント321のSEを再生
    public void PlayStartCount321Audio()
    {
        if (startCount321AudioSource != null)
        {
            startCount321AudioSource.Play();
        }
    }

    // スタート時カウントSTARTのSEを再生
    public void PlayStartCountStartAudio()
    {
        if (startCountStartAudioSource != null)
        {
            startCountStartAudioSource.Play();
        }
    }

    // ゴール時のSEを再生
    public void PlayGoalAudio()
    {
        if (goalAudioSource != null)
        {
            goalAudioSource.Play();
        }
    }

    // スコアアイテム獲得時のSEを再生
    public void PlayScoreItemAudio()
    {
        if (scoreItemAudioSource != null)
        {
            scoreItemAudioSource.Play();
        }
    }

    // 加速パネル判定時のSEを再生
    public void PlayAccelPanelAudio()
    {
        if (accelPanelAudioSource != null)
        {
            accelPanelAudioSource.Play();
        }
    }

    // UI選択音（クリック・コントローラー）を再生（ループ防止）
    public void PlayUISelectAudio()
    {
        if (uiSelectAudioSource != null && !uiSelectAudioSource.isPlaying)
        {
            uiSelectAudioSource.Play();
        }
    }

    // UI決定音を再生
    public void PlayUIDecideAudio()
    {
        if (uiDecideAudioSource != null)
        {
            uiDecideAudioSource.Play();
        }
    }

    // UI音量設定つまみ移動時（SE確認）を再生（ループ防止）
    public void PlayUIVolumeKnobAudio()
    {
        if (uiVolumeKnobAudioSource != null && !uiVolumeKnobAudioSource.isPlaying)
        {
            uiVolumeKnobAudioSource.Play();
        }
    }


    // BGM音量を設定（全BGMに適用）
    public void SetBGMVolume(float volume)
    {
        float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("BGMVolume", dbVolume);
            PlayerPrefs.SetFloat("BGMVolume", volume);
        }
        if (titleBgmAudioSource != null) titleBgmAudioSource.volume = volume;
        if (menuBgmAudioSource != null) menuBgmAudioSource.volume = volume;
        if (stageSelectBgmAudioSource != null) stageSelectBgmAudioSource.volume = volume;
        if (stageBgmAudioSource != null) stageBgmAudioSource.volume = volume;
        if (pauseBgmAudioSource != null) pauseBgmAudioSource.volume = volume;
        if (resultBgmAudioSource != null) resultBgmAudioSource.volume = volume;
    }

    // 効果音音量を設定（全SEに適用）
    public void SetSEVolume(float volume)
    {
        float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SEVolume", dbVolume);
            PlayerPrefs.SetFloat("SEVolume", volume);
        }
        if (gameClearAudioSource != null) gameClearAudioSource.volume = volume;
        if (clickAudioSource != null) clickAudioSource.volume = volume;
        if (autoMoveAudioSource != null) autoMoveAudioSource.volume = volume;
        if (laneMoveAudioSource != null) laneMoveAudioSource.volume = volume;
        if (jumpAudioSource != null) jumpAudioSource.volume = volume;
        if (enemyDefeatAudioSource != null) enemyDefeatAudioSource.volume = volume;
        if (playerCrashAudioSource != null) playerCrashAudioSource.volume = volume;
        if (trickAction1AudioSource != null) trickAction1AudioSource.volume = volume;
        if (trickAction2AudioSource != null) trickAction2AudioSource.volume = volume;
        if (startCount321AudioSource != null) startCount321AudioSource.volume = volume;
        if (startCountStartAudioSource != null) startCountStartAudioSource.volume = volume;
        if (goalAudioSource != null) goalAudioSource.volume = volume;
        if (scoreItemAudioSource != null) scoreItemAudioSource.volume = volume;
        if (accelPanelAudioSource != null) accelPanelAudioSource.volume = volume;
        if (uiSelectAudioSource != null) uiSelectAudioSource.volume = volume;
        if (uiDecideAudioSource != null) uiDecideAudioSource.volume = volume;
        if (uiVolumeKnobAudioSource != null) uiVolumeKnobAudioSource.volume = volume;
    }

    // マスター音量を設定
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            audioMixer.SetFloat("MasterVolume", dbVolume);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
    }

    // AudioSourceの初期設定
    private void SetupAudioSource(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            var seGroups = audioMixer.FindMatchingGroups("SEGroup");
            if (seGroups.Length > 0)
            {
                audioSource.outputAudioMixerGroup = seGroups[0];
            }
        }
    }

    // ゲーム終了時に呼ばれるメソッド
    private void OnApplicationQuit()
    {
        ResetSoundSettings();
    }

    // サウンド設定をリセットするメソッド
    private void ResetSoundSettings()
    {
        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("SEVolume");
        PlayerPrefs.DeleteKey("BGMVolume");
    }

    // ステージBGMのみ停止するメソッドを追加
    public void StopStageBGM()
    {
        if (stageBgmAudioSource != null)
        {
            stageBgmAudioSource.Stop();
        }
    }
}