using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace Features.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _phraseText;
        [SerializeField] private float _phraseDuration = 10f;

        public bool IsActive { get; private set; }

        private TextAnimator _textAnimator;
        private Coroutine _waitBeforeCoroutine;

        private List<string> _currentDialogue;
        private int _currentPhraseIndex;
        private Action _onDialogueComplete;
        private bool _isDialogueMode;

        public event Action<string> OnTypingStarted;
        public event Action<string> OnTypingFinished;

        [Inject]
        private void Init(TextAnimator textAnimator)
        {
            _textAnimator = textAnimator;
        }

        private void Awake()
        {
            Close();
        }

        public void ShowPhrase(string name, string phrase)
        {
            IsActive = true;
            _isDialogueMode = false;
            StopAllCoroutines();

            _dialoguePanel.SetActive(true);
            _nameText.text = name;

            OnTypingStarted?.Invoke(name);
            _textAnimator.StartTyping(phrase, _phraseText, OnPhraseTyped);
        }
        
        public void ShowDialogue(string name, List<string> dialogue, Action onComplete)
        {
            IsActive = true;
            _isDialogueMode = true;
            _currentDialogue = dialogue;
            _onDialogueComplete = onComplete;
            _currentPhraseIndex = 0;

            _dialoguePanel.SetActive(true);
            _nameText.text = name;

            PlayCurrentPhrase();
        }
        
        private void PlayCurrentPhrase()
        {
            if (_waitBeforeCoroutine != null) StopCoroutine(_waitBeforeCoroutine);

            string phrase = _currentDialogue[_currentPhraseIndex];

            OnTypingStarted?.Invoke(_nameText.text);
            _textAnimator.StartTyping(phrase, _phraseText, OnPhraseTyped);
        }
        
        private void OnPhraseTyped()
        {
            OnTypingFinished?.Invoke(_nameText.text);

            if (_isDialogueMode)
            {
                _waitBeforeCoroutine = StartCoroutine(WaitBefore(AdvanceDialogue));
                return;
            }

            _waitBeforeCoroutine = StartCoroutine(WaitBefore(Close));
        }

        private IEnumerator WaitBefore(Action action)
        {
            yield return new WaitForSeconds(_phraseDuration);
            action?.Invoke();
        }
        
        public void TryAdvance()
        {
            if (_textAnimator.IsTyping) return;

            if (_isDialogueMode)
            {
                AdvanceDialogue();
                return;
            }

            Close();
        }
        
        private void AdvanceDialogue()
        {
            if (_waitBeforeCoroutine != null) StopCoroutine(_waitBeforeCoroutine);

            _currentPhraseIndex++;
            if (_currentPhraseIndex >= _currentDialogue.Count)
            {
                Close();
                _onDialogueComplete?.Invoke();
            }
            else
            {
                PlayCurrentPhrase();
            }
        }
        
        public void Close()
        {
            if (_nameText != null && !string.IsNullOrEmpty(_nameText.text))
            {
                OnTypingFinished?.Invoke(_nameText.text);
            }

            IsActive = false;

            StopAllCoroutines();
            if (_textAnimator != null) _textAnimator.StopTyping();
            _dialoguePanel.SetActive(false);
            _isDialogueMode = false;
        }
    }
}