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
        if (GameManager.Instance.currentWave % 3 == 0)
        {
            ShowRelicScreen();
            return;
        }

        EventBus.Instance.DoNextWave();
    }
    public void ShowRelicScreen()
    {
        PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();

        // Generate randomly ordered relics and get first 3 of them
        var lockedRelics = player.GetInactiveRelics();
        var relicChoices = lockedRelics.OrderBy(_ => UnityEngine.Random.value).Take(3).ToList();

        for (int i = 0; i < relicButtons.Length; i++)
        {
            bool active = i < relicChoices.Count;

            relicButtons[i].gameObject.SetActive(active);

            if (active)
            {
                relicButtons[i].SetRelic(relicChoices[i]);
            }
        }

        RelicScreen.SetActive(true);
    }

    public void HideRelicScreen()
    {
        RelicScreen.SetActive(false);

        EventBus.Instance.DoNextWave();
    }
    void OnDestroy()
    {
        EventBus.Instance.OnHideRewardScreen -= CheckRelicCondition;
        EventBus.Instance.OnRelicSelected -= _ => HideRelicScreen();
    }
}