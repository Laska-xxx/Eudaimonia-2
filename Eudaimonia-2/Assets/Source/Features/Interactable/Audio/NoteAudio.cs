using Features.Interactable.Environment.Note;
using Features.UI;
using UnityEngine;
using Zenject;

public class NoteAudio : MonoBehaviour
{
    [SerializeField] private AudioDataSO openSound;
    [SerializeField] private AudioDataSO closeSound;

    private NoteItem _noteItem;
    private AudioSource _source;
    private NoteUI _noteUI;
    private bool _isCurrentlyReadingThis = false;

    [Inject]
    public void Init(NoteUI noteUI)
    {
        _noteUI = noteUI;
    }

    private void Awake()
    {
        _noteItem = GetComponent<NoteItem>();
        _source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _noteItem.OnOpened += OnOpened;
        if (_noteUI != null) _noteUI.OnNoteClosed += OnClosed;
    }

    private void OnDisable()
    {
        _noteItem.OnOpened -= OnOpened;
        if (_noteUI != null) _noteUI.OnNoteClosed -= OnClosed;
    }

    private void OnOpened()
    {
        _isCurrentlyReadingThis = true;
        openSound?.Play(_source);
    }

    private void OnClosed()
    {
        if (_isCurrentlyReadingThis)
        {
            closeSound?.Play(_source);
            _isCurrentlyReadingThis = false;
        }
    }
}
