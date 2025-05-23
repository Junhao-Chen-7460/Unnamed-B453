using UnityEngine;

public class ExpRing : MonoBehaviour
{
    [SerializeField] private int segments = 100;
    [SerializeField] private float radius = 1.2f;
    [SerializeField] private float lineWidth = 0.05f;
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    public void UpdateExpRing(int curExp, int maxExp)
    {
        float ratio = (maxExp <= 0) ? 0f : Mathf.Clamp01((float)curExp / maxExp);
        DrawRing(ratio);
    }

    private void DrawRing(float fillRatio)
    {
        float angleStep = 360f / segments;
        int filledSegments = Mathf.RoundToInt(segments * fillRatio);

        for (int i = 0; i <= segments; i++)
        {
            if (i > filledSegments)
            {
                lineRenderer.SetPosition(i, Vector3.zero);
                continue;
            }

            float angle = Mathf.Deg2Rad * (i * angleStep);
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}
