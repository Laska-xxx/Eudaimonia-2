using Features.Player.Interact;
using UnityEngine;

namespace Features.Interactable.Environment
{
    public class CigaretteItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string hintText = "Подобрать на [E]";
        public string HintText => hintText;

        public void Interact(PlayerInteractor player)
        {
            player.Inventory.AddCigarette();
            // В идеале тут звук подбора
            Destroy(gameObject);
        }
    }
}