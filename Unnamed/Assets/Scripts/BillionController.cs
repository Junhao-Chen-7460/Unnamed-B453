using UnityEngine;

public class BillionController : MonoBehaviour
{

    private GameObject targetFlag;
    private Vector2 velocity = Vector2.zero;
    private float currentSpeed;

    [SerializeField] float acceleration = 3.5f;
    [SerializeField] float maxSpeed = 6f;
    [SerializeField] float stoppingDistance = 0.4f;
    [SerializeField] float deceleration = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindFlag();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToFlag();
    }

    void MoveToFlag()
    {
        if (targetFlag == null)
        {
            FindFlag();
            return;
        }

        Vector2 targetPos = targetFlag.transform.position;
        float distance = Vector2.Distance(transform.position, targetPos);

        if (distance < stoppingDistance)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * deceleration);
        }
        else
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        }
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * currentSpeed * Time.deltaTime);
    }
    void FindFlag()
    {
        GameObject[] flags = GameObject.FindGameObjectsWithTag(gameObject.tag);
        float closestDistance = Mathf.Infinity;

        foreach (GameObject flag in flags)
        {
            float distance = Vector2.Distance(transform.position, flag.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetFlag = flag;
            }
        }
    }
}
