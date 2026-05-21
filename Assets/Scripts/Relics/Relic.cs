using UnityEngine;
using System.Collections;
using System.Dynamic;

public class Relic
{
    public string name;
    public string description;
    public Sprite sprite;

    public Trigger trigger;

    public Relic(string name, string description, Sprite sprite, Trigger trigger)
    {
        this.name = name;
        this.description = description;
        this.sprite = sprite;
        this.trigger = trigger;
    }
}
