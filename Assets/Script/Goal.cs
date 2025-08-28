using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("ResultScene");
            Debug.Log("ƒS[ƒ‹‚É‚É“’B");
        }
    }
}