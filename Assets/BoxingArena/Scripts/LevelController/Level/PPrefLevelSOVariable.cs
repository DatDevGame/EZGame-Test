using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PPrefLevelDataSO", menuName = "BoxingArena/Level/PPrefLevelDataSO")]
public class PPrefLevelSOVariable : LevelSOVariable
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.PropertyOrder(-1)]
#endif
    [SerializeField]
    protected string m_Key;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.PropertyOrder(-1)]
#endif
    [SerializeField]
    protected LevelListSOVariable m_LevelSOlList;

    public virtual bool hasKey => PlayerPrefs.HasKey(key);
    public virtual string key => m_Key;
    public override LevelDataSO value
    {
        get
        {
            if (m_RuntimeValue == null)
            {
                if (m_LevelSOlList == null || m_LevelSOlList.value == null)
                    return m_InitialValue;
                var itemGuid = PlayerPrefs.GetString(m_Key, m_InitialValue?.guid ?? string.Empty);
                foreach (var item in m_LevelSOlList.value)
                {
                    if (item.guid == itemGuid)
                    {
                        m_RuntimeValue = item;
                        break;
                    }
                }
            }
            return m_RuntimeValue ?? m_InitialValue;
        }
        set
        {
            PlayerPrefs.SetString(m_Key, value == null ? string.Empty : value.guid);
            base.value = value;
        }
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        GenerateSaveKey();
    }
    protected virtual void GenerateSaveKey()
    {
        if (string.IsNullOrEmpty(m_Key) && !string.IsNullOrEmpty(name))
        {
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
            m_Key = $"{name}_{guid}";
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
    public virtual void Clear()
    {
        ResetValue();
        PlayerPrefs.DeleteKey(m_Key);
    }
    public override void OnAfterDeserialize()
    {
        // Do nothing
    }
    public override void OnBeforeSerialize()
    {
        // Do nothing
    }
}
