using UnityEngine;

public class BillionController : MonoBehaviour
{
    [SerializeField] GameObject targetFlagPrefab;

    private Rigidbody2D rb;
    private GameObject targetFlag;

    private float acceleration = 1f;
    private float maxSpeed = 5f;
    private float stoppingDistance = 0.5f;
    private float decelerationFactor = 0.98f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestFlag();
        MoveTowardsFlag();
    }
    void FindClosestFlag()
    {

        string targetFlagTag = targetFlagPrefab.tag;
        GameObject[] allFlags = GameObject.FindGameObjectsWithTag(targetFlagTag);
        if (allFlags.Length > 0)
        {
            Debug.Log(allFlags.Length);
        }
        float closestDistance = Mathf.Infinity;

        foreach (GameObject flag in allFlags)
        {
            float distance = Vector2.Distance(transform.position, flag.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetFlag = flag;
            }
        }

        Collider2D flagCollider = targetFlag.GetComponent<Collider2D>();

        if (flagCollider != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), flagCollider);
        }
    }

    void MoveTowardsFlag()
    {

        if (targetFlag == null) return;

        Vector2 direction = (targetFlag.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetFlag.transform.position);

        if (distance > stoppingDistance)
        {
            float speedFactor = Mathf.Clamp(distance / 5f, 0.5f, 1f);
            float targetSpeed = Mathf.Min(rb.linearVelocity.magnitude + acceleration * speedFactor, maxSpeed);

            rb.AddForce(direction * acceleration, ForceMode2D.Force);

            if (rb.linearVelocity.magnitude > targetSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * targetSpeed;
            }
        }
        else
        {
            rb.linearVelocity *= decelerationFactor;
            if (rb.linearVelocity.magnitude < 0.01f)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(gameObject.tag))
        {
            Rigidbody2D otherRb = collision.rigidbody;
            if (otherRb != null)
            {
                Vector2 pushDirection = (otherRb.position - rb.position).normalized;
                float pushForce = 0.02f;


                rb.AddForce(-pushDirection * pushForce, ForceMode2D.Impulse);
                otherRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }
        }
    }
}
