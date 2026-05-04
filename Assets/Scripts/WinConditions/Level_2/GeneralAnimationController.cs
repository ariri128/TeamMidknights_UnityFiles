using UnityEngine;
using System.Collections;

public class GeneralAnimationController : MonoBehaviour
{
    [Tooltip("Assign the Animator on the General's model.")]
    public Animator animator;

    private int hashIsWalking;
    private int hashIsAttacking;
    private int hashIsDying;

    private bool isDead = false;
    private bool isAttacking = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        hashIsWalking = Animator.StringToHash("isWalking");
        hashIsAttacking = Animator.StringToHash("isAttackin");
        hashIsDying = Animator.StringToHash("isDying");
    }

    public void SetWalking(bool walking)
    {
        if (animator == null || isDead) return;
        animator.SetBool(hashIsWalking, walking);
    }

    public void SetAttacking(bool attacking)
    {
        if (animator == null || isDead) return;

        Debug.Log("GeneralAnim: SetAttacking = " + attacking);

        isAttacking = attacking;
        animator.SetBool(hashIsAttacking, attacking);

        // When attacking, not walking
        if (attacking)
            animator.SetBool(hashIsWalking, false);
    }

    public void PlayDeath()
    {
        if (animator == null || isDead) return;

        isDead = true;

        animator.SetBool(hashIsWalking, false);
        animator.SetBool(hashIsAttacking, false);
        animator.SetBool(hashIsDying, true);
    }

    public bool IsDeathComplete()
    {
        if (animator == null) return true;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName("Death") && info.normalizedTime >= 0.95f;
    }

    public void SetIdle()
    {
        if (animator == null || isDead) return;
        animator.SetBool(hashIsWalking, false);
        animator.SetBool(hashIsAttacking, false);
    }
}
