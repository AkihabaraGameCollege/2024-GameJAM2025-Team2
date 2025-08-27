using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameDirectory : MonoBehaviour
{
    // カウントアップの時間を表示するTextMeshProUGUI
    [SerializeField] TextMeshProUGUI countupText = null;

    // 現在のカウントアップ時間（秒）
    float currentCountup = 0f;

    // カウントアップ中かどうかを示すフラグ
    public bool IsCountingup { get; private set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCountup();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCountingup)
        {
            // 経過時間を加算
            currentCountup += Time.deltaTime;
            // テキストに現在の時間を表示（小数点第一位まで、3桁表示）
            countupText.text = "Time : " + currentCountup.ToString("000.0") + " s";
            Debug.Log("Countup: " + currentCountup); // 追加
        }
    }

    // カウントアップを開始するメソッド
    public void StartCountup()
    {
        // 経過時間をリセット
        currentCountup = 0f;
        // カウントアップフラグを有効化
        IsCountingup = true;
        // テキストオブジェクトが存在すれば表示する
        if (countupText != null)
            countupText.gameObject.SetActive(true);
    }

    // カウントアップを停止するメソッド
    public void StopCountup()
    {
        // カウントアップフラグを無効化
        IsCountingup = false;
    }

    // カウントアップをリセットし、テキストも初期化するメソッド
    public void ResetCountup()
    {
        // 経過時間をリセット
        currentCountup = 0f;
        // テキストを初期状態に戻す
        countupText.text = "Time : 000.0 s";
    }
}
