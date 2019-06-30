using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Rigidbody RagdollCore;
    public float timeToLive;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToLive);
    }

    public void ApplyForce(Vector3 force)
    {
        RagdollCore.AddForce(force);
    }
}
