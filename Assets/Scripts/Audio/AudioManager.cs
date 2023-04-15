using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    #region Singleton Stuff
    private static readonly object key = new object();
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) { Debug.LogError("Game Manager was null"); }
            return _instance;
        }
    }
    #endregion





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

    public static void Play(Sound s) {
        if (s.clip == null)
            return;
        AudioSource.PlayClipAtPoint(s.clip, s.position, volume: s.volume);
        // s.source.Play();
    }
}
