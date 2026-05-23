using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicRewardScreenManager : MonoBehaviour
{
    public GameObject RelicScreen;

    public GameObject RelicsContainer;

    public RelicSelectButton[] relicButtons;

    void Start() {
        RelicScreen.SetActive(false);

        EventBus.Instance.OnHideRewardScreen += CheckRelicCondition; // When user goes to next wave, check if relic condition (every 3 levels)
        EventBus.Instance.OnRelicSelected += _ => HideRelicScreen();
    }

    public void CheckRelicCondition()
    {
        if (GameManager.Instance.currentWave % 1 == 0)
        {
            ShowRelicScreen();
            return;
        }

        EventBus.Instance.DoNextWave();
    }
    public void ShowRelicScreen()
    {
        var relics = GameManager.Instance.relicTypes.Values.ToList<Relic>();

        for (int i = 0; i < relicButtons.Length; i++)
        {
            bool active = i < relics.Count;

            relicButtons[i].gameObject.SetActive(active);

            if (active)
            {
                relicButtons[i].SetRelic(relics[i]);
            }
        }

        RelicScreen.SetActive(true);
    }

    public void HideRelicScreen()
    {
        RelicScreen.SetActive(false);

        EventBus.Instance.DoNextWave();
    }
}