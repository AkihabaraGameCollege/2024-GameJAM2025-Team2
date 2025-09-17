using System.Collections;
using System.Collections.Generic;
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

    // スコア表示用スプライト（0〜9）
    [Header("スコア表示用スプライト（0〜9）")]
    [SerializeField] Sprite[] digitSprites = new Sprite[10];

    // スコア表示用Image（最大7桁分）
    [Header("スコア表示用Image（左から順に最大7個）")]
    [SerializeField] List<Image> scoreImages = new List<Image>();

    // カウントダウン開始秒数
    //[SerializeField] int countStartTime = 3;
    // 現在のカウントアップ時間（秒）
    float currentCountup = 0f;

    // カウントアップ中かどうかを示すフラグ
    public bool IsCountingup { get; private set; } = false;

    // 現在のスコア
    int currentScore = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CountdownAndStartCountup());
        // スコア初期化
        SetScore(0);
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
        // SoundManagerのインスタンス取得（新推奨方式）
        SoundManager soundManager = Object.FindFirstObjectByType<SoundManager>();

        Time.timeScale = 0f;

        countdownImage.sprite = sprite3;
        countdownImage.gameObject.SetActive(true);
        countupText.gameObject.SetActive(false);

        // 3
        soundManager?.PlayStartCount321Audio();
        yield return new WaitForSecondsRealtime(1f);

        // 2
        countdownImage.sprite = sprite2;
        soundManager?.PlayStartCount321Audio();
        yield return new WaitForSecondsRealtime(1f);

        // 1
        countdownImage.sprite = sprite1;
        soundManager?.PlayStartCount321Audio();
        yield return new WaitForSecondsRealtime(1f);

        // Start!!
        countdownImage.sprite = spriteStart;
        soundManager?.PlayStartCountStartAudio(); // START用SE
        yield return new WaitForSecondsRealtime(1f);

        countdownImage.gameObject.SetActive(false);

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

    // スコアをセットして表示を更新
    public void SetScore(int score)
    {
        currentScore = score;

        // 表示する桁数を決定（0なら1桁、以降スコアの桁数分表示、最大7桁）
        int digitCount = Mathf.Max(1, score.ToString().Length);
        digitCount = Mathf.Min(digitCount, scoreImages.Count);

        // 必要なImageだけ表示
        for (int i = 0; i < scoreImages.Count; i++)
            scoreImages[i].gameObject.SetActive(i < digitCount);

        // 各桁の数字をスプライトで表示
        for (int i = 0; i < digitCount; i++)
        {
            int digit = (score / (int)Mathf.Pow(10, digitCount - i - 1)) % 10;
            scoreImages[i].sprite = digitSprites[digit];
        }
    }

    // スコア加算例
    public void AddScore(int add)
    {
        SetScore(currentScore + add);
    }
}