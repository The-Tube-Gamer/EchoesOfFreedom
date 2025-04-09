using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBeatSync : MonoBehaviour
{
    public string paramName;
    public float defaultSpeed = 1f;
    private Animator anim;
    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance != null)
        {
            anim.SetFloat(paramName, defaultSpeed * (120 / GameManager.instance.GetSongBPM()));
        }
    }
}
