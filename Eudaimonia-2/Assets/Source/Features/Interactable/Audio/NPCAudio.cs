using Features.Interactable.NPS;
using Features.UI;
using System.Collections;
using UnityEngine;
using Zenject;

public class NPCAudio : MonoBehaviour
{
    [SerializeField] private AudioDataSO blabberSound;
    [SerializeField] private float betweenTime = 0.12f;

    private AudioSource _audioSource;
    private DialogueUI _dialogueUI;
    private string _npcName;
    private Coroutine _blabberCoroutine;

    [Inject]
    public void Init(DialogueUI dialogueUI)
    {
        _dialogueUI = dialogueUI;
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (TryGetComponent(out NPCInteractable nps))
            _npcName = nps.NpsData.NpsName;
    }

    private void OnEnable()
    {
        if (_dialogueUI != null)
        {
            _dialogueUI.OnTypingStarted += HandleTypingStarted;
            _dialogueUI.OnTypingFinished += HandleTypingFinished;
        }
    }

    private void OnDisable()
    {
        if (_dialogueUI != null)
        {
            _dialogueUI.OnTypingStarted -= HandleTypingStarted;
            _dialogueUI.OnTypingFinished -= HandleTypingFinished;
        }
        StopBlabbering();
    }

    private void HandleTypingStarted(string speakerName)
    {
        // Запускаем звук только если говорит этот конкретный НПС
        if (speakerName != _npcName) return;

        if (_blabberCoroutine != null) StopCoroutine(_blabberCoroutine);
        _blabberCoroutine = StartCoroutine(BlabberRoutine());
    }

    private void HandleTypingFinished(string speakerName)
    {
        if (speakerName != _npcName) return;
        StopBlabbering();
    }

    private IEnumerator BlabberRoutine()
    {
        while (true)
        {
            if (blabberSound != null)
            {
                blabberSound.Play(_audioSource);
            }
            yield return new WaitForSeconds(betweenTime);
        }
    }

    private void StopBlabbering()
    {
        if (_blabberCoroutine != null)
        {
            StopCoroutine(_blabberCoroutine);
            _blabberCoroutine = null;
        }

        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }
}
