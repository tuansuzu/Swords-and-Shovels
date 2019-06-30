using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class HeroController : MonoBehaviour
{
    public AttackDefinition demoAttack;
    public Aoe aoeStompAttack;

    Animator animator; // reference to the animator component
    NavMeshAgent agent; // reference to the NavMeshAgent
    CharacterStats stats;

    private GameObject attackTarget;
    private GameObject[] enemies;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<CharacterStats>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        InvokeRepeating("Tick", 0, 0.5f);

    }

    public void SetDestination(Vector3 destination)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = destination;
    }

    public void AttackTarget(GameObject target)
    {
        var weapon = stats.GetCurrentWeapon();

        if (weapon != null)
        {
            StopAllCoroutines();

            agent.isStopped = false;
            attackTarget = target;
            StartCoroutine(PursueAndAttackTarget());
        }
    }

    private IEnumerator PursueAndAttackTarget()
    {
        agent.isStopped = false;
        var weapon = stats.GetCurrentWeapon();

        while (Vector3.Distance(transform.position, attackTarget.transform.position) > weapon.Range)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
                 
        }

        agent.isStopped = true;

        transform.LookAt(attackTarget.transform);
        animator.SetTrigger("Attack");
    }

    public void Hit()
    {
        //have our weapon attack the attack target
        if (attackTarget != null)
            stats.GetCurrentWeapon().ExecuteAttack(gameObject, attackTarget);
    }

    void Tick()
    {

        foreach (GameObject g in enemies)
        {
            if (g != null && Vector3.Distance(transform.position, g.transform.position) < 10)
            {
                AttackTarget(g);
                break;

            }
        }

       


    }

    public void Stomp()
    {
        aoeStompAttack.Fire(gameObject, gameObject.transform.position, LayerMask.NameToLayer("PlayerSpells"));
    }

}
