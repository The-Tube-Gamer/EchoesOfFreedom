using UnityEngine;

[System.Serializable]
public class Music
{
    public AudioClip[] audioClips;
    public string name;
    public int bpm;
    int GetBPM()
    {
        return bpm;
    }
    AudioClip GetAudio(int n)
    {
        return audioClips[n];
    }
}
