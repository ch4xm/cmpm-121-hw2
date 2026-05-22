using System;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;

    public GameObject ClaimSpellIcon;

    public GameObject ClaimSpellButton;

    public GameObject SpellName;
    public GameObject SpellDescription;

    public GameObject SpellDropper;


    private Spell rewardSpell;

    TextMeshProUGUI title;
    TextMeshProUGUI stats;
    GameObject rewardText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rewardUI.SetActive(false);

        rewardText = new GameObject("rewardText");
        rewardText.transform.SetParent(rewardUI.transform);
        rewardText.transform.localPosition = new Vector3(0, 0);

        
        title = rewardText.AddComponent<TextMeshProUGUI>();
        title.alignment = TextAlignmentOptions.Center;
        title.fontSize = 36;
        title.color = Color.black;

        EventBus.Instance.OnWaveEnd += ShowRewards;
    }

    public void ShowRewards(int currentWave)
    {
        Button button = ClaimSpellButton.GetComponent<Button>();

        button.interactable = true;

        TextMeshProUGUI buttonText =
            ClaimSpellButton.GetComponentInChildren<TextMeshProUGUI>();

        buttonText.text = "Accept Spell";


        var dropperButtons = SpellDropper.GetComponentsInChildren<Button>();
        foreach (var dropperButton in dropperButtons)
        {
            dropperButton.GetComponentInChildren<TextMeshProUGUI>().text = "Drop";
            dropperButton.interactable = true;
            
        }

        var player = GameManager.Instance.player.GetComponent<PlayerController>();

        rewardUI.SetActive(true);

        float waveTime =
            GameManager.Instance.waveEndTime -
            GameManager.Instance.waveStartTime;

        //title.text =
        //    "Wave " + currentWave +
        //    " Defeated!\nTime: " +
        //    Mathf.Round(waveTime) + "s";

        var existingSpells = SpellDropper.GetComponentsInChildren<SpellUI>(true);

        for (int i = 0; i < existingSpells.Length; i++) // Recreate spell UI to allow dropping spells
        {
            if (player.spellcaster.Spells[i] is not null)
            {
                existingSpells[i].SetSpell(player.spellcaster.Spells[i]);
            }
            existingSpells[i].gameObject.SetActive(player.spellcaster.Spells[i] is not null);
        }

        rewardSpell = player.spellcaster.builder.BuildRandomSpell();

        SpellName.GetComponent<TextMeshProUGUI>().text = rewardSpell.GetFullName();

        SpellDescription.GetComponent<TextMeshProUGUI>().text = rewardSpell.GetName() + ": " + rewardSpell.GetDescription() + "\n" + rewardSpell.GetModifiersDescription();

        ClaimSpellIcon.GetComponent<SpellUI>().SetSpell(rewardSpell);
    }

    public void DropSpell(int index)
    {
        var player = GameManager.Instance.player.GetComponent<PlayerController>();

        player.spellcaster.DropSpell(index);

        var dropperButton = SpellDropper.GetComponentsInChildren<Button>()[index];

        dropperButton.GetComponentInChildren<TextMeshProUGUI>().text = "Dropped";
        dropperButton.interactable = false;
    }

    public void ClaimSpell()
    {
        if (rewardSpell is null)
            return;

        var player = GameManager.Instance.player.GetComponent<PlayerController>();

        player.spellcaster.SetFirstAvailableSpell(rewardSpell);


        rewardSpell = null;

        Button claim = ClaimSpellButton.GetComponent<Button>();
        claim.interactable = false;

        TextMeshProUGUI buttonText = ClaimSpellButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = "Accepted";
    }



    public void HideRewards()
    {
        rewardUI.SetActive(false);

        EventBus.Instance.DoHideRewardScreen();

        //GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
    }
}
