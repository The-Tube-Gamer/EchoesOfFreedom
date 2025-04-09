using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int health = 1;
    public float speed;
    public int onBeatDamageBonus;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            int damage = 1;
            if (GameManager.instance.beat)
            {
                damage += onBeatDamageBonus;
            }
            health -= damage;
        }
        else if (collision.gameObject.tag == "Player")
        {
            
        }
    }
}
