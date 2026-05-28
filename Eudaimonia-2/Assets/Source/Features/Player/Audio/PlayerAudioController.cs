using Cysharp.Threading.Tasks;
using Features.Player.Move;
using Features.Player.Move.MoveStates;
using System;
using System.Threading;
using UnityEngine;

namespace Features.Player.Audio
{
    public class PlayerAudioController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private AudioSource _audioSource;

        [Header("Audio Data")]
        [SerializeField] private AudioDataSO _footstepsWalkData;
        [SerializeField] private AudioDataSO _footstepsSprintData;
        [SerializeField] private AudioDataSO _jumpData;
        [SerializeField] private AudioDataSO _landData;
        [SerializeField] private AudioDataSO _climbData;

        [Header("Footstep Settings")]
        [SerializeField] private float _walkStepInterval = 0.5f;
        [SerializeField] private float _sprintStepInterval = 0.3f;
        [SerializeField] private float _squatStepInterval = 0.7f;
        [SerializeField] private float climbStepInterval = 0.7f;

        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _playerMovement.OnJumped += PlayJumpSound;
            _playerMovement.OnLanded += PlayLandSound;
            _cts = new CancellationTokenSource();
            FootstepLoopAsync(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            _playerMovement.OnJumped -= PlayJumpSound;
            _playerMovement.OnLanded -= PlayLandSound;

            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        private async UniTaskVoid FootstepLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_playerMovement.IsMoving)
                {
                    float interval = PlayFootstepAndGetInterval(_playerMovement.CurrentState);

                    bool isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: token).SuppressCancellationThrow();

                    if (isCancelled) return;
                }
                else
                {
                    await UniTask.Yield(token);
                }
            }
        }

        private float PlayFootstepAndGetInterval(MovementStateEnum state)
        {
            switch (state)
            {
                case MovementStateEnum.Walking:
                    _footstepsWalkData.Play(_audioSource);
                    return _walkStepInterval;

                case MovementStateEnum.Sprinting:
                    _footstepsSprintData.Play(_audioSource);
                    return _sprintStepInterval;

                case MovementStateEnum.Squatting:
                    _footstepsWalkData.Play(_audioSource);
                    return _squatStepInterval;

                case MovementStateEnum.Climbing:
                    _climbData.Play(_audioSource);
                    return climbStepInterval;

                default:
                    return 0.1f;
            }
        }

        private void PlayJumpSound()
        {
            _jumpData.Play(_audioSource);
        }

        private void PlayLandSound()
        {
            _landData.Play(_audioSource);
        }
    }
}