using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneProjectile : MonoBehaviour
{
    public PlayerController target;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        transform.right = -(target.transform.position - transform.position);
        //transform.rotation = new Quaternion(0, 0, transform.rotation.z, 0); 
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(-speed, 0, 0));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == target.transform)
        {
            target.Damage(1);
            Destroy(gameObject);
        }
    }
}
