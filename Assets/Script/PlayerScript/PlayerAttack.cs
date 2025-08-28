using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject rangeTrigger;

    public void Attack()
    {
        rangeTrigger.SetActive(true);
    }

    public void AttackFinished()
    {
        rangeTrigger.SetActive(false);
    }
}
