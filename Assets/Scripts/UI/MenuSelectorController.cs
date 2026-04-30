using UnityEngine;
using TMPro;

public class MenuSelectorController : MonoBehaviour
{
    public TextMeshProUGUI label;
    public string level;
    public EnemySpawner spawner;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevel(string text)
    {
        level = text;
        label.text = text;
    }

    public void StartLevel()
    {
        if (level == "New Game") spawner.LevelSelectMenu();
        else if (level == "Next Wave")
        {
            spawner.level_selector.gameObject.SetActive(false);
            spawner.NextWave();
        }
        else
        {
            spawner.level_selector.gameObject.SetActive(false);
            spawner.StartLevel(level);
        }

    }
}
