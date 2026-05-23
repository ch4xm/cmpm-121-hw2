using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = System.Object;

public class DataLoader
{
    public static List<Level> ReadLevels()
    {
        // read and deserialize levels.json
        string json = File.ReadAllText("Assets/Resources/levels.json");
        var levels = JsonConvert.DeserializeObject<List<Level>>(json);

        return levels;
    }
    public static Dictionary<string, Enemy> ReadEnemies()
    {
        // read and deserialize enemies.json
        string json = File.ReadAllText("Assets/Resources/enemies.json");

        var result = JsonConvert.DeserializeObject<List<Enemy>>(json);
        var enemiesDict = result.ToDictionary(x => x.name, x => x); // Convert result to dict of name to enemy pairs for easy enemy access

        return enemiesDict;
    }

    public class SpellReadResult
    {
        public Dictionary<string, SpellData> Spells { get; set; } = new();
        public Dictionary<string, ModifierData> Modifiers { get; set; } = new();
    }

    public static SpellReadResult ReadSpells()
    {
        // read and deserialize spells.json
        string json = File.ReadAllText("Assets/Resources/spells.json");

        SpellReadResult result = new();


        var parsedSpells = JsonConvert.DeserializeObject<Dictionary<string, Object>>(json);

        foreach (var spell in parsedSpells)
        {
            var obj = (JObject) spell.Value;
            if (obj.ContainsKey("projectile"))
            {
                result.Spells.Add(spell.Key, obj.ToObject<SpellData>());
            }
            else
            {
                result.Modifiers.Add(spell.Key, obj.ToObject<ModifierData>());
            }
        }
        JsonConvert.DeserializeObject<Dictionary<string, SpellData>>(json);

        return result;
    }

    public static Dictionary<string, Relic> ReadRelics()
    {
        // read and deserialize relics.json
        string json = File.ReadAllText("Assets/Resources/relics.json");
        
        var result = JsonConvert.DeserializeObject<List<Relic>>(json);

        foreach (var relic in result)
        {
            Debug.Log("Relic: " + relic.name);
            Debug.Log("Trigger: " + relic.trigger.type + " " + " type:  " + relic.trigger.GetType().Name);
            Debug.Log("Effect: " + relic.effect.type + " " + " type:  " + relic.effect.GetType().Name);
            Debug.Log("-----");
        }
        
        var relicDict = result.ToDictionary(x => x.name, x => x);
        
        return relicDict;
    }

    public static Dictionary<string, PlayerClass> ReadPlayerClasses()
    {
        // read and deserialize classes.json
        string json = File.ReadAllText("Assets/Resources/classes.json");

        var temp = JsonConvert.DeserializeObject<Dictionary<string, PlayerClass>>(json);

        var result = new Dictionary<string, PlayerClass>(temp);
        foreach (var key in temp.Keys.ToList())
        {
            result[key].name = key;
        }

        return result;
    }
}
