using Features.Interactable.Environment;
using System;
using UnityEngine;

namespace Features.Player
{
    public class PlayerGrabber : MonoBehaviour
    {
        [SerializeField] private Transform _holdPoint; 
        [SerializeField] private float _grabForce = 15f;
        [SerializeField] private float _damping = 10f;

        public event Action<GrabbableItem> OnItemGrabbed;
        public event Action<GrabbableItem> OnItemDropped;

        private GrabbableItem _heldItem;
        public bool IsHoldingItem => _heldItem != null;

        public void GrabItem(GrabbableItem item)
        {
            _heldItem = item;
            _heldItem.Rb.useGravity = false;
            _heldItem.Rb.linearDamping = _damping;
            _heldItem.Rb.angularDamping = _damping;

            _heldItem.Rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);

            OnItemGrabbed?.Invoke(_heldItem);
        }

        public void DropItem()
        {
            if (_heldItem == null) return;

            OnItemDropped?.Invoke(_heldItem);

            _heldItem.ReleaseItem();

            _heldItem.Rb.useGravity = true;
            _heldItem.Rb.linearDamping = 0f;
            _heldItem.Rb.angularDamping = 0.05f;
            _heldItem = null;
        }

        private void FixedUpdate()
        {
            if (_heldItem != null)
            {
                Vector3 directionToPoint = _holdPoint.position - _heldItem.transform.position;
                float distance = directionToPoint.magnitude;

                _heldItem.Rb.linearVelocity = directionToPoint * (_grabForce * distance);
            }
        }
    }
}