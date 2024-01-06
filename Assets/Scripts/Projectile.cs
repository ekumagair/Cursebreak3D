using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;
    public string ignoreTag = "";
    public GameObject[] createObjectOnHit;

    private bool _canEnterTrigger = true;

    void Update()
    {
        transform.Translate(speed * Vector3.forward * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject hitObj = other.gameObject;

        if (_canEnterTrigger && hitObj.tag != ignoreTag && hitObj.tag != "Projectile" && hitObj.tag != "Canvas" && hitObj.tag != "Item" && hitObj.tag != "Teleporter" && hitObj.tag != "ProjectileIgnores" && hitObj.tag != "EditorOnly")
        {
            _canEnterTrigger = false;

            if (hitObj.GetComponent<Health>() != null)
            {
                hitObj.GetComponent<Health>().TakeDamage(damage, false);
            }

            if (createObjectOnHit.Length > 0)
            {
                foreach (GameObject obj in createObjectOnHit)
                {
                    if (obj != null)
                    {
                        Instantiate(obj, transform.position, transform.rotation);
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}
