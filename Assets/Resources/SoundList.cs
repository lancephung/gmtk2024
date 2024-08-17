using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct SoundDescriptor
{
    public string Name;
    public AudioClip Clip;

    [Min(0.0f)]
    public float Pitch, PitchVariation;
    [Range(0.0f, 1.0f)]
    public float Volume;
}

[CreateAssetMenu(fileName = "Sound List", menuName = "ScriptableObjects/SoundList", order = 1)]
[ExecuteInEditMode]
public class SoundList : ScriptableObject
{
    public List<SoundDescriptor> AudioClips = new(){};

    #if UNITY_EDITOR
    void OnValidate()
    {
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] {"Assets/Resources/SFX"});
        foreach (string guid in guids)
        {
            AudioClip clip = (AudioClip) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(AudioClip));

            int index = AudioClips.FindIndex((descriptor) => descriptor.Clip == clip);
            if (index != -1)
            {
                var descriptor = AudioClips[index];
                descriptor.Name = clip.name;
                AudioClips[index] = descriptor;
                continue;
            }

            AudioClips.Add(new SoundDescriptor(){Clip = clip, Pitch = 1, Volume = 1, Name = clip.name});
        }
    }
    #endif
}