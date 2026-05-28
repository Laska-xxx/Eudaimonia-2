using Features.Interactable.Environment;
using UnityEngine;

public class GrabbableAudio : MonoBehaviour
{
    [SerializeField] private AudioDataSO grabSound;
    [SerializeField] private AudioDataSO dropSound;
    [SerializeField] private AudioDataSO collisionSound;
    [SerializeField] private float collisionVelocityThreshold = 1.5f;

    private GrabbableItem _grabbable;
    private AudioSource _audioSource;

    private void Awake()
    {
        _grabbable = GetComponent<GrabbableItem>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _grabbable.OnGrabbed += PlayGrab;
        _grabbable.OnDropped += PlayDrop;
    }

    private void OnDisable()
    {
        _grabbable.OnGrabbed -= PlayGrab;
        _grabbable.OnDropped -= PlayDrop;
    }

    private void PlayGrab()
    {
        grabSound?.Play(_audioSource);
    }

    private void PlayDrop()
    {
        dropSound?.Play(_audioSource);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= collisionVelocityThreshold)
        {
            collisionSound?.Play(_audioSource);
        }
    }
}
