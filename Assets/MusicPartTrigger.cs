using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPartTrigger : MonoBehaviour
{
    public int musicPart;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameManager.instance.SwitchPart(musicPart);
        }
    }
}
