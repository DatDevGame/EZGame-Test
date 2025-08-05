using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HCore.Events;
using HCore.Helpers;
using Premium;
using Premium.EditableStateMachine;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class BALevelController : MonoBehaviour
{
    public List<BaseBoxer> AllAliveBoxers => m_AllAliveBoxers;
    public List<BaseBoxer> PlayerTeams => m_PlayerTeams;

    [SerializeField, BoxGroup("References")] private LayerMask m_PlayerTeamLayerMask;
    [SerializeField, BoxGroup("References")] private LayerMask m_OpponentTeamLayerMask;
    [SerializeField, BoxGroup("References")] private RectTransform m_CanvasHolder;
    [SerializeField, BoxGroup("References")] private DamageFeedBackUI m_DamageFeedBackUI;
    [SerializeField, BoxGroup("Resource")] private PlayerBoxer m_PlayerBoxerPrefab;
    [SerializeField, BoxGroup("Data")] protected HealthBarSO m_HealthBarSO;
    [SerializeField, BoxGroup("Data")] private StatsSO m_PlayerStatSO;
    [SerializeField, BoxGroup("Data")] private PlayModeEnum m_GameMode;
    [SerializeField, BoxGroup("Data")] private LevelManagerSO m_LevelManagerSO;

    private LevelDataSO m_CurrentLevelSO;
    private BA_Map m_CurrentBAMap;
    private PlayerBoxer m_PlayerBoxer;
    private List<BaseBoxer> m_AllAliveBoxers;
    private List<BaseBoxer> m_PlayerTeams;
    private List<BaseBoxer> m_OpponentTeams;

    private void Awake()
    {
        Init();
        GameEventHandler.AddActionEvent(PVPEventCode.CharacterReceivedDamage, OnCharacterReceivedDamage);
        GameEventHandler.AddActionEvent(PVPEventCode.AnyCharacterDead, OnAnyCharacterDead);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PVPEventCode.CharacterReceivedDamage, OnCharacterReceivedDamage);
        GameEventHandler.RemoveActionEvent(PVPEventCode.AnyCharacterDead, OnAnyCharacterDead);
    }

    private void Init()
    {
        OnLoadLevel();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
            m_OpponentTeams.ForEach(v => v.DeadEditor());
        if (Input.GetKeyDown(KeyCode.L))
        {
            m_PlayerBoxer.DeadEditor();
            m_PlayerTeams.ForEach(v => v.DeadEditor());
        }
#endif
    }

    private void OnLoadLevel()
    {
        m_AllAliveBoxers = new List<BaseBoxer>();
        m_PlayerTeams = new List<BaseBoxer>();
        m_OpponentTeams = new List<BaseBoxer>();

        m_CurrentLevelSO = m_LevelManagerSO.GetCurrentLevelDataSO(m_GameMode.value);
        m_CurrentBAMap = Instantiate(m_CurrentLevelSO.MapPrefab, transform);
        List<Transform> spawnPoints = new List<Transform>(m_CurrentBAMap.GetSpawnFollowingMode(m_GameMode.value));

        //Spawn Player
        Transform playerSpawnPoint = spawnPoints[0];
        m_PlayerBoxer = Instantiate(m_PlayerBoxerPrefab);
        m_PlayerBoxer.SetLocal(true);
        m_PlayerBoxer.SetHealthBarMaterials(m_HealthBarSO.PlayerHealthBarMaterial);
        m_PlayerBoxer.transform.position = playerSpawnPoint.position;
        m_PlayerBoxer.transform.eulerAngles = playerSpawnPoint.transform.eulerAngles;
        m_PlayerBoxer.gameObject.layer = GetLayerFromMask(m_PlayerTeamLayerMask);
        if (!m_AllAliveBoxers.Contains(m_PlayerBoxer))
            m_AllAliveBoxers.Add(m_PlayerBoxer);
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
                OnLoadMannyVsMany();
                break;

            default:
                Debug.LogWarning("Unsupported GameMode");
                break;
        }

        m_CurrentBAMap.PreviewCharacterHandle.OnCompletedPreview += OnStartLevel;
        m_CurrentBAMap.PreviewCharacterHandle.PlayPreview();
    }
    private void OnStartLevel()
    {
        m_PlayerBoxer.Init();
        m_PlayerTeams.ForEach(v => v.Init(m_CurrentLevelSO.GetRandomStatsByProbability()));
        m_OpponentTeams.ForEach(v => v.Init(m_CurrentLevelSO.GetRandomStatsByProbability()));
        GameEventHandler.Invoke(PVPEventCode.OnLevelStart, m_CurrentLevelSO, m_PlayerBoxer);
        m_CurrentBAMap.PreviewCharacterHandle.OnCompletedPreview -= OnStartLevel;
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

    private void OnLoadMannyVsMany()
    {
        List<Transform> playerSpawnPoint = new List<Transform>(m_CurrentBAMap.GetPlayerTeamSpawnPoint());
        List<Transform> opponentSpawnPoint = new List<Transform>(m_CurrentBAMap.GetOpponentTeamSpawnPoint());

        //Player Team
        SpawnCharacter(m_CurrentLevelSO.PlayerTeamCount, m_PlayerTeamLayerMask, playerSpawnPoint);

        //Opponent Team
        SpawnCharacter(m_CurrentLevelSO.EnemyTeamCount, m_OpponentTeamLayerMask, opponentSpawnPoint);
    }

    private void SpawnCharacter(int count, LayerMask layerMask, List<Transform> spawnPoints)
    {
        for (int i = 0; i < count; i++)
        {
            Transform pointSpawn = spawnPoints.GetRandom();
            CharacterSO characterSO = m_LevelManagerSO.CharacterManagerSO.initialValue.GetRandom() as CharacterSO;
            BaseBoxer baseBoxer = Instantiate(characterSO.BaseBoxerPrefab);
            baseBoxer.SetLocal(false);
            baseBoxer.transform.position = pointSpawn.position;
            baseBoxer.transform.eulerAngles = pointSpawn.eulerAngles;
            baseBoxer.gameObject.layer = GetLayerFromMask(layerMask);
            spawnPoints.Remove(pointSpawn);

            bool isOpponent = baseBoxer.gameObject.layer != GetLayerFromMask(m_PlayerTeamLayerMask);
            baseBoxer.SetHealthBarMaterials(isOpponent ? m_HealthBarSO.OpponentHealthBarMaterial : m_HealthBarSO.PlayerTeamHealthBarMaterial);
            if (isOpponent)
                m_OpponentTeams.Add(baseBoxer);
            else
                m_PlayerTeams.Add(baseBoxer);

            if (!m_AllAliveBoxers.Contains(baseBoxer))
                m_AllAliveBoxers.Add(baseBoxer);
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

    private void OnAnyCharacterDead(object[] parameters)
    {
        if (parameters == null || parameters.Length <= 0)
            return;
        BaseBoxer baseBoxer = (BaseBoxer)parameters[0];
        if (m_AllAliveBoxers.Contains(baseBoxer))
            m_AllAliveBoxers.Remove(baseBoxer);

        //Handle End Game Following Mode
        switch (m_GameMode.value)
        {
            case GameMode.OneVsOne:
                OnEndLevelDuelMode();
                break;
            case GameMode.OneVsMany:
                OnEndLevelSurvivorMode();
                break;
            case GameMode.ManyVsMany:
                OnEndLevelTeamBattle();
                break;
        }
    }

    private void OnEndLevelDuelMode()
    {
        if (m_AllAliveBoxers.Count <= 1)
        {
            var boxer = m_AllAliveBoxers.Find(v => v.IsLocal);
            bool isVictory = boxer != null;
            if (isVictory)
                m_LevelManagerSO.OneVsOneCurrentLevel.value++;
            GameEventHandler.Invoke(PVPEventCode.OnLevelEnd, m_CurrentLevelSO, isVictory);
        }
    }

    private void OnEndLevelSurvivorMode()
    {
        if (!m_PlayerBoxer.IsAlive)
        {
            GameEventHandler.Invoke(PVPEventCode.OnLevelEnd, m_CurrentLevelSO, false);
            return;
        }
        if (!m_OpponentTeams.Any(v => v.IsAlive))
        {
            m_LevelManagerSO.OneVsManyCurrentLevel.value++;
            GameEventHandler.Invoke(PVPEventCode.OnLevelEnd, m_CurrentLevelSO, true);
        }
    }

    private void OnEndLevelTeamBattle()
    {
        if (!m_PlayerTeams.Any(v => v.IsAlive) && !m_PlayerBoxer.IsAlive)
            GameEventHandler.Invoke(PVPEventCode.OnLevelEnd, m_CurrentLevelSO, false);

        if (!m_OpponentTeams.Any(v => v.IsAlive))
        {
            m_LevelManagerSO.ManyVsManyCurrentLevel.value++;
            GameEventHandler.Invoke(PVPEventCode.OnLevelEnd, m_CurrentLevelSO, true);
        }
    }
}
