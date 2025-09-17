using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // スコア表示用スプライト（0〜9）
    [Header("スコア表示用スプライト（0〜9）")]
    [SerializeField] Sprite[] digitSprites = new Sprite[10];

    // スコア表示用Image（最大7桁分）
    [Header("スコア表示用Image（左から順に最大7個）")]
    [SerializeField] List<Image> scoreImages = new List<Image>();

    // スコアプラス表示用Image
    [Header("スコアプラス表示用Image")]
    [SerializeField] Image scorePlusImage = null;

    // スコアプラス表示用Sprite
    [SerializeField] Sprite scorePlusSprite = null;

    // カウントアップ表示用スプライト（0〜9）
    [Header("カウントアップ表示用スプライト（0〜9）")]
    [SerializeField] Sprite[] countupDigitSprites = new Sprite[10];

    // カウントアップ表示用Image（左から順に最大4個など必要数）
    [Header("カウントアップ表示用Image（左から順に最大4個）")]
    [SerializeField] List<Image> countupImages = new List<Image>();

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

        // カウントアップスプライト初期化
        InitCountupSprites();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCountingup)
        {
            // 経過時間を加算
            currentCountup += Time.deltaTime;

            // カウントアップをスプライトで表示
            UpdateCountupSprites();
        }
    }

    IEnumerator CountdownAndStartCountup()
    {
        // SoundManagerのインスタンス取得（新推奨方式）
        SoundManager soundManager = Object.FindFirstObjectByType<SoundManager>();

        Time.timeScale = 0f;

        countdownImage.sprite = sprite3;
        countdownImage.gameObject.SetActive(true);
        SetCountupSpritesActive(false);

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
        // カウントアップスプライト表示
        SetCountupSpritesActive(true);
        UpdateCountupSprites();
    }

    // カウントアップを停止するメソッド
    public void StopCountup()
    {
        // カウントアップフラグを無効化
        IsCountingup = false;
        // カウントアップスプライト非表示
        SetCountupSpritesActive(false);
    }

    // カウントアップをリセットし、スプライトも初期化するメソッド
    public void ResetCountup()
    {
        // 経過時間をリセット
        currentCountup = 0f;
        // カウントアップスプライト初期化
        InitCountupSprites();
    }

    // スコアをセットして表示を更新
    public void SetScore(int score)
    {
        // スコアが0未満の場合は0にする
        currentScore = Mathf.Max(0, score);

        // 表示する桁数を決定（0なら1桁、以降スコアの桁数分表示、最大7桁）
        int digitCount = Mathf.Max(1, currentScore.ToString().Length);
        digitCount = Mathf.Min(digitCount, scoreImages.Count);

        // 必要なImageだけ表示
        for (int i = 0; i < scoreImages.Count; i++)
            scoreImages[i].gameObject.SetActive(i < digitCount);

        // 各桁の数字をスプライトで表示
        for (int i = 0; i < digitCount; i++)
        {
            int digit = (currentScore / (int)Mathf.Pow(10, digitCount - i - 1)) % 10;
            digit = Mathf.Clamp(digit, 0, digitSprites.Length - 1);
            scoreImages[i].sprite = digitSprites[digit];
        }

        // スコアプラスImageの位置調整（右隣に配置）
        if (scorePlusImage != null)
        {
            if (digitCount > 0)
            {
                RectTransform lastDigitRect = scoreImages[digitCount - 1].rectTransform;
                RectTransform plusRect = scorePlusImage.rectTransform;

                // 同じ親にしてローカル座標で配置
                plusRect.SetParent(lastDigitRect.parent, false);
                // 右隣に配置（X方向にサイズ分ずらす）
                plusRect.anchoredPosition = lastDigitRect.anchoredPosition + new Vector2(lastDigitRect.sizeDelta.x, 0);
            }
        }
    }

    // スコア加算
    public void AddScore(int add)
    {
        SetScore(currentScore + add);
        StartCoroutine(ShowScorePlusImage());
    }

    // スコアプラスImageを一時的に表示するコルーチン
    IEnumerator ShowScorePlusImage()
    {
        if (scorePlusImage != null && scorePlusSprite != null)
        {
            scorePlusImage.sprite = scorePlusSprite;
            scorePlusImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f); // 表示時間は調整可能
            scorePlusImage.gameObject.SetActive(false);
        }
    }

    // カウントアップ表示用スプライト初期化
    void InitCountupSprites()
    {
        // 全て非表示
        SetCountupSpritesActive(false);
        // 1の位のみ0で初期化
        if (countupImages.Count > 0)
        {
            countupImages[0].gameObject.SetActive(true);
            countupImages[0].sprite = countupDigitSprites[0];
        }
    }

    // カウントアップスプライトの表示/非表示制御
    void SetCountupSpritesActive(bool active)
    {
        for (int i = 0; i < countupImages.Count; i++)
            countupImages[i].gameObject.SetActive(active && i == 0); // 初期は1の位のみ
    }

    // カウントアップ値をスプライトで表示
    void UpdateCountupSprites()
    {
        // 小数点以下切り捨て（整数表示）
        int count = Mathf.FloorToInt(currentCountup);

        // 表示する桁数（1の位のみ→10,100で増加）
        int digitCount = 1;
        if (count >= 100) digitCount = 3;
        else if (count >= 10) digitCount = 2;

        digitCount = Mathf.Min(digitCount, countupImages.Count);

        // 必要なImageだけ表示
        for (int i = 0; i < countupImages.Count; i++)
            countupImages[i].gameObject.SetActive(i < digitCount);

        // 各桁の数字をスプライトで表示
        for (int i = 0; i < digitCount; i++)
        {
            int digit = (count / (int)Mathf.Pow(10, digitCount - i - 1)) % 10;
            countupImages[i].sprite = countupDigitSprites[digit];
        }
    }
}