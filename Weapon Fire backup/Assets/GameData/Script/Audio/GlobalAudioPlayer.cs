using UnityEngine;

public static class GlobalAudioPlayer
{

    public static AudioManager audioPlayer;

    public static void Play(string name)
    {
        if (audioPlayer != null && name != "") audioPlayer.Play(name);
    }

    public static void PlaySoundAtPosition(string name, Transform worldposition)
    {
        if (audioPlayer != null && name != "") audioPlayer.PlaySoundAtPosition(name, worldposition);
    }


    public static void PlayMusic(string musicName)
    {
        if (audioPlayer != null) audioPlayer.PlayMusic(musicName);
    }
    public static void MusicOnOff()
    {
        if (audioPlayer != null) audioPlayer.MusicOnOff();
    }
}
