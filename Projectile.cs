using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    private GameObject caster;
    private float speed;
    private float range;
    private Vector3 travelDirection;

    private float distanceTraveled;

    public event Action<GameObject, GameObject> ProjectileCollided;

    public void Fire(GameObject Caster, Vector3 Target, float Speed, float Range)
    {
        caster = Caster;
        speed = Speed;
        range = Range;

        // calculate travel direction
        travelDirection = Target - transform.position;
        travelDirection.y = 0f;
        travelDirection.Normalize();

        //initialize distance traveled
        distanceTraveled = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //move this projectile through space
        float distanceToTravel = speed * Time.deltaTime;

        transform.Translate(travelDirection * distanceToTravel);

        //check to see if we traveled too far, if so destroy this projectile
        distanceTraveled += distanceToTravel;
        if (distanceToTravel > range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        //raise an event
        if (ProjectileCollided != null)
        {
            ProjectileCollided(caster, other.gameObject);
        }

        //destroy Object
        Destroy(gameObject);
    }
}
