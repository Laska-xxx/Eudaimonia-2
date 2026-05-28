using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Audio
{
    public class BgMusicController : MonoBehaviour
    {
        [Header("Playlist Settings")]
        [SerializeField] private List<AudioDataSO> playlist;
        [SerializeField] private float fadeDuration = 2f;

        [Header ("Audio Sourses")]
        [SerializeField] private AudioSource _sourceA;
        [SerializeField] private AudioSource _sourceB;

        private bool _useSourceA = true;

        private CancellationTokenSource _cts;

        private void Start()
        {
            _cts = new CancellationTokenSource();
            PlayPlaylist(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            _sourceA.DOKill();
            _sourceB.DOKill();
        }

        private async UniTaskVoid PlayPlaylist(CancellationToken token)
        {
            if (playlist == null || playlist.Count == 0) return;

            while (!token.IsCancellationRequested)
            {
                AudioDataSO currentTrack = playlist[Random.Range(0, playlist.Count)];

                AudioSource activeSource = _useSourceA ? _sourceA : _sourceB;
                AudioSource fadingSource = _useSourceA ? _sourceB : _sourceA;

                activeSource.volume = 0f;
                float trackLength = currentTrack.Play(activeSource);

                activeSource.DOFade(currentTrack.Volume, fadeDuration).SetEase(Ease.InOutSine);
                fadingSource.DOFade(0f, fadeDuration).SetEase(Ease.InOutSine);

                _useSourceA = !_useSourceA;
                float waitTime = trackLength - fadeDuration;

                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);

                fadingSource.Stop();
            }
        }
    }
}