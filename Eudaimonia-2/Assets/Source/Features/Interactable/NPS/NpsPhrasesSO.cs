using System.Collections.Generic;
using UnityEngine;

namespace Features.Interactable.NPS
{
    [CreateAssetMenu(fileName = "NpsPhrasesData", menuName = "Nps/NpsPhrases")]
    public class NpsPhrasesSO : ScriptableObject
    {
        [field: SerializeField] public string NpsName { get; private set; }
        [field: SerializeField] public List<string> NpsPhrases { get; private set; }
        [field: SerializeField] public string EndPhrase { get; private set; }
    }
}