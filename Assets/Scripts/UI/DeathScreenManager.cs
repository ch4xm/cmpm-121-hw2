using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class DeathScreenManager : MonoBehaviour
{
    public GameObject deathUI;
    TextMeshProUGUI title;

    GameObject deathText;

    private void Start()
    {
        deathText = new GameObject("deathText");
        deathText.transform.SetParent(deathUI.transform);
        deathText.transform.localPosition = new Vector3(0, 0);


        title = deathText.AddComponent<TextMeshProUGUI>();
        title.alignment = TextAlignmentOptions.Center;
        title.fontSize = 36;
        title.color = Color.black;
    }

    public void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            int currentWave = GameManager.Instance.currentWave;
            float waveTime = GameManager.Instance.waveEndTime - GameManager.Instance.waveStartTime;
            title.text = "You Died!\n\nYou made it to Wave " + currentWave;

            deathUI.SetActive(true);
        }
        else
        {
            deathUI.SetActive(false);
        }
    }
}
