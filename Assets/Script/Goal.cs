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
                StartCoroutine(WaitForGoalSE(soundManager));
            }
            else
            {
                SceneManager.LoadScene("ResultScene");
            }

            Debug.Log("ゴールに到達");
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