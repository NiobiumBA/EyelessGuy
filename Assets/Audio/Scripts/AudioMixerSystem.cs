using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerSystem : MonoBehaviour
{
    public enum AudioGroup
    {
        Master, GUI, Music, Entity, GameLogic
    }

    public static AudioMixer Mixer => s_instance.m_mixer;

    private static AudioMixerSystem s_instance;

    [SerializeField] private AudioMixer m_mixer;
    [SerializeField] private float m_minVolumeDb = -80;

    /// <summary>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="volume">
    ///     in range [0..1]
    /// </param>
    public static void SetVolume(AudioGroup type, float volume)
    {
        float normalizedVolume = Mathf.Lerp(s_instance.m_minVolumeDb, 0, volume);

        Mixer.SetFloat(type.ToString(), normalizedVolume);
    }

    public static float GetVolume(AudioGroup type)
    {
        Mixer.GetFloat(type.ToString(), out float result);

        return result / s_instance.m_minVolumeDb;
    }

    private void Awake()
    {
        if (FindObjectsOfType<AudioMixerSystem>().Length > 1)
            print($"Too many {nameof(AudioMixerSystem)}'s");

        s_instance = this;
    }
}
