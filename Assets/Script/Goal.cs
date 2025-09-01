using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ƒS[ƒ‹SEÄ¶
            SoundManager soundManager = Object.FindFirstObjectByType<SoundManager>();
            if (soundManager != null)
            {
                soundManager.PlayGoalAudio();
            }

            SceneManager.LoadScene("ResultScene");
            Debug.Log("ƒS[ƒ‹‚É‚É“’B");
        }
    }
}