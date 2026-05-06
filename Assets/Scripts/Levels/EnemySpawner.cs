using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPNEvaluator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using State = Unity.VisualScripting.State;

#nullable enable

// helper class for reading level.json
public class Spawn
{
    public string enemy;
    public string count;
    public List<int>? sequence;
    public string? delay;
    public string? location;
    public string? hp;
    public string? speed;
    public string? damage;
}

// helper class for reading level.json
public class Level
{
    public string name;
    public int? waves;
    public List<Spawn> spawns;
}

// helper class for reading enemies.json
public class Enemy
{
    public string name;
    public int sprite;
    public float hp;
    public float speed;
    public float damage;
}

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public Dictionary<string, Enemy> enemyTypes;    // Store in key value pairs of enemy name ("zombie") to Enemy template classes
    public List<Level> levels;
    public SpawnPoint[] SpawnPoints;

    private bool gameWon = false;

    private int activeSpawnGroups = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // deserialize here into enemyTemplates and levels
        levels = ReadLevels();
        enemyTypes = ReadEnemies();
        
        LevelSelectMenu();
    }

    public void LevelSelectMenu()
    {
        GameManager.Instance.state = GameManager.GameState.MENU;
        foreach (Transform child in level_selector.transform)
        {
            Destroy(child.gameObject);  // Clear old buttons
        }
        level_selector.gameObject.SetActive(true);
        for (int i = 0; i < levels.Count; ++i)
        {
            GameObject selector = Instantiate(button, level_selector.transform);
            selector.transform.localPosition = new Vector3(0, 130 - 100 * i); // TODO: make this dynamic to the number of bottoms
            selector.GetComponent<MenuSelectorController>().spawner = this;
            selector.GetComponent<MenuSelectorController>().SetLevel(levels[i].name); // Shouldnt start yet, only start when level button is clicked
        }
    }
    
    public void NewGameMenu()
    {
        GameManager.Instance.RemoveAllEnemies();
        
        GameManager.Instance.state = GameManager.GameState.MENU;
        foreach (Transform child in level_selector.transform)
        {
            Destroy(child.gameObject);
        }
        level_selector.gameObject.SetActive(true);
        GameObject selector = Instantiate(button, level_selector.transform);
        selector.transform.localPosition = new Vector3(0, 100);
        selector.GetComponent<MenuSelectorController>().spawner = this;
        selector.GetComponent<MenuSelectorController>().SetLevel("New Game");
    }

    public void WinMenu()
    { 
        GameManager.Instance.state = GameManager.GameState.MENU;
        foreach (Transform child in level_selector.transform)
        {
            Destroy(child.gameObject);
        }
        level_selector.gameObject.SetActive(true);

        // Create a new GameObject for the text
        GameObject statObject = new GameObject("StatText");
        statObject.transform.SetParent(level_selector.transform);
        statObject.transform.localPosition = new Vector3(0, 0);
        
        // Add TextMeshProUGUI to the new GameObject
        TextMeshProUGUI stat = statObject.AddComponent<TextMeshProUGUI>();
        stat.text = "You Win!";
        stat.alignment = TextAlignmentOptions.Center;
        stat.fontSize = 36;
        stat.color  = Color.black;
        
        GameObject selector = Instantiate(button, level_selector.transform);
        selector.transform.localPosition = new Vector3(0, 100);
        selector.GetComponent<MenuSelectorController>().spawner = this;
        selector.GetComponent<MenuSelectorController>().SetLevel("New Game");
    }
    
    private List<Level> ReadLevels()
    {
        // read levels.json
        string json = File.ReadAllText("Assets/Resources/levels.json");
        var levels = JsonConvert.DeserializeObject<List<Level>>(json);

        return levels;
    }
    private Dictionary<string, Enemy> ReadEnemies()
    {
        // read enemies.json
        string json = File.ReadAllText("Assets/Resources/enemies.json");

        var result = JsonConvert.DeserializeObject<List<Enemy>>(json);
        var enemiesDict = result.ToDictionary(x => x.name, x => x); // Convert result to dict of name to enemy pairs for easy enemy access

        return enemiesDict;
    }

    // Update is called once per frame
    void Update()
    {
        //if ((GameManager.Instance.state == GameManager.GameState.INWAVE ||
        //     GameManager.Instance.state == GameManager.GameState.WAVEEND)
        //    && GameManager.Instance.player.GetComponent<PlayerController>().hp.hp == 0)
        //{
        //    NewGameMenu();
        //}
    }

    public void StartLevel(string levelname)
    {
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.currentLevel = levels.FindIndex(x => x.name == levelname);
        level_selector.gameObject.SetActive(false);

        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        GameManager.Instance.currentWave++;

        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()
    {
        activeSpawnGroups = 0;
        Level currentLevel = levels[GameManager.Instance.currentLevel];

        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        GameManager.Instance.waveStartTime = Time.time;

        List<Coroutine> routines = new();

        foreach (var item in currentLevel.spawns)  // todo: change to current level
        {
            activeSpawnGroups++;
            routines.Add(StartCoroutine(SpawnGroup(item, GameManager.Instance.currentWave))); // Spawn coroutine so each spawn group spawns simultaneously
        } // TODO: change to current wave

        yield return new WaitUntil(() => activeSpawnGroups == 0);   // Only allow win condition once all enemies have spawned
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);

        GameManager.Instance.state = GameManager.GameState.WAVEEND;
        GameManager.Instance.waveEndTime = Time.time;
        if (currentLevel.waves.HasValue && GameManager.Instance.currentWave >= currentLevel.waves.Value)
        {
            GameManager.Instance.state = GameManager.GameState.GAMEOVER;
            WinMenu();
        }
    }

    IEnumerator SpawnGroup(Spawn item, int wave)
    {
        var variables = new Dictionary<string, float>
        {
            { "wave", wave }
        };

        var currentEnemy = enemyTypes[item.enemy];

        variables["base"] = currentEnemy.hp;
        var calculatedHealth = RPNEvaluator.RPNEvaluator.Evaluatef(item.hp ?? "base", variables);

        variables["base"] = currentEnemy.damage;
        var calculatedDamage = RPNEvaluator.RPNEvaluator.Evaluatef(item.damage ?? "base", variables);

        variables["base"] = currentEnemy.speed;
        var calculatedSpeed = RPNEvaluator.RPNEvaluator.Evaluatef(item.speed ?? "base", variables);

        var location = item.location ?? "random";

        var calculatedCount = (int) RPNEvaluator.RPNEvaluator.Evaluatef(item.count, variables);
        var sequence = item.sequence ?? new List<int>() { 1 };
        var calculatedDelay = RPNEvaluator.RPNEvaluator.Evaluatef(item.delay ?? "2", variables);

        var enemyData = new Enemy()
        {
            name = currentEnemy.name,
            sprite = currentEnemy.sprite,
            hp = calculatedHealth,
            damage = calculatedDamage,
            speed = calculatedSpeed,
        };

        int spawned = 0;

        for (int i = 0; i < calculatedCount; i++)
        {
            int spawnAmount = Math.Min(sequence[i % sequence.Count], calculatedCount - spawned);    // Cap spawned amount to not go over total count for spawn

            for (int j = 0; j < spawnAmount; j++)
            {
                yield return SpawnEnemy(enemyData, location);
            }

            spawned += spawnAmount;
            if (spawned < calculatedCount)
            {
                yield return new WaitForSeconds(calculatedDelay);
            }
        }

        activeSpawnGroups--;
    }

    IEnumerator SpawnEnemy(Enemy enemyData, string location)
    {
        SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        while ("random " + spawn_point.kind.ToString().ToLower() != location && location != "random")
        {
            spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        }
        
        Vector2 offset = Random.insideUnitCircle * 1.8f;

        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
         GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(enemyData.sprite);
        EnemyController en = new_enemy.GetComponent<EnemyController>();

        en.hp = new Hittable(Mathf.RoundToInt(enemyData.hp), Hittable.Team.MONSTERS, new_enemy);
        en.speed = (int) enemyData.speed;
        en.damage = (int) enemyData.damage;
        GameManager.Instance.AddEnemy(new_enemy);
        yield return new WaitForSeconds(0.5f);
    }
}
