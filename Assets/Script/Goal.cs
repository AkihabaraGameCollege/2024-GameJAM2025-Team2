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

            // ゴール時のSEを再生
            SoundManager soundManager = Object.FindFirstObjectByType<SoundManager>();
            if (soundManager != null)
            {
                soundManager.PlayGoalAudio();
                // ゴールSE再生後にシーン遷移
                StartCoroutine(WaitForGoalSE(soundManager));
            }
            else
            {
                // SoundManagerが見つからない場合は即シーン遷移
                SceneManager.LoadScene("ResultScene");
            }

            Debug.Log("ゴールにに到達");
        }
    }

    private System.Collections.IEnumerator WaitForGoalSE(SoundManager soundManager)
    {
        // goalAudioSourceがpublicなら直接参照、privateならSoundManagerにgetter追加推奨
        var goalAudioSourceField = typeof(SoundManager).GetField("goalAudioSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        AudioSource goalAudioSource = goalAudioSourceField?.GetValue(soundManager) as AudioSource;

        // goalAudioSourceが取得できた場合のみ待機
        if (goalAudioSource != null)
        {
            while (goalAudioSource.isPlaying)
            {
                yield return null;
            }
        }
        // SE再生終了後にシーン遷移
        SceneManager.LoadScene("ResultScene");
    }
}