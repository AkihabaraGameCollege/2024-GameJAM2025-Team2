using UnityEngine;

public class EnemyCon : MonoBehaviour
{
    public GameObject enemy = null;

    private void Start()
    {
        //GetComponent<EnemyCon>();
        //enemy.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("“G‚ª‚ ‚½‚Á‚½");
            //enemy.SetActive(false);
        }
    }
}
