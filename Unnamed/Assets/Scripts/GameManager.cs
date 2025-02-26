using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject blueFlagPrefab;
    [SerializeField] GameObject redFlagPrefab;
    [SerializeField] GameObject greenFlagPrefab;
    [SerializeField] GameObject yellowFlagPrefab;
    [SerializeField] Material LineMaterial;

    private Dictionary<string, Queue<GameObject>> flags = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<KeyCode, (string tag, GameObject prefab)> flagMaps;

    private int maxFlagsPerColor = 2;

    private GameObject dragging;
    private LineRenderer line;
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        line = gameObject.AddComponent<LineRenderer>();
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.positionCount = 2;
        line.enabled = false;
        line.material = LineMaterial;

        flagMaps = new Dictionary<KeyCode, (string, GameObject)>()
        {
            { KeyCode.Alpha1, ("Blue", blueFlagPrefab) },
            { KeyCode.Alpha2, ("Red", redFlagPrefab) },
            { KeyCode.Alpha3, ("Green", greenFlagPrefab) },
            { KeyCode.Alpha4, ("Yellow", yellowFlagPrefab) }
        };

        foreach (var entry in flagMaps)
        {
            flags[entry.Value.tag] = new Queue<GameObject>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                dragging = hit.collider.gameObject;
                line.enabled = true;
            }
            else
            {
                HandleFlagPlacement(mousePos);
            }
        }

        HandleFlagDragging(mousePos);
    }
    void HandleFlagPlacement(Vector2 position)
    {
        foreach (var entry in flagMaps)
        {
            if (Input.GetKey(entry.Key))
            {
                SpawnFlag(entry.Value.tag, entry.Value.prefab, position);
            }
        }
    }
    void SpawnFlag(string FlagTag, GameObject prefab, Vector2 position)
    {
        Queue<GameObject> flagQueue = flags[FlagTag];

        GameObject closestFlag = FindClosestFlag(position, FlagTag);

        if (flagQueue.Count >= maxFlagsPerColor)
        {
            if (closestFlag != null)
            {
                closestFlag.transform.position = position;
                return;
            }
        }
        else
        {
            GameObject newFlag = Instantiate(prefab, position, Quaternion.identity);
            newFlag.tag = tag;
            flagQueue.Enqueue(newFlag);
        }
    }
    GameObject FindClosestFlag(Vector2 position, string tag)
    {
        GameObject closestFlag = null;
        float Closest = Mathf.Infinity;

        foreach (GameObject flag in flags[tag])
        {
            if (flag != null)
            {
                float distance = Vector2.Distance(flag.transform.position, position);
                if (distance < Closest)
                {
                    Closest = distance;
                    closestFlag = flag;
                }
            }
        }
        return closestFlag;
    }

    void HandleFlagDragging(Vector2 mousePos)
    {
        if (dragging != null && Input.GetMouseButton(0))
        {
            line.SetPosition(0, dragging.transform.position);
            line.SetPosition(1, mousePos);
        }

        if (Input.GetMouseButtonUp(0) && dragging != null)
        {
            dragging.transform.position = mousePos;
            dragging = null;
            line.enabled = false;
        }
    }
}
