using UnityEngine;

public class HeartGem : MonoBehaviour
{
    public GameObject heartGem;

    private SoundManager soundManager;
    private UIManager uiManager; // 追加

    private void Start()
    {
        GetComponent<HeartGem>();
        heartGem.SetActive(true);

        // SoundManagerの取得
        soundManager = Object.FindFirstObjectByType<SoundManager>();

        // UIManagerの取得
        uiManager = Object.FindFirstObjectByType<UIManager>();
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

            // スコア加算
            if (uiManager != null)
            {
                uiManager.AddScore(100);
            }
        }
    }
}
