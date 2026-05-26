using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using Features.Player.Interact.Smoking;

namespace Features.UI
{
    public class PlayerEffectsView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coughText;
        [SerializeField] private List<string> _coughPhrases;

        private SignalBus _signalBus;

        [Inject]
        private void Init(SignalBus signalBus)
        {
            _signalBus = signalBus;

            _signalBus.Subscribe<CoughFromSmokingSignal>(PlayCoughEffect);
        }

        private void OnDisable()
        {
            if (_signalBus != null)
            {
                _signalBus.TryUnsubscribe<CoughFromSmokingSignal>(PlayCoughEffect);
            }
        }

        private void Start()
        {
            coughText.gameObject.SetActive(false);
        }

        public void PlayCoughEffect(CoughFromSmokingSignal signal)
        {
            Debug.Log("Smoking very long");
            StartCoroutine(CoughRoutine());
        }

        private IEnumerator CoughRoutine()
        {
            coughText.text = _coughPhrases[Random.Range(0, _coughPhrases.Count)];
            coughText.gameObject.SetActive(true);

            yield return new WaitForSeconds(2);
            coughText.gameObject.SetActive(false);
        }
    }
}