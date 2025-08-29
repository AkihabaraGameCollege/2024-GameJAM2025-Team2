using UnityEngine;

public class HeartGem : MonoBehaviour
{
    public GameObject heartGem;

    private void Start()
    {
        GetComponent<HeartGem>();
        heartGem.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ハートジェムを取った");
            heartGem.SetActive(false);
        }
    }
}
