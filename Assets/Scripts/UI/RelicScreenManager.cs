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

    public GameObject RelicButtons;

    public RelicUI[] relicSlots;

    private List<Relic> choices;

    void Start() {
        RelicScreen.SetActive(false);

        choices = new List<Relic>();

        EventBus.Instance.OnHideRewardScreen += CheckRelicCondition; // When user goes to next wave, check if relic condition (every 3 levels)
    }

    public void CheckRelicCondition()
    {
        if (GameManager.Instance.currentWave % 2 == 1)
        {
            ShowRelicScreen();
            return;
        }

        EventBus.Instance.DoNextWave();
    }
    public void ShowRelicScreen()
    {
        var player = GameManager.Instance.player.GetComponent<PlayerController>();
        var relics = player.relicTypes.Values.ToList<Relic>();

        var relicButtons = RelicButtons.GetComponentsInChildren<Button>();

        for (int i = 0; i < 3; i++)
        {
            if (relics[i] is not null)
            {
                choices.Add(relics[i]);
                relicSlots[i].SetRelic(relics[i]);

                var name = relicButtons[i].GetComponentsInChildren<TextMeshProUGUI>()[0];
                var description = relicButtons[i].GetComponentsInChildren<TextMeshProUGUI>()[1];

                name.text = relics[i].name;
                description.text = relics[i].trigger.description + relics[i].effect.description;
            }
        }

        RelicScreen.SetActive(true);


    }

    public void PickupRelic(int index)
    {
        //EventBus.Instance.PickupRelic(choices[index]);
        choices[index].Activate();
        
        HideRelicScreen();
    }

    public void HideRelicScreen()
    {
        RelicScreen.SetActive(false);
        EventBus.Instance.DoNextWave();
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