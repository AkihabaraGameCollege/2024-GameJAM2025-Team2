using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 追加

/// <summary>
/// ゲーム全体のサウンド管理を行うクラス
/// BGM・各種SEの再生、音量設定、AudioMixer連携、設定保存などを担当
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("オーディオミキサーの設定")]
    [SerializeField] private AudioMixer audioMixer; // 全体の音量管理用ミキサー

    [Header("BGMのAudioSource")]
    [SerializeField] private AudioSource bgmAudioSource; // BGM再生用

    [Header("ゲームクリアのSE")]
    [SerializeField] private AudioSource gameClearAudioSource; // ゲームクリア時SE

    [Header("プレイヤー自動前進時のSE")]
    [SerializeField] private AudioSource playerAutoMoveAudioSource; // 自動前進SE

    [Header("プレイヤーレーン移動時のSE")]
    [SerializeField] private AudioSource playerLaneMoveAudioSource; // レーン移動SE

    [Header("ジャンプ時のSE")]
    [SerializeField] private AudioSource jumpAudioSource; // ジャンプSE

    [Header("敵・障害物に激突（ミス）時のSE")]
    [SerializeField] private AudioSource hitObstacleAudioSource; // 障害物衝突SE

    [Header("敵撃破時のSE")]
    [SerializeField] private AudioSource enemyDefeatAudioSource; // 敵撃破SE

    [Header("トリックアクション時のSE")]
    [SerializeField] private AudioSource trickActionAudioSource; // トリックアクションSE

    [Header("スタート時カウント321のSE")]
    [SerializeField] private AudioSource startCount321AudioSource; // カウント321SE

    [Header("スタート時カウントSTARTのSE")]
    [SerializeField] private AudioSource startCountStartAudioSource; // カウントSTART SE

    [Header("ゴール(クリア)時のSE")]
    [SerializeField] private AudioSource goalAudioSource; // ゴール到達SE

    [Header("スコアアイテム獲得時のSE")]
    [SerializeField] private AudioSource scoreItemGetAudioSource; // スコアアイテム取得SE

    [Header("加速パネル判定時のSE")]
    [SerializeField] private AudioSource accelPanelAudioSource; // 加速パネルSE

    [Header("選択音UIボタン(コントローラーやマウスで選択時)のSE")]
    [SerializeField] private AudioSource uiSelectAudioSource; // UI選択SE

    [Header("決定音UIボタン(コントローラーやマウスで選択時)のSE")]
    [SerializeField] private AudioSource uiSubmitAudioSource; // UI決定SE

    [Header("音量設定つまみ移動時（SE確認）のSE")]
    [SerializeField] private AudioSource volumeSliderMoveAudioSource; // 音量スライダー移動SE

    [Header("マスター音量スライダー")]
    [SerializeField] private Slider masterVolumeSlider; // マスター音量調整用

    [Header("SE音量スライダー")]
    [SerializeField] private Slider seVolumeSlider; // SE音量調整用

    [Header("BGM音量スライダー")]
    [SerializeField] private Slider bgmVolumeSlider; // BGM音量調整用

    // スライダーSEループ再生制御用
    private bool isVolumeSliderSEPlaying = false;

    /// <summary>
    /// 初期化処理。AudioSourceの設定やスライダーの初期値・リスナー登録を行う
    /// </summary>
    private void Start()
    {
        // 各AudioSourceの初期設定（AudioMixerGroup割り当て）
        SetupAudioSource(bgmAudioSource);
        SetupAudioSource(gameClearAudioSource);
        SetupAudioSource(playerAutoMoveAudioSource);
        SetupAudioSource(playerLaneMoveAudioSource);
        SetupAudioSource(jumpAudioSource);
        SetupAudioSource(hitObstacleAudioSource);
        SetupAudioSource(enemyDefeatAudioSource);
        SetupAudioSource(trickActionAudioSource);
        SetupAudioSource(startCount321AudioSource);
        SetupAudioSource(startCountStartAudioSource);
        SetupAudioSource(goalAudioSource);
        SetupAudioSource(scoreItemGetAudioSource);
        SetupAudioSource(accelPanelAudioSource);
        SetupAudioSource(uiSelectAudioSource);
        SetupAudioSource(uiSubmitAudioSource);
        SetupAudioSource(volumeSliderMoveAudioSource);

        // 各音量スライダーの初期値設定とイベント登録
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            AddSliderPointerEvents(masterVolumeSlider);
        }
        if (seVolumeSlider != null)
        {
            seVolumeSlider.onValueChanged.AddListener(SetSEVolume);
            seVolumeSlider.value = PlayerPrefs.GetFloat("SEVolume", 1f);
            AddSliderPointerEvents(seVolumeSlider);
        }
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
            bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
            AddSliderPointerEvents(bgmVolumeSlider);
        }

        // 保存済み音量設定を反映
        ApplySavedVolumes();
    }

    /// <summary>
    /// スライダーにPointerDown/PointerUpイベントを追加
    /// </summary>
    private void AddSliderPointerEvents(Slider slider)
    {
        var trigger = slider.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = slider.gameObject.AddComponent<EventTrigger>();

        // PointerDown
        var entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        entryDown.callback.AddListener((data) => OnSliderPointerDown());
        trigger.triggers.Add(entryDown);

        // PointerUp
        var entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entryUp.callback.AddListener((data) => OnSliderPointerUp());
        trigger.triggers.Add(entryUp);
    }

    /// <summary>
    /// スライダーを押したときにSEループ再生開始
    /// </summary>
    private void OnSliderPointerDown()
    {
        if (!isVolumeSliderSEPlaying && volumeSliderMoveAudioSource != null)
        {
            volumeSliderMoveAudioSource.loop = true;
            volumeSliderMoveAudioSource.Play();
            isVolumeSliderSEPlaying = true;
        }
    }

    /// <summary>
    /// スライダーを離したときにSE停止
    /// </summary>
    private void OnSliderPointerUp()
    {
        if (isVolumeSliderSEPlaying && volumeSliderMoveAudioSource != null)
        {
            volumeSliderMoveAudioSource.Stop();
            volumeSliderMoveAudioSource.loop = false;
            isVolumeSliderSEPlaying = false;
        }
    }

    /// <summary>
    /// 保存済みの音量設定をAudioMixerに反映
    /// </summary>
    private void ApplySavedVolumes()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetMasterVolume(masterVolume);

        float seVolume = PlayerPrefs.GetFloat("SEVolume", 1f);
        SetSEVolume(seVolume);

        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        SetBGMVolume(bgmVolume);
    }

    // --- 各種SE再生メソッド ---

    /// <summary>ゲームクリアSE再生</summary>
    public void PlayGameClearAudio()
    {
        if (gameClearAudioSource != null)
            gameClearAudioSource.Play();
    }

    /// <summary>プレイヤー自動前進SE再生</summary>
    public void PlayPlayerAutoMoveAudio()
    {
        if (playerAutoMoveAudioSource != null)
            playerAutoMoveAudioSource.Play();
    }

    /// <summary>プレイヤーレーン移動SE再生</summary>
    public void PlayPlayerLaneMoveAudio()
    {
        if (playerLaneMoveAudioSource != null)
            playerLaneMoveAudioSource.Play();
    }

    /// <summary>ジャンプSE再生</summary>
    public void PlayJumpAudio()
    {
        if (jumpAudioSource != null)
            jumpAudioSource.Play();
    }

    /// <summary>障害物衝突SE再生</summary>
    public void PlayHitObstacleAudio()
    {
        if (hitObstacleAudioSource != null)
            hitObstacleAudioSource.Play();
    }

    /// <summary>敵撃破SE再生</summary>
    public void PlayEnemyDefeatAudio()
    {
        if (enemyDefeatAudioSource != null)
            enemyDefeatAudioSource.Play();
    }

    /// <summary>トリックアクションSE再生</summary>
    public void PlayTrickActionAudio()
    {
        if (trickActionAudioSource != null)
            trickActionAudioSource.Play();
    }

    /// <summary>スタートカウント321SE再生</summary>
    public void PlayStartCount321Audio()
    {
        if (startCount321AudioSource != null)
            startCount321AudioSource.Play();
    }

    /// <summary>スタートカウントSTART SE再生</summary>
    public void PlayStartCountStartAudio()
    {
        if (startCountStartAudioSource != null)
            startCountStartAudioSource.Play();
    }

    /// <summary>ゴール到達SE再生</summary>
    public void PlayGoalAudio()
    {
        if (goalAudioSource != null)
            goalAudioSource.Play();
    }

    /// <summary>スコアアイテム取得SE再生</summary>
    public void PlayScoreItemGetAudio()
    {
        if (scoreItemGetAudioSource != null)
            scoreItemGetAudioSource.Play();
    }

    /// <summary>加速パネルSE再生</summary>
    public void PlayAccelPanelAudio()
    {
        if (accelPanelAudioSource != null)
            accelPanelAudioSource.Play();
    }

    /// <summary>UI選択SE再生</summary>
    public void PlayUISelectAudio()
    {
        if (uiSelectAudioSource != null)
            uiSelectAudioSource.Play();
    }

    /// <summary>UI決定SE再生</summary>
    public void PlayUISubmitAudio()
    {
        if (uiSubmitAudioSource != null)
            uiSubmitAudioSource.Play();
    }

    /// <summary>音量スライダー移動SE再生</summary>
    public void PlayVolumeSliderMoveAudio()
    {
        if (volumeSliderMoveAudioSource != null)
            volumeSliderMoveAudioSource.Play();
    }

    // --- BGM制御 ---

    /// <summary>BGM再生</summary>
    public void PlayBGM()
    {
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
            bgmAudioSource.Play();
    }

    /// <summary>BGM停止</summary>
    public void StopBGM()
    {
        if (bgmAudioSource != null)
            bgmAudioSource.Stop();
    }

    // --- 音量設定 ---

    /// <summary>BGM音量を設定し保存</summary>
    public void SetBGMVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            audioMixer.SetFloat("BGMVolume", dbVolume);
            PlayerPrefs.SetFloat("BGMVolume", volume);
        }
    }

    /// <summary>SE音量を設定し保存</summary>
    public void SetSEVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            audioMixer.SetFloat("SEVolume", dbVolume);
            PlayerPrefs.SetFloat("SEVolume", volume);
        }
    }

    /// <summary>マスター音量を設定し保存</summary>
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
            audioMixer.SetFloat("MasterVolume", dbVolume);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
    }

    /// <summary>
    /// AudioSourceにAudioMixerGroup(SEGroup)を割り当てる
    /// </summary>
    private void SetupAudioSource(AudioSource audioSource)
    {
        if (audioSource != null && audioMixer != null)
        {
            var seGroups = audioMixer.FindMatchingGroups("SEGroup");
            if (seGroups.Length > 0)
            {
                audioSource.outputAudioMixerGroup = seGroups[0];
            }
        }
    }

    /// <summary>
    /// アプリ終了時にサウンド設定をリセット
    /// </summary>
    private void OnApplicationQuit()
    {
        ResetSoundSettings();
    }

    /// <summary>
    /// サウンド設定(PlayerPrefs)をリセット
    /// </summary>
    private void ResetSoundSettings()
    {
        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("SEVolume");
        PlayerPrefs.DeleteKey("BGMVolume");
    }
}