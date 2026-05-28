using UnityEngine;
using Zenject;

namespace Features.Tutorial
{
    internal class TutorialAudio : MonoBehaviour
    {
        [SerializeField] private AudioDataSO taskComplete;

        private TutorialManager _tutorialManager;
        private AudioSource _audioSource;

        [Inject]
        private void Init(TutorialManager tutorialManager)
        {
            _tutorialManager = tutorialManager;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _tutorialManager.OnTaskCompleted += OnTaskCompletePlay;
        }

        private void OnDisable()
        {
            _tutorialManager.OnTaskCompleted -= OnTaskCompletePlay;
        }

        private void OnTaskCompletePlay()
        {
            taskComplete.Play(_audioSource);
        }
    }
}
