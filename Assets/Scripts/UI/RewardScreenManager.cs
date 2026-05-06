using System;
using TMPro;
using UnityEngine;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;

    TextMeshProUGUI title;
    TextMeshProUGUI stats;
    GameObject rewardText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rewardText = new GameObject("rewardText");
        rewardText.transform.SetParent(rewardUI.transform);
        rewardText.transform.localPosition = new Vector3(0, 0);

        
        title = rewardText.AddComponent<TextMeshProUGUI>();
        title.alignment = TextAlignmentOptions.Center;
        title.fontSize = 36;
        title.color = Color.black;

        //stats = rewardText.AddComponent<TextMeshProUGUI>();
        //stats.alignment = TextAlignmentOptions.Center;
        //stats.fontSize = 24;
        //stats.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            int currentWave = GameManager.Instance.currentWave;
            float waveTime = GameManager.Instance.waveEndTime - GameManager.Instance.waveStartTime;
            title.text = "Wave " + currentWave + " Defeated!\nTime Spent: " + Mathf.Round(waveTime) + "s";

            //stats.text = "Time Spent: " + Mathf.Round(waveTime) + "s";

            rewardUI.SetActive(true);
        }
        else
        {
            rewardUI.SetActive(false);
        }
    }
}
