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

            // ƒS[ƒ‹‚ÌSE‚ğÄ¶
            SoundManager soundManager = Object.FindFirstObjectByType<SoundManager>();
            if (soundManager != null)
            {
                // BGM’â~‚ğ’Ç‰Á
                soundManager.StopStageBGM();

                soundManager.PlayGoalAudio();
                // ƒS[ƒ‹SEÄ¶Œã‚ÉƒV[ƒ“‘JˆÚ
                StartCoroutine(WaitForGoalSE(soundManager));
            }
            else
            {
                // SoundManager‚ªŒ©‚Â‚©‚ç‚È‚¢ê‡‚Í‘¦ƒV[ƒ“‘JˆÚ
                SceneManager.LoadScene("ResultScene");
            }

            Debug.Log("ƒS[ƒ‹‚É“’B");
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
}