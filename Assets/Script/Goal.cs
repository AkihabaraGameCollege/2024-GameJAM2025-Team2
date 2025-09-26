using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private bool isGoalTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isGoalTriggered)
        {
            isGoalTriggered = true;

            // カウントアップ停止
            UIManager uiManager = Object.FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.StopCountup();

                // スコアとゴールタイム保存
                PlayerPrefs.SetInt("ResultScore", uiManager.GetCurrentScore());
                PlayerPrefs.SetInt("ResultTime", Mathf.FloorToInt(uiManager.GetCurrentCountup()));
            }

            // ゴール時のSEを再生
            SoundManager soundManager = Object.FindFirstObjectByType<SoundManager>();
            if (soundManager != null)
            {
                soundManager.StopStageBGM();
                soundManager.StopAutoMoveAudio();
                soundManager.PlayGoalAudio();
                // 直前のステージ名を保存（リザルト画面のUI制御用）
                SaveLastStageSceneName();
                StartCoroutine(WaitForGoalSE(soundManager));
            }
            else
            {
                // 直前のステージ名を保存（リザルト画面のUI制御用）
                SaveLastStageSceneName();
                SceneManager.LoadScene("ResultScene");
            }

            // プレイヤーのぶれ防止
            var playerCon = other.GetComponent<PlayerCon>();
            if (playerCon != null)
            {
                playerCon.OnGoalReached();
                playerCon.SetControlEnabled(false); // ← 追加：操作無効化
            }

            // ポーズ禁止フラグをセット
            PauseManager pauseManager = Object.FindFirstObjectByType<PauseManager>();
            if (pauseManager != null)
            {
                pauseManager.SetGoalReached();
            }

            Debug.Log("ゴールに到達");
        }
    }

    // 直前のステージ名を保存するメソッド
    private void SaveLastStageSceneName()
    {
        // ScnenManagerのstatic変数に保存
        var scnenManagerType = typeof(ScnenManager);
        var field = scnenManagerType.GetField("lastStageSceneName", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        if (field != null)
        {
            field.SetValue(null, SceneManager.GetActiveScene().name);
        }
    }

    private System.Collections.IEnumerator WaitForGoalSE(SoundManager soundManager)
    {
        var goalAudioSourceField = typeof(SoundManager).GetField("goalAudioSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        AudioSource goalAudioSource = goalAudioSourceField?.GetValue(soundManager) as AudioSource;

        if (goalAudioSource != null)
        {
            while (goalAudioSource.isPlaying)
            {
                yield return null;
            }
        }
        // リザルト画面遷移時にBGMを再生
        SceneManager.LoadScene("ResultScene");
        var resultSoundManager = Object.FindFirstObjectByType<SoundManager>();
        resultSoundManager?.StopAutoMoveAudio();
        resultSoundManager?.StopAllBgmAudio();
        resultSoundManager?.PlayResultBGM();
    }
}