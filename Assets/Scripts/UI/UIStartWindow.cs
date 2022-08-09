using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStartWindow : MonoBehaviour
{
    [SerializeField] private UIManager2 _ui;
    [SerializeField] private TMP_Dropdown _dropDown;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private GameObject[] _nameObjects;
    [SerializeField] private TMP_InputField[] _nameFields;
    [SerializeField] private Button _startGame;
    private int _dropdownChoice;
    private bool isEmpty = true;

    void Awake()
    {
        _ui.DisableInteractions(false);
        _startGame.interactable = false;
    }

    private void Update()
    {
        for (int i = 0; i < _nameFields.Length; i++)
        {
            if (_nameFields[i].text == "" && _nameFields[i].gameObject.activeInHierarchy)
            {
                isEmpty = true;
                break;
            }
            isEmpty = false;
        }
        if (isEmpty) _startGame.interactable = false;
        else _startGame.interactable = true;
    }

    public void OnDropDownChanged()
    {
        _dropdownChoice = Int32.Parse(_dropDown.options[_dropDown.value].text);
        for (int i = 0; i < _nameObjects.Length; i++) _nameObjects[i].SetActive(false);
        for (int i = 0; i < _dropdownChoice - 1; i++) _nameObjects[i].SetActive(true);
    }

    public void PlayersSet()
    {
         string[] names = new string[_nameFields.Length];
         for (int i = 0; i < _nameFields.Length; i++) names[i] = _nameFields[i].text;
        _dropdownChoice = Int32.Parse(_dropDown.options[_dropDown.value].text);
        _playerManager.SetPlayers(_dropdownChoice,names);
    }

}
