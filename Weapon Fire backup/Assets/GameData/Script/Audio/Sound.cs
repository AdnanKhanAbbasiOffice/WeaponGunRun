using UnityEngine;
[System.Serializable]
public class Sound
{

    [SerializeField] internal string name;
    [SerializeField] internal AudioClip[] clip;
    [Range(0f, 1f)]
    [SerializeField] internal float volume = 1f;
    [Range(0.1f, 3f)]
    [SerializeField] internal float pitch = 1f;
    [SerializeField] internal bool loop;
    internal AudioSource source;

    public Sound(string names) { this.name = names; }
}
