using System;
using UnityEngine;

public class RelicRewardScreenManager : MonoBehaviour
{
    public GameObject RelicScreen;

    public GameObject RelicChoices;

    void Start() {

    }

    void Update()
    {
        var relicUIs = RelicChoices.GetComponentsInChildren<RelicUI>(true);

        for (int i = 0; i < relicUIs.Length; i++)
        {
            relicUIs[i].SetRelic(i);
        }
    }
}