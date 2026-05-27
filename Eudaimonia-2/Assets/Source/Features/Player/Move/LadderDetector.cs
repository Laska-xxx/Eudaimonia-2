using UnityEngine;

namespace Features.Player.Move
{
    public class LadderDetector : MonoBehaviour
    {
        [SerializeField] private PlayerViewProvider viewProvider;
        [SerializeField] private LayerMask ladderLayer;

        public bool IsNearLadder { get; private set; }

        private Ladder _curLadder;

        public bool CheckLookingAtLadder()
        {
            if (_curLadder == null) return false;

            Vector3 lookXZ = new Vector3(viewProvider.LookDirection.x, 0f, viewProvider.LookDirection.z);
            Vector3 ladderCenter = _curLadder.GetComponent<Collider>().bounds.center;
            Vector3 toLadderXZ = new Vector3(ladderCenter.x - transform.position.x, 0f, ladderCenter.z - transform.position.z);

            bool isLooking = false;

            if (lookXZ.sqrMagnitude > 0.001f && toLadderXZ.sqrMagnitude > 0.001f)
            {
                float dot = Vector3.Dot(lookXZ.normalized, toLadderXZ.normalized);
                isLooking = dot > 0.2f;
            }
            else
            {
                isLooking = true;
            }

            if (!isLooking && viewProvider.IsLookingAtLayer(ladderLayer))
            {
                isLooking = true;
            }

            return isLooking;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Ladder>(out _curLadder))
            {
                IsNearLadder = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Ladder>(out Ladder ladder))
            {
                if (ladder == _curLadder)
                {
                    _curLadder = null;
                    IsNearLadder = false;
                }
            }
        }
    }
}