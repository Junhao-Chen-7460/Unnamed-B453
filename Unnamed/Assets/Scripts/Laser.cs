using UnityEngine;

public class Laser : MonoBehaviour
{
    public string ownerTag;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Base"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag(ownerTag))
        {
            return;
        }
        else if (other.GetComponent<BillionController>() != null)
        {
            other.GetComponent<BillionController>().minusHealth();
            Destroy(gameObject);
        }
    }
}
