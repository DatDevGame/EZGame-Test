using System;
using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using HightLightDebug;
using Premium.GameManagement;
using UnityEngine;

public class BAGameManager : Singleton<BAGameManager>
{
    private const string k_DebugTag = nameof(GameManager);
        private const string k_GameManagerName = "BAGameManager";

        private static event Action s_OnSpawnCompleted = delegate { };
        public static event Action onSpawnCompleted
        {
            add
            {
                if (isSpawned)
                    value?.Invoke();
                s_OnSpawnCompleted += value;
            }
            remove
            {
                s_OnSpawnCompleted -= value;
            }
        }
        public static bool isSpawned => Instance != null;

        private static void NotifyEventSpawnCompleted()
        {
            s_OnSpawnCompleted.Invoke();
            if (Instance.m_Verbose)
                DebugPro.AquaBold("GameManager is spawned completely");
        }

        /// <summary>
        /// Auto spawn GameManager before scene is loaded
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnRuntimeInitializeLoad()
        {
            Instantiate(Resources.Load(k_GameManagerName)).name = k_GameManagerName;
            NotifyEventSpawnCompleted();
        }

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = Premium.GameManagement.GlobalGameConfigSO.Instance.targetFPS;
            QualitySettings.vSyncCount = 0;
            if (Instance.m_Verbose)
                DebugPro.AquaBold($"Set target frame rate: {Premium.GameManagement.GlobalGameConfigSO.Instance.targetFPS}");
        }

        private void OnApplicationFocus(bool focus)
        {
            GameEventHandler.Invoke(ApplicationLifecycleEventCode.OnApplicationFocus, focus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            GameEventHandler.Invoke(ApplicationLifecycleEventCode.OnApplicationPause, pauseStatus);
        }

        private void OnApplicationQuit()
        {
            GameEventHandler.Invoke(ApplicationLifecycleEventCode.OnApplicationQuit);
        }
}
