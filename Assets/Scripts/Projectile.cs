using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;
    public string ignoreTag = "";
    public GameObject createObjectOnHit;

    void Update()
    {
        transform.Translate(speed * Vector3.forward * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject hitObj = other.gameObject;

        if (hitObj.tag != ignoreTag && hitObj.tag != "Projectile" && hitObj.tag != "Canvas" && hitObj.tag != "Item")
        {
            if(hitObj.GetComponent<Health>() != null)
            {
                hitObj.GetComponent<Health>().TakeDamage(damage, false);
            }

            if (createObjectOnHit != null)
            {
                Instantiate(createObjectOnHit, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
    }
}
