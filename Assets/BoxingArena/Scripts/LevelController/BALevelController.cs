using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using HCore.Helpers;
using Premium;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class BALevelController : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private LayerMask m_PlayerTeamLayerMask;
    [SerializeField, BoxGroup("References")] private LayerMask m_OpponentTeamLayerMask;
    [SerializeField, BoxGroup("References")] private RectTransform m_CanvasHolder;
    [SerializeField, BoxGroup("References")] private DamageFeedBackUI m_DamageFeedBackUI;
    [SerializeField, BoxGroup("Resource")] private PlayerBoxer m_PlayerBoxerPrefab;
    [SerializeField, BoxGroup("Data")] private PlayModeEnum m_GameMode;
    [SerializeField, BoxGroup("Data")] private LevelManagerSO m_LevelManagerSO;

    private LevelDataSO m_CurrentLevelSO;
    private BA_Map m_CurrentBAMap;
    private PlayerBoxer m_PlayerBoxer;
    private List<BaseBoxer> m_PlayerTeams;
    private List<BaseBoxer> m_OpponentTeams;

    private void Awake()
    {
        Init();
        GameEventHandler.AddActionEvent(PVPEventCode.CharacterReceivedDamage, OnCharacterReceivedDamage);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PVPEventCode.CharacterReceivedDamage, OnCharacterReceivedDamage);
    }

    private void Init()
    {
        OnLoadLevel();
    }

    private void OnLoadLevel()
    {
        m_PlayerTeams = new List<BaseBoxer>();
        m_OpponentTeams = new List<BaseBoxer>();

        m_CurrentLevelSO = m_LevelManagerSO.GetCurrentLevelDataSO();
        m_CurrentBAMap = Instantiate(m_CurrentLevelSO.MapPrefab, transform);
        List<Transform> spawnPoints = new List<Transform>(m_CurrentBAMap.PointSpawn);

        //Spawn Player
        Transform playerSpawnPoint = spawnPoints.GetRandom();
        m_PlayerBoxer = Instantiate(m_PlayerBoxerPrefab);
        m_PlayerBoxer.transform.position = playerSpawnPoint.position;
        m_PlayerBoxer.gameObject.layer = GetLayerFromMask(m_PlayerTeamLayerMask);
        spawnPoints.Remove(playerSpawnPoint);

        //Spawn Following Mode
        switch (m_GameMode.value)
        {
            case GameMode.OneVsOne:
                OnLoadOneVsOne(spawnPoints);
                break;

            case GameMode.OneVsMany:
                OnLoadOneVsMany(spawnPoints);
                break;

            case GameMode.ManyVsMany:
                OnLoadMannyVsMany(spawnPoints);
                break;

            default:
                Debug.LogWarning("Unsupported GameMode");
                break;
        }

        StartCoroutine(CommonCoroutine.Delay(3f, false, () =>
        {
            m_PlayerBoxer.Init();
            m_OpponentTeams.ForEach(v => v.Init());
            GameEventHandler.Invoke(PVPEventCode.OnLevelStart, m_CurrentLevelSO, m_PlayerBoxer);
        }));
    }

    private void OnLoadOneVsOne(List<Transform> spawnPoints)
    {
        SpawnCharacter(1, m_OpponentTeamLayerMask, spawnPoints);
    }

    private void OnLoadOneVsMany(List<Transform> spawnPoints)
    {
        int rollEnemy = m_CurrentLevelSO.RandomEnemy;
        SpawnCharacter(rollEnemy, m_OpponentTeamLayerMask, spawnPoints);
    }

    private void OnLoadMannyVsMany(List<Transform> spawnPoints)
    {

    }

    private void SpawnCharacter(int count, LayerMask layerMask, List<Transform> spawnPoints)
    {
        for (int i = 0; i < count; i++)
        {
            if (spawnPoints.Count <= 0) break;
            Transform pointSpawn = spawnPoints.GetRandom();
            CharacterSO characterSO = m_LevelManagerSO.CharacterManagerSO.initialValue.GetRandom() as CharacterSO;
            BaseBoxer baseBoxer = Instantiate(characterSO.BaseBoxerPrefab);
            baseBoxer.transform.position = pointSpawn.position;
            baseBoxer.gameObject.layer = GetLayerFromMask(layerMask);
            spawnPoints.Remove(pointSpawn);

            if (baseBoxer.gameObject.layer != GetLayerFromMask(m_PlayerTeamLayerMask))
                m_OpponentTeams.Add(baseBoxer);
        }
    }
    private int GetLayerFromMask(LayerMask mask)
    {
        int value = mask.value;
        if (value == 0 || (value & (value - 1)) != 0)
        {
            return 0;
        }
        return (int)Mathf.Log(value, 2);
    }
    private void OnCharacterReceivedDamage(object[] parameters)
    {
        if (parameters == null || parameters.Length <= 0)
            return;
        BaseBoxer baseBoxer = (BaseBoxer)parameters[0];
        int damage = (int)parameters[1];

        DamageFeedBackUI damageFeedBackUI = PoolManager.GetOrCreatePool(m_DamageFeedBackUI, initialCapacity: 1).Get();
        damageFeedBackUI.transform.position = baseBoxer.transform.position + (Vector3.up * 1.5F);
        damageFeedBackUI.Show(damage);
    }
}
