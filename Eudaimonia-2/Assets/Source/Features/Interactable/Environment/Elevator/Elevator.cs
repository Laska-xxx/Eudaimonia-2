using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace Features.Interactable.Environment.Elevator
{
    public class Elevator : MonoBehaviour
    {
        [Header("Elevator Settings")]
        [SerializeField] private Transform platform;
        [SerializeField] private Transform targetPos;
        [SerializeField] private float moveDuration = 7f;
        [SerializeField] private float waitTime = 5f;

        private Vector3 _startLocalPos;
        private Vector3 _targetLocalPos;
        private CancellationTokenSource _cts;
        private Tween _moveTween;

        private void Start()
        {
            if (platform != null)
            {
                _startLocalPos = platform.localPosition;
                _targetLocalPos = targetPos.localPosition;
                _cts = new CancellationTokenSource();

                ElevatorRoutine(_cts.Token).Forget();
            }
        }

        private void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            if (_moveTween != null && _moveTween.IsActive())
            {
                _moveTween.Kill();
            }
        }

        private async UniTaskVoid ElevatorRoutine(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await UniTask.Delay(System.TimeSpan.FromSeconds(waitTime), cancellationToken: token);

                    _moveTween = platform.DOLocalMove(_targetLocalPos, moveDuration).SetEase(Ease.InOutSine);
                    await _moveTween.ToUniTask(cancellationToken: token);

                    await UniTask.Delay(System.TimeSpan.FromSeconds(waitTime), cancellationToken: token);

                    _moveTween = platform.DOLocalMove(_startLocalPos, moveDuration).SetEase(Ease.InOutSine);
                    await _moveTween.ToUniTask(cancellationToken: token);
                }
            }
            catch (System.OperationCanceledException)
            {
            }
        }
    }
}