using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUIContainer spellUI;

    public List<Relic> relics;

    public int speed;

    public Unit unit;

    public float lastMove;
    public float lastStand;
    
    public Dictionary<string, Relic> relicTypes;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
        relicTypes = DataLoader.ReadRelics();
        relicTypes["Green Gem"].Activate();
        
        GameManager.Instance.player = gameObject;
    }

    public void StartLevel()
    {
        Debug.Log(spellUI);

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

    public void LevelUp(int wave)
    {
        Dictionary<string, int> vars = new Dictionary<string, int>();
        vars["wave"] = wave;
        hp.SetMaxHP(RPNEvaluator.RPNEvaluator.Evaluate("95 wave 5 * +", vars));
        spellcaster.max_mana = RPNEvaluator.RPNEvaluator.Evaluate("90 wave 10 * +", vars);
        spellcaster.mana_reg = RPNEvaluator.RPNEvaluator.Evaluate("10 wave +", vars);
        spellcaster.spell_power = RPNEvaluator.RPNEvaluator.Evaluate("wave 10 *", vars);
        speed = 5;
    }
}
