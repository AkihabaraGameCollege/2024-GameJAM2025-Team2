using UnityEngine;

public class EnemyCon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log($"{name} ‚ª {other.name} ‚ğUŒ‚");
        }
    }
}
