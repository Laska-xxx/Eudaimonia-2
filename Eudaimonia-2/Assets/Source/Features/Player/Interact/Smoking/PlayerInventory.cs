using Zenject;

namespace Features.Player.Interact.Smoking
{
    public class PlayerInventory
    {
        public int CigarettesCount { get; private set; }

        private readonly SignalBus _signalBus;


        public PlayerInventory(SignalBus signalBus)
        {
            CigarettesCount = 3;
            _signalBus = signalBus;
        }

        public void AddCigarette()
        {
            CigarettesCount++;
            _signalBus.Fire(new CigarettesCountChangedSignal { NewCount = CigarettesCount });
        }

        public void GetCigarette()
        {
            CigarettesCount--;
            _signalBus.Fire(new CigarettesCountChangedSignal { NewCount = CigarettesCount });
        }

        public bool TryConsumeCigarette()
        {
            if (CigarettesCount > 0)
            {
                return true;
            }

            return false;
        }
    }
}