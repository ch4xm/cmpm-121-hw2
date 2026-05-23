using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    private PlayerClass currentClass;

    public SpellCaster spellcaster;
    public SpellUIContainer spellUI;
    
    public int speed;

    public Unit unit;

    public float lastMove;
    public float lastStand;

    public GameObject sprite;

    public Dictionary<string, Relic> relics;
    
    public float healingOverTime = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();

        relics = DataLoader.ReadRelics();
        
        GameManager.Instance.player = gameObject;
        
        AddRelic(GameManager.Instance.relicTypes["Speed Bracelet"]);

        EventBus.Instance.OnRelicSelected += AddRelic;
        
        InvokeRepeating("HealingOverTime", 0, 1);
    }

    public void SetClass(string className)
    {
        currentClass = GameManager.Instance.playerClasses[className];

        SetIcon(currentClass.sprite);
    }

    public void SetIcon(int icon)
    {
        sprite.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.playerSpriteManager.Get(icon);
    }

    public void AddRelic(Relic relic)
    {
        relic.Activate();
        relics.Add(relic);
    }

    public void StartLevel()
    {
        relics = new List<Relic>(); // TODO: refactor this

        spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
        spellUI.RefreshUI();

        StartCoroutine(spellcaster.ManaRegeneration());
        
        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject, this);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
    }

    // Update is called once per frame
    void Update()
    {
        if (spellcaster is not null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) spellcaster.SelectSpell(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) spellcaster.SelectSpell(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) spellcaster.SelectSpell(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) spellcaster.SelectSpell(3);
        }

        if (unit.movement == Vector2.zero)
        {
            EventBus.Instance.DoStandStill(Time.time - lastMove, hp);
            lastStand = Time.time;
        }
        else
        {
            EventBus.Instance.DoMove(Time.time - lastStand, hp);
            lastMove = Time.time;
        }
        
        EventBus.Instance.DoNotTakingDamage(Time.time - hp.lastDamageTime, hp);
    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state != GameManager.GameState.INWAVE && 
            GameManager.Instance.state != GameManager.GameState.COUNTDOWN) return;
        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;
        StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
    }

    // in amount per second
    public void HealingOverTime()
    {
        hp.Heal(healingOverTime);
    }
    
    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state != GameManager.GameState.INWAVE &&
            GameManager.Instance.state != GameManager.GameState.COUNTDOWN &&
            GameManager.Instance.state != GameManager.GameState.WAVEEND) return;

        unit.movement = value.Get<Vector2>()*speed;
    }

    void Die()
    {
        unit.movement = Vector2.zero;
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
        Debug.Log("You Lost");
    }

    public void ApplyScaling(int wave)
    {
        Dictionary<string, int> vars = new ()
        {
            ["wave"] = wave
        };

        hp.SetMaxHP(RPNEvaluator.RPNEvaluator.Evaluate(currentClass.health, vars));
        spellcaster.max_mana = RPNEvaluator.RPNEvaluator.Evaluate(currentClass.mana, vars);
        spellcaster.mana_reg = RPNEvaluator.RPNEvaluator.Evaluate(currentClass.mana_regeneration, vars);
        spellcaster.spell_power = RPNEvaluator.RPNEvaluator.Evaluate(currentClass.spellpower, vars);
        speed = RPNEvaluator.RPNEvaluator.Evaluate(currentClass.speed, vars);
    }
    void OnDestroy()
    {
        EventBus.Instance.OnRelicSelected -= AddRelic;
    }
}
