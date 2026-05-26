using Features.Player.Interact;

namespace Features.Interactable
{
    public interface IInteractable
    {
        public string HintText { get; }

        public void Interact(PlayerInteractor player);
    }
}