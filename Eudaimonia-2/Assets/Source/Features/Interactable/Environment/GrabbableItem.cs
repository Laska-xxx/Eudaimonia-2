using Features.Player.Interact;
using System;
using UnityEngine;

namespace Features.Interactable.Environment
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrabbableItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string hintText = "Взять";
        public string HintText => hintText;
        public Rigidbody Rb { get; private set; }

        public event Action OnGrabbed;
        public event Action OnDropped;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody>();
        }

        public void Interact(PlayerInteractor player)
        {
            player.Grabber.GrabItem(this);
            OnGrabbed?.Invoke();
        }

        public void ReleaseItem()
        {
            OnDropped?.Invoke();
        }
    }
}