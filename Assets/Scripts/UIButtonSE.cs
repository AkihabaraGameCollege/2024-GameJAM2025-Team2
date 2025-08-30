using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSE : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler
{
    private SoundManager soundManager;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    // 選択時（コントローラーやTab移動時）
    public void OnSelect(BaseEventData eventData)
    {
        soundManager?.PlayUISelectAudio();
    }

    // 決定時（コントローラーの決定ボタンやEnterキー）
    public void OnSubmit(BaseEventData eventData)
    {
        soundManager?.PlayUISubmitAudio();
    }

    // マウスクリック時
    public void OnPointerClick(PointerEventData eventData)
    {
        soundManager?.PlayUISubmitAudio();
    }
}