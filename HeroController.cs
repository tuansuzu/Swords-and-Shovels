using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class HeroController : MonoBehaviour
{
    public AttackDefinition demoAttack;

    Animator animator; // reference to the animator component
    NavMeshAgent agent; // reference to the NavMeshAgent
    CharacterStats stats;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<CharacterStats>();
    }

    private void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);

    }

    public void SetDestination(Vector3 destination)
    {
        agent.destination = destination;
    }

    public void AttackTarget(GameObject target)
    {
        var attack = demoAttack.CreateAttack(stats, target.GetComponent<CharacterStats>());

        var attackables = target.GetComponentsInChildren(typeof(IAttackable));

        foreach (IAttackable attackable in attackables)
        {
            attackable.OnAttack(gameObject, attack);
        }
    }

}
