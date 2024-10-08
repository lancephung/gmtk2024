using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static GameObject prefab;
    static Dictionary<string, SoundDescriptor> descriptors = new();
    
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;

        LoadResources();
    }

    public static void LoadResources()
    {
        prefab = Resources.Load<GameObject>("Sound Emitter");
        foreach (SoundDescriptor sound in Resources.Load<SoundList>("Sound List").AudioClips)
        {
            descriptors[sound.Clip.name] = sound;
        }
    }

    public static SoundBehavior PlaySound(string name)
    {
        GameObject emitter = GameObject.Instantiate(prefab);
        AudioSource audio = emitter.GetComponent<AudioSource>();

        SoundDescriptor descriptor = descriptors[name];

        audio.clip = descriptor.Clip;
        audio.volume = descriptor.Volume * GameManager.volume;
        audio.pitch = descriptor.Pitch + descriptor.PitchVariation * (Random.value - 0.5f);
        // audio.outputAudioMixerGroup = Resources.Load<AudioMixer>("Audio Mixer").outputAudioMixerGroup;

        audio.Play();

        return emitter.GetComponent<SoundBehavior>();
    }
}
