using UnityEngine;

public class Laser : MonoBehaviour
{
    public string ownerTag;
    [SerializeField] int damage;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Base"))
        {
            Destroy(gameObject);
        }
        else if (other.GetComponent<BillionController>() != null && !other.CompareTag(ownerTag))
        {
            other.GetComponent<BillionController>().minusHealth(damage);
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
}
