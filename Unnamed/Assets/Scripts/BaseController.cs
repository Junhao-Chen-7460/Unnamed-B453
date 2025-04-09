using UnityEngine;
using System.Collections.Generic;
using System;

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

    private float fireTimer;
    private float timer;
    private Vector3 SpawnPoint;

    private Rigidbody2D rb;
    private Transform turretGun;
    private Transform shotPoint;

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
            turretGun = turrent.transform.Find("TurrentGun");
            shotPoint = turretGun.Find("shotPoint");
        }
    }

    // Update is called once per frame
    void Update()
    {
        FaceClosestEnemy();
        fireTimer += Time.deltaTime;
        TryFireAtClosestEnemy();
        
        timer += Time.deltaTime;
        if (timer >= SpawnTime)
        {
            timer = 0f;
            SpawnBillion();
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
        Instantiate(billions, new Vector3(randX,randY,-0.01f), Quaternion.identity);
    }

    void FaceClosestEnemy()
    {
        if (turretGun == null || shotPoint == null) return;
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
            Vector2 direction = (closestEnemy.transform.position - turretGun.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turretGun.rotation = Quaternion.RotateTowards(turretGun.rotation, Quaternion.Euler(0, 0, angle), 180 * Time.deltaTime);
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
        laser.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        
        String oTag = null;
        switch (turrent.tag)
        {
            case "TRed": oTag = "MRed"; break;
            case "TBlue": oTag = "MBlue"; break;
            case "TGreen": oTag = "MGreen"; break;
            case "TYellow": oTag = "MYellow"; break;
        }
        laser.GetComponent<Laser>().ownerTag = oTag;
    }
}
