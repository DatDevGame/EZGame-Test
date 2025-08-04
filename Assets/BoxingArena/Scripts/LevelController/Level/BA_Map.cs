using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BA_Map : MonoBehaviour
{
    public List<Transform> PointSpawn => m_PointSpawn;

    [SerializeField, BoxGroup("References")]
    private List<Transform> m_PointSpawn = new List<Transform>();

    [SerializeField, BoxGroup("References")]
    private Transform m_PointHolder;

    [SerializeField, BoxGroup("Grid Settings")]
    private int m_GridX = 3, m_GridY = 3;

    [SerializeField, BoxGroup("Grid Settings")]
    private float m_OffsetX = 1f, m_OffsetY = 1f;

#if UNITY_EDITOR
    [Button("Generate Grid")]
    private void GenerateGrid()
    {
        ClearPoints();

        float startX = -((m_GridX - 1) * m_OffsetX) / 2f;
        float startY = -((m_GridY - 1) * m_OffsetY) / 2f;

        for (int y = 0; y < m_GridY; y++)
        {
            for (int x = 0; x < m_GridX; x++)
            {
                GameObject point = new GameObject($"Point_{x}_{y}");
                point.transform.SetParent(m_PointHolder);

                Vector3 localPos = new Vector3(startX + x * m_OffsetX, 0f, startY + y * m_OffsetY);
                point.transform.localPosition = localPos;

                m_PointSpawn.Add(point.transform);
            }
        }
    }


    [Button("Clear Points")]
    private void ClearPoints()
    {
        foreach (Transform t in m_PointSpawn)
        {
            if (t != null)
                DestroyImmediate(t.gameObject);
        }
        m_PointSpawn.Clear();
    }
#endif
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (m_PointSpawn == null || m_PointSpawn.Count == 0)
            return;

        Gizmos.color = Color.red;
        foreach (Transform point in m_PointSpawn)
        {
            if (point != null)
            {
                Gizmos.DrawSphere(point.position, 0.2f);
#endif
            }
        }
    }

}
