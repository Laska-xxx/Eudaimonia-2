using UnityEngine;

namespace Features.Interactable.Environment
{
    [CreateAssetMenu(fileName = "NewNoteData", menuName = "Interactable/NoteDataSO")]
    public class NoteDataSO : ScriptableObject
    {
        [SerializeField] private Sprite _noteImage;

        public Sprite NoteImage => _noteImage;
    }
}