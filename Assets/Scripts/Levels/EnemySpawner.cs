using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

// helper class for reading level.json
public class Spawn
{
    public string enemy;
    public string count;
    public string hp;
    public string? delay;
    public List<int>? sequence;
    public string location;
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
    public int hp;
    public int speed;
    public int damage;
}

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public Dictionary<string, Enemy> enemyTypes;    // Store in key value pairs of enemy name ("zombie") to Enemy template classes
    public List<Level> levels;
    public SpawnPoint[] SpawnPoints;    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // deserialize here into enemyTemplates and levels
        levels = ReadLevels();
        enemyTypes = ReadEnemies();

        for (int i = 0; i < levels.Count; ++i)
        {
            GameObject selector = Instantiate(button, level_selector.transform);
            selector.transform.localPosition = new Vector3(0, 130 - 100 * i); // TODO: make this dynamic to the number of bottoms
            selector.GetComponent<MenuSelectorController>().spawner = this;
            selector.GetComponent<MenuSelectorController>().SetLevel(levels[i].name); // Shouldnt start yet, only start when level button is clicked
        }

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
        
    }

    public void StartLevel(string levelname)
    {
        level_selector.gameObject.SetActive(false);
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        for (int i = 0; i < 10; ++i)
        {
            yield return SpawnEnemy("zombie");
        }
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }

    IEnumerator SpawnEnemy(string enemyName)
    {
        var enemyType = enemyTypes[enemyName];

        SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;

        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(enemyType.sprite);
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(enemyType.hp, Hittable.Team.MONSTERS, new_enemy);
        en.speed = enemyType.speed;
        GameManager.Instance.AddEnemy(new_enemy);
        yield return new WaitForSeconds(0.5f);
    }
}
