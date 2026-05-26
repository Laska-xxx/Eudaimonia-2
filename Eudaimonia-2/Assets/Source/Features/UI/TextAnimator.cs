using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Features.UI
{
    public class TextAnimator : MonoBehaviour
    {
        [SerializeField] private float _timePerLetter = 0.05f;

        private Coroutine _typingCoroutine;
        private Action _onCompleteCallback;

        public bool IsTyping { get; private set; }

        public void StartTyping(string text, TextMeshProUGUI textComponent, Action onComplete = null)
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);

            _onCompleteCallback = onComplete;
            IsTyping = true;
            textComponent.text = "";

            _typingCoroutine = StartCoroutine(TypeRoutine(text, textComponent));
        }

        public void StopTyping()
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            IsTyping = false;
        }

        private IEnumerator TypeRoutine(string text, TextMeshProUGUI textComponent)
        {
            for (int i = 0; i <= text.Length; i++)
            {
                textComponent.text = text.Substring(0, i);
                yield return new WaitForSeconds(_timePerLetter);
            }

            IsTyping = false;
            _onCompleteCallback?.Invoke();
        }
    }
}