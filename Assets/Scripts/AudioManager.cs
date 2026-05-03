using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public enum SoundType
    {
        WaterPowerReg,
        WaterPowerBigBlast,
        GuardSwing,
        SlowDownTime,
        RewindTime,
        WaterFountain,
        WaterRefill,
        Music_Prince,
        Music_King,
        Music_General,
        Music_Hub,
        UI_Click,
        Object_Pickup
        

    }

    [System.Serializable]
    public class Sound
    {
        public SoundType Type;
        public AudioClip Clip;

        [Range(0f, 1f)]
        public float Volume = 1f;
    }

    public static AudioManager Instance;

    public Sound[] AllSounds;

    private Dictionary<SoundType, Sound> _soundDictionary = new Dictionary<SoundType, Sound>();
    private AudioSource _musicSource;

    private void Awake()
    {
        // Singleton safety
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build dictionary
        foreach (var s in AllSounds)
        {
            _soundDictionary[s.Type] = s;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

// Tries to switch to Scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        switch (scene.name)
        {
            case "Hub":
                ChangeMusic(SoundType.Music_Hub);
                break;

            case "Level_3":
                ChangeMusic(SoundType.Music_Prince);
                break;

            case "Level_1":
                ChangeMusic(SoundType.Music_King);
                break;

            default:
                ChangeMusic(SoundType.Music_General);
                break;
        }
    }

    // 🔊 Play one-shot SFX
    public void Play(SoundType type)
    {
        if (!_soundDictionary.TryGetValue(type, out Sound s))
        {
            Debug.LogWarning($"Sound type {type} not found!");
            return;
        }

        GameObject soundObj = new GameObject($"Sound_{type}");
        AudioSource audioSrc = soundObj.AddComponent<AudioSource>();

        audioSrc.clip = s.Clip;
        audioSrc.volume = s.Volume;
        audioSrc.spatialBlend = 0f; // IMPORTANT: makes it 2D sound

        audioSrc.Play();

        Destroy(soundObj, s.Clip.length);
    }

    // 🎵 Music system
    public void ChangeMusic(SoundType type)
    {
        if (!_soundDictionary.TryGetValue(type, out Sound track))
        {
            Debug.LogWarning($"Music track {type} not found!");
            return;
        }

        if (_musicSource == null)
        {
            GameObject container = new GameObject("MusicSource");
            _musicSource = container.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.spatialBlend = 0f;
        }

        if (_musicSource.clip == track.Clip)
            return; // prevents restarting same music

        _musicSource.clip = track.Clip;
        _musicSource.volume = track.Volume;
        _musicSource.Play();
    }
}