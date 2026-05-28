using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/AudioData")]
public class AudioDataSO : ScriptableObject
{
    [field:SerializeField] public AudioClip[] Сlips {  get; private set; }
    [field: SerializeField][Range(0f, 1f)] public float Volume { get; private set; } = 1f;

    [Header("Pitch Settings")]
    [field: SerializeField] public bool UseRandomPitch { get; private set; } = false;
    [field: SerializeField][Range(0.1f, 3f)] public float MinPitch { get; private set; } = 0.9f;
    [field: SerializeField][Range(0.1f, 3f)] public float MaxPitch { get; private set; } = 1.1f;

    public float Play(AudioSource source)
    {
        if (Сlips == null || Сlips.Length == 0) return 0;

        var clip = Сlips[Random.Range(0, Сlips.Length)];
        source.clip = clip;
        source.volume = Volume;
        source.pitch = UseRandomPitch ? Random.Range(MinPitch, MaxPitch) : 1f;
        source.Play();

        return clip.length;
    }

    public void PlayOneShot(AudioSource source)
    {
        if (Сlips == null || Сlips.Length == 0) return;

        var clip = Сlips[Random.Range(0, Сlips.Length)];
        source.volume = Volume;
        source.pitch = UseRandomPitch ? Random.Range(MinPitch, MaxPitch) : 1f;
        source.PlayOneShot(clip);
    }
}
