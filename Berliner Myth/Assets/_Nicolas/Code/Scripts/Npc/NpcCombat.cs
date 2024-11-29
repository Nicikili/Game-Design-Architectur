using UnityEngine;

public class NpcCombat : NpcComponent
{
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCoolDown = 1f;
    private float attackTimer = 0f;

    [SerializeField] private float attackRange = 2f;

    new protected void Awake()
    {
        if (npc != null)
        {
            npc.CurrentHealth = maxHealth;
        }
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        npc.CurrentHealth -= damage;
        npc.CurrentHealth = Mathf.Clamp(npc.CurrentHealth, 0f, maxHealth);

        if (npc.CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void Attack(NpcCombat target)
    {
        if (target != null)
        {

            Debug.Log("Found target");
            if (attackTimer <= 0 && Vector3.Distance(transform.position, target.transform.position) <= attackRange)
            {
                Debug.Log("Target is in range");

                target.TakeDamage(attackDamage);
                attackTimer = attackCoolDown;
            }
        }
        else
        {
            Debug.LogError("Target is null");
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
   
}
