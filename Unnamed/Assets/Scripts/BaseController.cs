using UnityEngine;

public class BaseController : MonoBehaviour
{
    [SerializeField] GameObject billions;
    [SerializeField] float SpawnTime;

    private float timer;
    private Vector3 SpawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        getSpawnPoint();
    }

    // Update is called once per frame
    void Update()
    {
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
        float randX = Random.Range(SpawnPoint.x - 0.1f, SpawnPoint.x + 0.1f);
        float randY = Random.Range(SpawnPoint.y - 0.1f, SpawnPoint.y + 0.1f);
        Instantiate(billions, new Vector3(randX,randY,-0.01f), Quaternion.identity);
    }
}
