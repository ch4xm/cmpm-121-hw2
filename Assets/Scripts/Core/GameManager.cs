using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameManager 
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,    // All enemies killed, game should show menu when this state is true
        COUNTDOWN,
        GAMEOVER,
        MENU
    }
    public GameState state;

    public int countdown;

    public Dictionary<string, Enemy> enemyTypes;    // Store in key value pairs of enemy name ("zombie") to Enemy template classes
    public List<Level> levels;

    public Level currentLevel;
    public int currentWave = 1;
    public float waveStartTime;
    public float waveEndTime;

    private static GameManager theInstance;
    public static GameManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new GameManager();
            return theInstance;
        }
    }

    public GameObject player;
    
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }
    
    public void RemoveAllEnemies()
    {
        foreach (var item in enemies)
        {
            UnityEngine.Object.Destroy(item);
        }
        enemies.Clear();
    }

    public void SetupLevel(string levelName)
    {
        RemoveAllEnemies();
        Instance.currentWave = 1;
        Instance.currentLevel = levels.Find(x => x.name == levelName);

        var relic = new Relic();
    }
    
    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a,b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    private GameManager()
    {
        levels = DataLoader.ReadLevels();
        enemyTypes = DataLoader.ReadEnemies();
        DataLoader.ReadRelics();

        enemies = new List<GameObject>();
    }
}
