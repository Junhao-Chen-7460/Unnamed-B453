using System;
using System.Collections.Generic;
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

    public int health;
    public int maxHealth = 4;

    [SerializeField] GameObject DmgEffect1;
    [SerializeField] GameObject DmgEffect2;
    [SerializeField] GameObject DmgEffect3;

    [Header("biu")]
    [SerializeField] GameObject redLaserPrefab;
    [SerializeField] GameObject blueLaserPrefab;
    [SerializeField] GameObject greenLaserPrefab;
    [SerializeField] GameObject yellowLaserPrefab;

    [SerializeField] float laserSpeed = 7f;

    [SerializeField] float fireDistance = 4f;
    [SerializeField] float fireCooldown = 1f;

    public int lvl = 0;

    private Transform turrent;
    private float fireTimer;

    Dictionary<string, List<string>> enemyDict = new Dictionary<string, List<string>>()
    {
        { "MRed",    new List<string>{ "MBlue", "MYellow", "MGreen", "BB", "BY", "BG" } },
        { "MBlue",   new List<string>{ "MRed", "MGreen", "MYellow", "BR", "BY", "BG" } },
        { "MGreen",  new List<string>{ "MRed", "MBlue", "MYellow", "BB", "BY", "BR" } },
        { "MYellow", new List<string>{ "MRed", "MBlue", "MGreen", "BB", "BR", "BG" } }
    };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        turrent = transform.Find("turrent");
        fireTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        FaceClosestEnemy();
        fireTimer += Time.deltaTime;
        TryFireAtClosestEnemy();
        FindClosestFlag();
        MoveTowardsFlag();
        
    }
    void FindClosestFlag()
    {
        string targetFlagTag = targetFlagPrefab.tag;
        GameObject[] allFlags = GameObject.FindGameObjectsWithTag(targetFlagTag);
        float closestDistance = Mathf.Infinity;
        targetFlag = null;

        foreach (GameObject flag in allFlags)
        {
            float distance = Vector2.Distance(transform.position, flag.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetFlag = flag;
            }

            Collider2D flagCol = flag.GetComponent<Collider2D>();
            if (flagCol != null)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), flagCol, true);
            }
        }

        if (targetFlag != null)
        {
            Collider2D flagCollider = targetFlag.GetComponent<Collider2D>();
            if (flagCollider != null)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), flagCollider);
            }
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

    public void HandleHealthVFX()
    {
        // if (health <= 0)
        // {
        //     Destroy(gameObject);
        // }

        float healthPercent = (float)health / maxHealth;
        if (healthPercent <= 1f / 3f)
        {
            DmgEffect1.SetActive(false);
            DmgEffect2.SetActive(false);
            DmgEffect3.SetActive(true);
        }
        else if (healthPercent <= 2f / 3f)
        {
            DmgEffect1.SetActive(false);
            DmgEffect2.SetActive(true);
            DmgEffect3.SetActive(false);
        }
        else if(healthPercent < 1f)
        {
            DmgEffect1.SetActive(true);
            DmgEffect2.SetActive(false);
            DmgEffect3.SetActive(false);
        }

    }

    // public void minusHealth(int number)
    // {
    //     health -= number;
    //     HandleHealth();
    // }
    void FaceClosestEnemy()
    {
        if (!enemyDict.ContainsKey(gameObject.tag)) return; 

        List<string> enemyTags = enemyDict[gameObject.tag];

        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                if (enemy == this.gameObject) continue;

                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        if (closestEnemy != null)
        {
            Vector2 direction = (closestEnemy.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }
    void TryFireAtClosestEnemy()
    {
        if (!enemyDict.ContainsKey(gameObject.tag)) return;

        List<string> enemyTags = enemyDict[gameObject.tag];
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                if (enemy == this.gameObject) continue;

                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        if (closestEnemy != null && closestDistance <= fireDistance && fireTimer >= fireCooldown)
        {
            fireTimer = 0f;
            FireLaser(closestEnemy.transform.position);
        }
    }

    void FireLaser(Vector3 targetPos)
    {
        GameObject laserPrefab = null;
        switch (gameObject.tag)
        {
            case "MRed": laserPrefab = redLaserPrefab; break;
            case "MBlue": laserPrefab = blueLaserPrefab; break;
            case "MGreen": laserPrefab = greenLaserPrefab; break;
            case "MYellow": laserPrefab = yellowLaserPrefab; break;
        }

        if (laserPrefab == null || turrent == null) return;

        Vector3 direction = (targetPos - turrent.position).normalized;
        GameObject laser = Instantiate(laserPrefab, turrent.position, Quaternion.identity);
        laser.GetComponent<Rigidbody2D>().linearVelocity = direction * laserSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laser.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        String bTag = null;
        switch (gameObject.tag)
        {
            case "MRed": bTag = "BR"; break;
            case "MBlue": bTag = "BB"; break;
            case "MGreen": bTag = "BG"; break;
            case "MYellow": bTag = "BY"; break;
        }

        int dmg = 1 + ((lvl + 1) / 2);

        laser.GetComponent<Laser>().ownerTag = gameObject.tag;
        laser.GetComponent<Laser>().baseTag = bTag;
        laser.GetComponent<Laser>().damage = dmg;
    }

    public void HandleLVL(int lvl)
    {
        maxHealth += lvl / 2;
        health = maxHealth;
        Transform lvlEffect = transform.Find("lvlEffect");
        if (lvlEffect == null)
        {
            Debug.LogWarning("lvlEffect not found");
            return;
        }

        for (int i = 0; i <= 9; i++)
        {
            Transform child = lvlEffect.Find(i.ToString());
            if (child != null)
            {
                child.gameObject.SetActive(i == lvl);
            }
        }
    }
}
