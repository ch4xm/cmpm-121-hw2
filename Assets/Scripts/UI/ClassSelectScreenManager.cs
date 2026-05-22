using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlayerClass = PlayerController.PlayerClass;

public class ClassSelectScreenManager : MonoBehaviour
{
    public GameObject classSelectUI;

    public GameObject classButtonContainer;

    public ClassSelectButton[] classButtons;

    private string selectedLevel;

    void Start() {
        classSelectUI.SetActive(false);

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
        var classes = GameManager.Instance.playerClasses.Values.ToList<PlayerClass>();

        for (int i = 0; i < classButtons.Length; i++)
        {
            if (classes[i] is not null)
            {
                classButtons[i].SetButtonDetails(classes[i]);
            }
        }

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