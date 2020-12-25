using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] private StatDisplay[] _statDisplays;
    [SerializeField] private string[] _statNames;

    private Stat[] _stats;

    private void OnValidate()
    {
        _statDisplays = GetComponentsInChildren<StatDisplay>();
        UpdateStatNames();
    }

    public void SetStats(params Stat[] playerStats)
    {
        _stats = playerStats;

        if(_stats.Length > _statDisplays.Length)
        {
            Debug.LogError("Not enough stat displays!", this);
            return;
        }
        for(int i = 0; i < _statDisplays.Length; i++)
        {
            _statDisplays[i].gameObject.SetActive(i < _stats.Length);

            if(i < _stats.Length)
            {
                _statDisplays[i].Stat = _stats[i];
            }
        }
    }
    public void UpdateStatValues()
    {
        for(int i = 0; i < _stats.Length; i++)
        {
            _statDisplays[i].UpdateStatValue();
        }
    }
    public void UpdateStatNames()
    {
        for(int i = 0; i < _statNames.Length; i++)
        {
            _statDisplays[i].Name = _statNames[i];
        }
    }
}
