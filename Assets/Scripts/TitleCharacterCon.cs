using UnityEngine;

public class TitleCharacterCon : MonoBehaviour
{

    [SerializeField] private Animator characterAnimator = null;
    private float timer;

    [Header("別アニメーションの間隔")]
    [SerializeField] private float minInterval = 5f;
    [SerializeField] private float maxInterval = 15f;

    [Header("アニメーションのトリガー名リスト")]
    [SerializeField]
    private string[] animationTriggers
        = { "Squad", "LookingUI" };

    [Header("UIごとの位置")]
    [SerializeField] private Transform titlePosition;
    [SerializeField] private Transform stageSelectPosition;

    [Header("キャラクターオブジェクト")]
    [SerializeField] private GameObject characterModel;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        ExAnim();
    }

    void ExAnim()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            int index = Random.Range(0, animationTriggers.Length);
            characterAnimator.SetTrigger(animationTriggers[index]);

            ResetTimer();
        }
    }

    void ResetTimer()
    {
        timer = Random.Range(minInterval, maxInterval);
    }

    #region UIごとのキャラの位置
    public void SetToTitlePos()
    {
        transform.position = titlePosition.position;
        transform.rotation = titlePosition.rotation;
    }

    public void SetToNextUIPos()
    {
        transform.position = stageSelectPosition.position;
        transform.rotation = stageSelectPosition.rotation;
    }

    public void SetActiveCharacter(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    #endregion
}
