using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // カウントダウン用スプライト（個別にアサイン）
    [Header("カウントダウン用スプライト")]
    [SerializeField] Sprite sprite3 = null;
    [SerializeField] Sprite sprite2 = null;
    [SerializeField] Sprite sprite1 = null;
    [SerializeField] Sprite spriteStart = null;
    // カウントダウンの画像を表示するImage
    [SerializeField] Image countdownImage = null;
    // カウントアップの時間を表示するTextMeshProUGUI
    [SerializeField] TextMeshProUGUI countupText = null;

    // カウントダウン開始秒数
    [SerializeField] int countStartTime = 3;
    // 現在のカウントアップ時間（秒）
    float currentCountup = 0f;

    // カウントアップ中かどうかを示すフラグ
    public bool IsCountingup { get; private set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CountdownAndStartCountup());
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
        }
    }

    IEnumerator CountdownAndStartCountup()
    {
        // タイムスケールを0にしてゲームを一時停止
        Time.timeScale = 0f;

        countdownImage.sprite = sprite3; // 追加：最初にスプライトを設定
        countdownImage.gameObject.SetActive(true);
        countupText.gameObject.SetActive(false);

        // ここで1秒待つ
        yield return new WaitForSecondsRealtime(1f);

        // 3
        countdownImage.sprite = sprite3;
        yield return new WaitForSecondsRealtime(1f);
        // 2
        countdownImage.sprite = sprite2;
        yield return new WaitForSecondsRealtime(1f);
        // 1
        countdownImage.sprite = sprite1;
        yield return new WaitForSecondsRealtime(1f);
        // Start!!
        countdownImage.sprite = spriteStart;
        yield return new WaitForSecondsRealtime(1f);

        countdownImage.gameObject.SetActive(false);

        // タイムスケールを1に戻してゲーム再開
        Time.timeScale = 1f;

        StartCountup();
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
