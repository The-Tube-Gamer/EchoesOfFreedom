using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : EnemyBase
{
    public int direction = 1;
    public float maxDist;
    public GameObject sprite;
    public bool isAngry;
    public Animator anim;
    public Transform projectileSpawnPoint;
    public float shootTime;
    public GameObject projectile;
    public PlayerController player;
    public bool canShoot;
    float trueShootTime;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        trueShootTime = shootTime;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Angry", isAngry);
        if (!isAngry)
        {
            Vector3 tempPos = transform.position;
            tempPos.x += speed * direction;
            transform.position = tempPos;
        }
        if (Mathf.Abs(startPos.x - transform.position.x) >= maxDist)
        {
            direction *= -1;
        }
        Vector3 tempScale = sprite.transform.lossyScale;
        tempScale.x = direction;
        sprite.transform.localScale = tempScale;
        if (isAngry && canShoot)
        {
            trueShootTime -= Time.deltaTime;
            if (trueShootTime <= 0)
            {
                GameObject newProjectile = Instantiate(projectile, projectileSpawnPoint.position, transform.rotation);
                newProjectile.GetComponent<DroneProjectile>().target = player;
                trueShootTime = shootTime;
            }
        }
    }
    public void SetAngry(bool a)
    {
        isAngry = a;
    }
}
