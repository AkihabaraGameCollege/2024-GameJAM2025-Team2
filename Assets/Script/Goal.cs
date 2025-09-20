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
                // BGM停止を追加
                soundManager.StopStageBGM();

                soundManager.PlayGoalAudio();
                // ゴールSE再生後にシーン遷移
                StartCoroutine(WaitForGoalSE(soundManager));
            }
            else
            {
                // SoundManagerが見つからない場合は即シーン遷移
                LoadNextScene();
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
        SceneManager.LoadScene("ResultScene");
    }

    //ステージ２の追加で変更
    private void LoadNextScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "PlayerStage1Scene")
        {
            SceneManager.LoadScene("PlayerStage2Scene");
        }
        else if (currentScene == "PlayerStage2Scene")
        {
            SceneManager.LoadScene("ResultScene");
        }
    }
}