using UnityEngine;

public class HeartGem : MonoBehaviour
{
    public GameObject heartGem;

    private SoundManager soundManager;

    private void Start()
    {
        GetComponent<HeartGem>();
        heartGem.SetActive(true);

        // SoundManagerの取得
        soundManager = Object.FindFirstObjectByType<SoundManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ハートジェムを取った");
            heartGem.SetActive(false);

            // スコアアイテム獲得SEを再生
            if (soundManager != null)
            {
                soundManager.PlayScoreItemAudio();
            }
        }
    }
}
