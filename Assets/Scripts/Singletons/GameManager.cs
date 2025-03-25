using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    public static GameManager instance = null;
    public AudioSource player;
    public Music[] trackList;
    public bool beat;
    public int currentSong;
    public float timer;
    public float timerRounded;
    public bool musicPlaying;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        float roundAmount = 4;
        //timerRounded = Mathf.Round(timer * roundAmount) / roundAmount;
        Debug.Log(timerRounded % (trackList[currentSong].bpm / 120f));
        if (timer >= (120f / trackList[currentSong].bpm))
        {
            timer = 0;
            beat = true;
            if (!musicPlaying)
            {
                GetComponent<AudioSource>().Play();
                musicPlaying = true;
            }
        }
        else if (timer <= 0.5f)
            {
            beat = true;
        }
        else
        {
            beat = false;
        }

    }
    void Awake()
    {
        
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(this.gameObject);
    }
}
