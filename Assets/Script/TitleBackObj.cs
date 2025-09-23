using UnityEngine;

public class TitleBackObj : MonoBehaviour
{
    [SerializeField] private GameObject comment;

    private void Start()
    {
        comment.SetActive(false);
    }

    public void SetCommentBGActive(bool active)
    {
        comment.SetActive(active);
    }
}
