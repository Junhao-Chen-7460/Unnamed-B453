using UnityEngine;

public class LifeRing : MonoBehaviour
{
    [SerializeField] int segments = 100;
    [SerializeField] float radius = 1f;
    private LineRenderer lr;
    private int maxHealth = 4;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = false;
        lr.positionCount = segments + 1;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
    }

    public void Init(int maxHealth)
    {
        this.maxHealth = maxHealth;
        SetHealth(maxHealth);
    }

    public void SetHealth(int currentHealth)
    {
        float fillPercent = Mathf.Clamp01((float)currentHealth / maxHealth);
        DrawArc(fillPercent * 360f);
    }

    void DrawArc(float angle)
    {
        float angleStep = angle / segments;
        float start = 90f;
        for (int i = 0; i <= segments; i++)
        {
            float curAngle = Mathf.Deg2Rad * (start - i * angleStep);
            float x = Mathf.Cos(curAngle) * radius;
            float y = Mathf.Sin(curAngle) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
