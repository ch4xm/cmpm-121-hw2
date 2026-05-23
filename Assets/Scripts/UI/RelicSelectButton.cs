using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class RelicSelectButton : MonoBehaviour
{
    public GameObject icon;
    public GameObject name;
    public GameObject description;

    public Relic rewardRelic;

    public void SetRelic(Relic data)
    {
        rewardRelic = data;
        SetIcon(data.sprite);
        name.GetComponent<TextMeshProUGUI>().text = data.name.FirstCharacterToUpper();
        description.GetComponent<TextMeshProUGUI>().text = data.trigger.description + ", " + data.effect.description;
    }

    public void SetIcon(int sprite)
    {
        GameManager.Instance.relicIconManager.PlaceSprite(sprite, icon.GetComponent<Image>());
    }

    public void SelectRelic()
    {
        EventBus.Instance.RelicSelected(rewardRelic);
    }
}