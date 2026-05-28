using Features.Player.Interact;
using System;
using UnityEngine;

namespace Features.Interactable.Environment
{
    public class SoapBubblesItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string hintText = "Подобрать";
        public string HintText => hintText;

        public event Action OnPickedUp;

        public void Interact(PlayerInteractor player)
        {
            OnPickedUp?.Invoke();

            player.Inventory.AddCigarette();
            Destroy(gameObject);
        }
    }
}