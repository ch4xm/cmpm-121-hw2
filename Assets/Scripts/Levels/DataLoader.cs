using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
public class DataLoader
{
    public static List<Level> ReadLevels()
    {
        // read levels.json
        string json = File.ReadAllText("Assets/Resources/levels.json");
        var levels = JsonConvert.DeserializeObject<List<Level>>(json);

        return levels;
    }
    public static Dictionary<string, Enemy> ReadEnemies()
    {
        // read enemies.json
        string json = File.ReadAllText("Assets/Resources/enemies.json");

        var result = JsonConvert.DeserializeObject<List<Enemy>>(json);
        var enemiesDict = result.ToDictionary(x => x.name, x => x); // Convert result to dict of name to enemy pairs for easy enemy access

        return enemiesDict;
    }
}
