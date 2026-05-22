using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassSelectScreenManager : MonoBehaviour
{
    public GameObject classSelectUI;

    public GameObject classButtons;

    private string selectedLevel;

    void Start() {

        EventBus.Instance.OnLevelSelected += HandleLevelSelected;
        EventBus.Instance.OnClassSelected += HandleClassSelected;
        //EventBus.Instance.OnHideRewardScreen += CheckRelicCondition; // When user goes to next wave, check if relic condition (every 3 levels)
    }

    public void HandleLevelSelected(string levelName)
    {
        selectedLevel = levelName;

        ShowClassSelection();
    }

    public void ShowClassSelection()
    {
        classSelectUI.SetActive(true);
    }

    public void HandleClassSelected(string playerClass)
    {
        classSelectUI.SetActive(false);

        EventBus.Instance.StartGame(selectedLevel, playerClass);   
    }

    public void SelectClass(string className)
    {
        EventBus.Instance.StartGame(selectedLevel, className);
    }

    void Update()
    {
        //var relicUIs = RelicButtons.GetComponentsInChildren<RelicUI>(true);

        //for (int i = 0; i < relicUIs.Length; i++)
        //{
        //    relicUIs[i].SetRelic(i);
        //}
    }
}