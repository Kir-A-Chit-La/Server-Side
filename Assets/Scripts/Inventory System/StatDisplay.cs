using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    private Stat _stat;
    public Stat Stat 
    {
        get => _stat; 
        set
        {
            _stat = value;
            UpdateStatValue();
        }
    }
    private string _name;
    public string Name 
    {
        get => _name;
        set
        {
            _name = value;
            _nameText.text = _name;
        }
    }
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _valueText;

    private void OnValidate()
    {
        TMP_Text[] text = GetComponentsInChildren<TMP_Text>();
        _nameText = text[0];
        _valueText = text[1];
    }
    public void UpdateStatValue()
    {
        _valueText.text = _stat.Value.ToString();
    }
}
