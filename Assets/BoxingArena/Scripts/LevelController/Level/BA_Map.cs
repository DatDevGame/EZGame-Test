using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BA_Map : MonoBehaviour
{
    [SerializeField, BoxGroup("References")]
    private List<Transform> m_OneVsOnePointSpawn, m_OneVsManyPointSpawn, m_ManyVsManyPointSpawn, m_PlayerTeamPointSpawn, m_OpponentTeamPointSpawn;

    [SerializeField, BoxGroup("References")]
    private Transform m_OneVsOnePointHolder, m_OneVsManyPointHolder, m_ManyVsManyPointHolder;

    [SerializeField, BoxGroup("Grid Settings")]
    private GameMode m_Mode;
    [SerializeField, BoxGroup("Grid Settings")]
    private int m_GridX = 3, m_GridY = 3;

    [SerializeField, BoxGroup("Grid Settings")]
    private float m_OffsetX = 1f, m_OffsetY = 1f;

    public List<Transform> GetPlayerTeamSpawnPoint() => m_PlayerTeamPointSpawn;
    public List<Transform> GetOpponentTeamSpawnPoint() => m_OpponentTeamPointSpawn;
    public List<Transform> GetSpawnFollowingMode(GameMode gameMode)
    {
        return gameMode switch
        {
            GameMode.OneVsOne => m_OneVsOnePointSpawn,
            GameMode.OneVsMany => m_OneVsManyPointSpawn,
            GameMode.ManyVsMany => m_ManyVsManyPointSpawn,
            _ => m_OneVsOnePointSpawn,
        };
    }
#if UNITY_EDITOR
    [Button("Generate Grid")]
    private void GenerateGrid()
    {
        float startX = -((m_GridX - 1) * m_OffsetX) / 2f;
        float startY = -((m_GridY - 1) * m_OffsetY) / 2f;

        for (int y = 0; y < m_GridY; y++)
        {
            for (int x = 0; x < m_GridX; x++)
            {
                GameObject point = new GameObject($"Point_{x}_{y}");
                switch (m_Mode)
                {
                    case GameMode.OneVsOne:
                        m_OneVsOnePointSpawn.Add(point.transform);
                        point.transform.SetParent(m_OneVsOnePointHolder);
                        break;
                    case GameMode.OneVsMany:
                        m_OneVsManyPointSpawn.Add(point.transform);
                        point.transform.SetParent(m_OneVsManyPointHolder);
                        break;
                    case GameMode.ManyVsMany:
                        m_ManyVsManyPointSpawn.Add(point.transform);
                        point.transform.SetParent(m_ManyVsManyPointHolder);
                        break;
                }
                Vector3 localPos = new Vector3(startX + x * m_OffsetX, 0f, startY + y * m_OffsetY);
                point.transform.localPosition = localPos;
            }
        }
    }


    [Button("Clear Points")]
    private void ClearPoints()
    {
        List<Transform> spawnPoints = m_Mode switch
        {
            GameMode.OneVsOne => m_OneVsOnePointSpawn,
            GameMode.OneVsMany => m_OneVsManyPointSpawn,
            GameMode.ManyVsMany => m_ManyVsManyPointSpawn,
            _ => null
        };
        if (spawnPoints == null) return;
        foreach (Transform t in spawnPoints)
        {
            if (t != null)
                DestroyImmediate(t.gameObject);
        }
        spawnPoints.Clear();
    }

#endif
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (m_OneVsOnePointSpawn != null && m_OneVsOnePointSpawn.Count != 0)
        {
            Gizmos.color = Color.red;
            foreach (Transform point in m_OneVsOnePointSpawn)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.2f);

                }
            }
        }

        if (m_OneVsManyPointSpawn != null && m_OneVsManyPointSpawn.Count != 0)
        {
            foreach (Transform point in m_OneVsManyPointSpawn)
            {
                if (point != null)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(point.position, 0.2f);
                }
            }
        }

        if (m_ManyVsManyPointSpawn != null && m_ManyVsManyPointSpawn.Count != 0)
        {
            foreach (Transform point in m_ManyVsManyPointSpawn)
            {
                if (point != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(point.position, 0.2f);
                }
            }
        }
#endif
    }

}
