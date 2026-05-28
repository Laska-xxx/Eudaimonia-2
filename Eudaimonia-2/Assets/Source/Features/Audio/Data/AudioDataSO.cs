using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/AudioData")]
public class AudioDataSO : ScriptableObject
{
    public AudioClip[] clips;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float minPitch = 0.9f;
    [Range(0.1f, 3f)] public float maxPitch = 1.1f;

    public void Play(AudioSource source)
    {
        if (clips == null || clips.Length == 0) return;

        source.clip = clips[Random.Range(0, clips.Length)];
        source.volume = volume;
        source.pitch = Random.Range(minPitch, maxPitch);
        source.Play();
    }
}
