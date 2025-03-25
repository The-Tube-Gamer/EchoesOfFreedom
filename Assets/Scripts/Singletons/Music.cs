using UnityEngine;

[System.Serializable]
public class Music
{
    public AudioClip audio;
    public string name;
    public int bpm;
    int GetBPM()
    {
        return bpm;
    }
    AudioClip GetAudio()
    {
        return audio;
    }
}
