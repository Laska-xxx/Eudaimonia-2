using UnityEngine;

public class CigaretteAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void StartSmokingAnim()
    {
        animator.SetTrigger("Start Smok");
    }
}
