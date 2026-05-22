using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Unit unit;
    public Transform target;
    public int speed;
    public Hittable hp;
    public HealthBar healthui;
    public bool dead;
    public int damage;

    public float freeze_timestamp = 0;
    public float freeze_length = 0;

    public float cooldown = 0.75f;

    public float last_attack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameManager.Instance.player.transform;
        hp.OnDeath += Die;
        hp.OnDeath += () => EventBus.Instance.DoKill(Time.time, GameManager.Instance.player.GetComponent<PlayerController>().hp, hp);
        healthui.SetHealth(hp);

        unit = GetComponent<Unit>();
    }

    public void Freeze(float time)
    {
        freeze_timestamp = Time.time;
        freeze_length = time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - freeze_timestamp < freeze_length)   // Freeze enemy
        {
            unit.movement = Vector2.zero;
            return;
        }
        if (GameManager.Instance.state != GameManager.GameState.INWAVE)
        {
            unit.movement = Vector2.zero;
            return;
        }

        Vector3 direction = target.position - transform.position;
        if (direction.magnitude < 2f)
        {
            DoAttack();
        }
        else
        {
            unit.movement = direction.normalized * speed;
        }
    }
    
    void DoAttack()
    {
        if (last_attack + cooldown < Time.time)
        {
            last_attack = Time.time;
            target.gameObject.GetComponent<PlayerController>().hp.Damage(new Damage(damage, Damage.Type.PHYSICAL));
        }
    }


    void Die()
    {
        if (!dead)
        {
            dead = true;
            GameManager.Instance.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}
