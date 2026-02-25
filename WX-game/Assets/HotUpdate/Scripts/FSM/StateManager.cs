using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class StateManager : SerializedMonoBehaviour
{
    private static StateManager _instance;
    public static StateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<StateManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    [OdinSerialize]
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.CollapsedFoldout)]
    public Dictionary<EnemyType,Dictionary<StateType, State>> enemyStates = new Dictionary<EnemyType, Dictionary<StateType, State>>();

    public void AddState(EnemyType enemyType, StateType stateType, State state)
    {
        if (!enemyStates.ContainsKey(enemyType))
        {
            enemyStates[enemyType] = new Dictionary<StateType, State>();
        }
        enemyStates[enemyType][stateType] = state;
    }

    public bool HasState(EnemyType enemyType, StateType stateType)
    {
        return enemyStates.ContainsKey(enemyType) && enemyStates[enemyType].ContainsKey(stateType);
    }
    
    public void TryGetState(EnemyType enemyType, StateType stateType, out State state)
    {
        state = null;
        if (enemyStates.ContainsKey(enemyType) && enemyStates[enemyType].ContainsKey(stateType))
        {
            state = enemyStates[enemyType][stateType];
        }
    }
}
