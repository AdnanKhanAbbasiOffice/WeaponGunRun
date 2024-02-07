using System;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    #region Variables

    public Sound[] sounds;


    private int rand;
    private int index = 0;
    private int totalAudioSource = 30;
    AudioSource[] source;
    AudioSource music;
    #endregion Variables

    #region UnityFunction


    private void Awake()
    {
        GlobalAudioPlayer.audioPlayer = this;
        source = new AudioSource[totalAudioSource];
        for (int i = 0; i < totalAudioSource; i++)
        {
            GameObject newgameobject = new GameObject();
            newgameobject.name = "AudioSource " + i;
            source[i] = newgameobject.AddComponent<AudioSource>();
            source[i].playOnAwake = false;
            source[i].transform.parent = transform;
        }

    }

    #endregion UnityFunction

    #region CustomFuntion

    int IndexAudioSource()
    {
        index = 0;
        for (int i = 0; i < totalAudioSource; i++)
        {
            if (source[i].isPlaying)
            {

                index = (index + 1) % totalAudioSource;

            }

            else
                break;

        }
        return index;
    }
    internal void Play(string name)
    {
        if (!GameManager.Instance.sound)
            return;

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound  " + name + " not found");
            return;
        }

        rand = UnityEngine.Random.Range(0, 1000) % s.clip.Length;
        index = IndexAudioSource();
        AudioSource tempSource = source[index];
        tempSource.pitch = s.pitch;
        tempSource.volume = s.volume;
        tempSource.loop = s.loop;
        tempSource.clip = s.clip[rand];
        tempSource.spatialBlend = 0.0f;
        tempSource.Play();
    }
    internal void PlaySoundAtPosition(string name, Transform worldPosition)
    {
        if (!GameManager.Instance.sound)
            return;

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound  " + name + " not found");
            return;
        }






        //pick a random number
        rand = UnityEngine.Random.Range(0, 1000) % s.clip.Length;
        index = IndexAudioSource();





        GameObject audioObj = new GameObject();
        audioObj.transform.parent = worldPosition;
        audioObj.name = name;
        audioObj.transform.position = worldPosition.position;
        AudioSource audiosource = audioObj.AddComponent<AudioSource>();

        //audio source settings
        audiosource.clip = s.clip[rand];
        audiosource.spatialBlend = 1.0f;
        audiosource.minDistance = 4f;
        audiosource.volume = s.volume;
        audiosource.loop = s.loop;
        audiosource.Play();

        //Destroy on finish
        if (!s.loop && audiosource.clip != null)
        {
            TimeToLive TTL = audioObj.AddComponent<TimeToLive>();
            TTL.LifeTime = audiosource.clip.length;
        }
    }
    internal void PlayMusic(string name)
    {

        //create a separate gameobject designated for playing music
        GameObject temp = new GameObject();
        temp.name = "Music";
        music = temp.AddComponent<AudioSource>();
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Name  " + name + " not found");
            return;
        }
        music.clip = s.clip[0];
        music.loop = true;
        music.volume = s.volume;
        music.pitch = s.pitch;
        music.Play();
        MusicOnOff();
    }
    internal void MusicOnOff()
    {
        if (music != null)
        {
            if (GameManager.Instance.music)
                music.mute = false;
            else
                music.mute = true;
        }

    }
    #endregion CustomFuntion

}
