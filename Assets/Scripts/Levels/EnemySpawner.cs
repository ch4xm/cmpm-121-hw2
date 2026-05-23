using Newtonsoft.Json;
using RPNEvaluator;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public SpawnPoint[] SpawnPoints;

    private int activeSpawnGroups = 0;

    public string selectedLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventBus.Instance.OnGameStart += StartLevel;
        EventBus.Instance.OnNextWave += NextWave;

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
        for (int i = 0; i < GameManager.Instance.levels.Count; ++i)
        {
            GameObject selector = Instantiate(button, level_selector.transform);
            selector.transform.localPosition = new Vector3(0, 130 - 100 * i); // TODO: make this dynamic to the number of bottoms
            selector.GetComponent<MenuSelectorController>().spawner = this;
            selector.GetComponent<MenuSelectorController>().SetLevel(GameManager.Instance.levels[i].name); // Shouldnt start yet, only start when level button is clicked
        }
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

    public void StartLevel(string levelName, string playerClass)
    {
        // TODO: use playerclass string to scale stats
        StopAllCoroutines();
        GameManager.Instance.SetupLevel(levelName);

        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        level_selector.gameObject.SetActive(false);

        var player = GameManager.Instance.player.GetComponent<PlayerController>();

        player.SetClass(playerClass);
        player.StartLevel();
        
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        GameManager.Instance.currentWave++;
        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()
    {
        GameManager.Instance.player.GetComponent<PlayerController>().ApplyScaling(GameManager.Instance.currentWave);
        
        activeSpawnGroups = 0;

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

        foreach (var item in GameManager.Instance.currentLevel.spawns)  // todo: change to current level
        {
            activeSpawnGroups++;
            routines.Add(StartCoroutine(SpawnGroup(item, GameManager.Instance.currentWave))); // Spawn coroutine so each spawn group spawns simultaneously
        } // TODO: change to current wave

        yield return new WaitUntil(() => activeSpawnGroups == 0);   // Only allow win condition once all enemies have spawned
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);

        GameManager.Instance.state = GameManager.GameState.WAVEEND;

        GameManager.Instance.waveEndTime = Time.time;

        EventBus.Instance.DoWaveEnd(GameManager.Instance.currentWave);

        if (GameManager.Instance.currentLevel.waves.HasValue && GameManager.Instance.currentWave >= GameManager.Instance.currentLevel.waves.Value)
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

        var currentEnemy = GameManager.Instance.enemyTypes[item.enemy];

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

        en.hp = new Hittable(Mathf.RoundToInt(enemyData.hp), Hittable.Team.MONSTERS, new_enemy, en);
        en.speed = (int) enemyData.speed;
        en.damage = (int) enemyData.damage;
        GameManager.Instance.AddEnemy(new_enemy);
        yield return new WaitForSeconds(0.5f);
    }
}
