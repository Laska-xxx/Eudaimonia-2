using Core;
using Features.Interactable.Environment.Note;
using Features.UI.DoTween;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Features.UI
{
    public class NoteUI : MonoBehaviour
    {
        public bool IsNoteOpen { get; private set; }

        [SerializeField] private UIPanel notePanel;
        [SerializeField] private Image noteImage;
        [SerializeField] private TextMeshProUGUI countNotesText;

        public event Action OnNoteClosed;

        private InputManager _inputManager;
        private ActionMapType _curActionType;
        private NoteManager _noteManager;

        [Inject]
        private void Init(InputManager inputManager, NoteManager noteManager)
        {
            _inputManager = inputManager;
            _noteManager = noteManager;

            _inputManager.GameInput.Note.CloseNote.performed += CloseNote;
            _noteManager.OnNoteCollected += UpdateNoteCount;
        }

        private void Awake()
        {
            notePanel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_inputManager.GameInput != null)
            {
                _inputManager.GameInput.Note.CloseNote.performed -= CloseNote;
            }

            _noteManager.OnNoteCollected -= UpdateNoteCount;
        }

        public void OpenNote(NoteDataSO noteData)
        {
            IsNoteOpen = true;
            noteImage.sprite = noteData.NoteImage;
            notePanel.Show();

            _curActionType = _inputManager.CurrentActionMapType;
            _inputManager.SwitchActionMapType(ActionMapType.Note);
        }

        public void CloseNote(InputAction.CallbackContext ctx)
        {
            IsNoteOpen = false;
            notePanel.Hide();

            OnNoteClosed?.Invoke();

            _inputManager.SwitchActionMapType(_curActionType);

            print(_inputManager.CurrentActionMapType);
        }

        private void UpdateNoteCount(int count)
        {
            countNotesText.text = count.ToString();
        }
    }
}