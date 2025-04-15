using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Mathematics;

public class BaseController : MonoBehaviour
{
    [SerializeField] GameObject billions;
    [SerializeField] float SpawnTime;

    [Header("Boom")]
    [SerializeField] GameObject redLaserPrefab;
    [SerializeField] GameObject blueLaserPrefab;
    [SerializeField] GameObject greenLaserPrefab;
    [SerializeField] GameObject yellowLaserPrefab;

    [SerializeField] float laserSpeed = 7f;

    [SerializeField] float fireDistance = 4f;
    [SerializeField] float fireCooldown = 1f;
    [SerializeField] GameObject turrentBase;
    [SerializeField] GameObject turrent;
    [SerializeField] float maxRotationSpeed = 30f;

    private float fireTimer;
    private float timer;
    private Vector3 SpawnPoint;

    private Rigidbody2D rb;
    private Transform turrentGun;
    private Transform shotPoint;

    [SerializeField] int maxHealth = 20;
    private int health;
    [SerializeField] LifeRing lifeRing;

    public int lvl = 0;

    private float timer1;
    private float upgradeTimer = 10f;

    public int curExp = 0;
    public int maxExp = 5;
    Dictionary<string, List<string>> enemyDict = new Dictionary<string, List<string>>()
    {
        { "TRed",    new List<string>{ "MBlue", "MYellow", "MGreen" } },
        { "TBlue",   new List<string>{ "MRed", "MGreen", "MYellow" } },
        { "TGreen",  new List<string>{ "MRed", "MBlue", "MYellow" } },
        { "TYellow", new List<string>{ "MRed", "MBlue", "MGreen" } }
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        getSpawnPoint();
        fireTimer = 0f;

        if (turrent != null)
        {
            turrentGun = turrent.transform.Find("TurrentGun");
            shotPoint = turrentGun.Find("shotPoint");
        }

        health = maxHealth;
        lifeRing.Init(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        FaceClosestEnemy();
        fireTimer += Time.deltaTime;
        TryFireAtClosestEnemy();
        
        timer += Time.deltaTime;
        timer1 += Time.deltaTime;
        if (timer >= SpawnTime)
        {
            timer = 0f;
            SpawnBillion();
        }

        if (timer1 >= upgradeTimer)
        {
            timer1 = 0f;
            Upgrade();
        }
    }

    private void getSpawnPoint()
    {
        SpawnPoint = transform.position + new Vector3(0.5f , -0.5f, 0f);
    }
    private void SpawnBillion()
    {
        float randX = UnityEngine.Random.Range(SpawnPoint.x - 0.1f, SpawnPoint.x + 0.1f);
        float randY = UnityEngine.Random.Range(SpawnPoint.y - 0.1f, SpawnPoint.y + 0.1f);

        billions.GetComponent<BillionController>().lvl = lvl;
        billions.GetComponent<BillionController>().HandleLVL(lvl);

        Instantiate(billions, new Vector3(randX,randY,-0.01f), Quaternion.identity);


    }

    void FaceClosestEnemy()
    {
        if (turrentGun == null || shotPoint == null) return;
        if (!enemyDict.ContainsKey(turrent.tag)) return;

        List<string> enemyTags = enemyDict[turrent.tag];
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
            Vector3 targetDir = (closestEnemy.transform.position - transform.position).normalized;
            Vector3 currentDir = (turrentGun.position - transform.position).normalized;
            float angleDiff = Vector3.SignedAngle(currentDir, targetDir, Vector3.forward);
            float step = Mathf.Clamp(angleDiff, -maxRotationSpeed * Time.deltaTime, maxRotationSpeed * Time.deltaTime);
            turrentGun.RotateAround(transform.position, Vector3.forward, step);
        }
    }
    void TryFireAtClosestEnemy()
    {
        if (!enemyDict.ContainsKey(turrent.tag)) return;

        List<string> enemyTags = enemyDict[turrent.tag];
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
        switch (turrent.tag)
        {
            case "TRed": laserPrefab = redLaserPrefab; break;
            case "TBlue": laserPrefab = blueLaserPrefab; break;
            case "TGreen": laserPrefab = greenLaserPrefab; break;
            case "TYellow": laserPrefab = yellowLaserPrefab; break;
        }

        if (laserPrefab == null || shotPoint == null) return;

        Vector3 direction = (targetPos - shotPoint.position).normalized;
        GameObject laser = Instantiate(laserPrefab, shotPoint.position, Quaternion.identity);
        laser.GetComponent<Rigidbody2D>().linearVelocity = direction * laserSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laser.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        String oTag = null;
        switch (turrent.tag)
        {
            case "TRed": oTag = "MRed"; break;
            case "TBlue": oTag = "MBlue"; break;
            case "TGreen": oTag = "MGreen"; break;
            case "TYellow": oTag = "MYellow"; break;
        }

        String bTag = null;
        switch (turrent.tag)
        {
            case "TRed": bTag = "BR"; break;
            case "TBlue": bTag = "BB"; break;
            case "TGreen": bTag = "BG"; break;
            case "TYellow": bTag = "BY"; break;
        }

        int dmg = 1919810;
        laser.GetComponent<Laser>().ownerTag = oTag;
        laser.GetComponent<Laser>().baseTag = bTag;
        laser.GetComponent<Laser>().damage = dmg;
    }

    void HandleHealth()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        Debug.Log(gameObject.tag + " Current Health left: " + health);
    }

    public void minusHealth(int number)
    {
        health -= number;
        HandleHealth();
        lifeRing.SetHealth(health);
    }

    public void Upgrade()
    {
        if (lvl < 9){
            lvl++;
            Transform lvlText = transform.Find("lvlText");
            if (lvlText == null)
            {
                Debug.LogWarning("lvlText not found");
                return;
            }
            for (int i = 0; i <= 9; i++)
            {
                Transform child = lvlText.Find(i.ToString());
                if (child != null)
                {
                    child.gameObject.SetActive(i == lvl);
                }
            }
        } else 
        {
            return;
        }
    }

    
}
