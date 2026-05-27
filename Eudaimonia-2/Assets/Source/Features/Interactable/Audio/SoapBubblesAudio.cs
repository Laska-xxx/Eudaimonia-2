using Features.Interactable.Environment;
using UnityEngine;

public class SoapBubblesAudio : MonoBehaviour
{
    [SerializeField] private AudioDataSO pickupSound;

    private SoapBubblesItem _bubble;
    private AudioSource _audioSource;

    private void Awake()
    {
        _bubble = GetComponent<SoapBubblesItem>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _bubble.OnPickedUp += PlayPickup;
    }
    private void OnDisable()
    {
        _bubble.OnPickedUp -= PlayPickup;
    }

    private void PlayPickup()
    {
        AudioSource.PlayClipAtPoint(pickupSound.clips[Random.Range(0, pickupSound.clips.Length)], transform.position, pickupSound.volume);
    }
}
