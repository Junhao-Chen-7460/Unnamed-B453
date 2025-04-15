using UnityEngine;

public class Laser : MonoBehaviour
{
    public string ownerTag;
    public string baseTag;
    public int damage;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else if (
            (other.GetComponent<BillionController>() != null || other.GetComponent<BaseController>() != null)
             && !other.CompareTag(ownerTag))
        {
            if (other.CompareTag(baseTag))
            {
                return;
            }
            if (other.GetComponent<BillionController>() != null)
            {
                other.GetComponent<BillionController>().minusHealth(damage);
            }
            else
            {
                other.GetComponent<BaseController>().minusHealth(damage);
            }
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
}
