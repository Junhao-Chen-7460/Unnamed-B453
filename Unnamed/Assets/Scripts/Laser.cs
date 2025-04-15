using UnityEngine;

public class Laser : MonoBehaviour
{
    public string ownerTag;
    public string baseTag;
    [SerializeField] int damage;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else if (other.GetComponent<BillionController>() != null && !other.CompareTag(ownerTag))
        {
            if (other.CompareTag(baseTag))
            {
                return;
            }
            other.GetComponent<BillionController>().minusHealth(damage);
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
}
