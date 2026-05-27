using Features.Player.Interact;
using UnityEngine;

namespace Features.Interactable.Environment
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrabbableItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string hintText = "Взять на [E]";
        public string HintText => hintText;
        public Rigidbody Rb { get; private set; }

        private void Awake()
        {
            Rb = GetComponent<Rigidbody>();
        }

        public void Interact(PlayerInteractor player)
        {
            player.Grabber.GrabItem(this);
        }
    }
}