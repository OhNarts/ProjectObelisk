using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    #region Singleton Stuff
    private static readonly object key = new object();
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null) { Debug.LogError("Audio Manager was null"); }
            return _instance;
        }
    }
    #endregion





    void Awake() {
        lock (key)
        {
            if (_instance == null)
            {
                _instance = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    public static void Play(Sound s) {
        if (s.clip == null)
            return;
        AudioSource.PlayClipAtPoint(s.clip, s.position, volume: s.volume);
        // s.source.Play();
    }
}
