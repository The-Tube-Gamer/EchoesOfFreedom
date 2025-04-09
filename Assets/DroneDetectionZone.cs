using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDetectionZone : MonoBehaviour
{
    public Drone drone;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        drone.SetAngry(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        drone.SetAngry(false);
    }
}
