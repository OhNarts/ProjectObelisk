using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake() {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        // foreach (Sound s in sounds) {
        //     s.source = gameObject.AddComponent<AudioSource>();
        //     s.source.clip = s.clip;
        //     s.source.volume = s.volume;
        //     s.source.pitch = s.pitch;
        // }
    }

    // public void Play(Sound s) {
    //     if (s == null)
    //         return;
    //     s.source.Play();
    // }
}
