using Zenject;

namespace Features.Player.Interact.Blowing
{
    public class PlayerInventory
    {
        public int SoapBubblesCount { get; private set; }

        private readonly SignalBus _signalBus;


        public PlayerInventory(SignalBus signalBus)
        {
            SoapBubblesCount = 3;
            _signalBus = signalBus;
        }

        public void AddCigarette()
        {
            SoapBubblesCount++;
            _signalBus.Fire(new SoapBubblesCountChangedSignal { NewCount = SoapBubblesCount });
        }

        public void GetCigarette()
        {
            SoapBubblesCount--;
            _signalBus.Fire(new SoapBubblesCountChangedSignal { NewCount = SoapBubblesCount });
        }

        public bool TryConsumeCigarette()
        {
            if (SoapBubblesCount > 0)
            {
                return true;
            }

            return false;
        }
    }
}