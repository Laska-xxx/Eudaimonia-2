using Features.Player.Interact.Blowing;
using System.Collections;
using UnityEngine;

namespace Features.Player.Audio
{
    public class BlowingAudio : MonoBehaviour
    {
        [SerializeField] private BlowingController blowingController;

        [Header("Audio Data")]
        [SerializeField] private AudioDataSO equipSound;
        [SerializeField] private AudioDataSO unEquipSound;
        [SerializeField] private AudioDataSO blowingSound;
        
        [SerializeField] private float betweenTime = 0.1f;

        private AudioSource _audioSource;
        private Coroutine _blowingCoroutine;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            blowingController.OnEquip += PlayOnEquip;
            blowingController.OnUnEquip += PlayOnUnequip;
            blowingController.OnBlowingStarted += StartBlowing;
            blowingController.OnBlowingFinished += StopBlowing;
        }

        private void OnDisable()
        {
            blowingController.OnEquip -= PlayOnEquip;
            blowingController.OnUnEquip -= PlayOnUnequip;
            blowingController.OnBlowingStarted -= StartBlowing;
            blowingController.OnBlowingFinished -= StopBlowing;

            StopBlowing();
        }

        private void PlayOnEquip()
        {
            equipSound.Play(_audioSource);
        }

        private void PlayOnUnequip()
        {
            unEquipSound.Play(_audioSource);
        }

        private void StartBlowing()
        {
            if (_blowingCoroutine != null)
                StopCoroutine(_blowingCoroutine);

            _blowingCoroutine = StartCoroutine(BlabberRoutine());
        }

        private IEnumerator BlabberRoutine()
        {
            while (true)
            {
                if (blowingSound != null)
                {
                    blowingSound.Play(_audioSource);
                }
                yield return new WaitForSeconds(betweenTime);
            }
        }

        private void StopBlowing()
        {
            if (_blowingCoroutine != null)
            {
                StopCoroutine(_blowingCoroutine);
                _blowingCoroutine = null;
            }

            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }
    }
}

namespace Features.Audio
{
}