using Core;
using Features.Interactable.Environment;
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

        public event Action OnNoteClosed;

        private InputManager _inputManager;
        private ActionMapType _curActionType;

        [Inject]
        private void Init(InputManager inputManager)
        {
            _inputManager = inputManager;

            _inputManager.GameInput.Note.CloseNote.performed += CloseNote;
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
    }
}