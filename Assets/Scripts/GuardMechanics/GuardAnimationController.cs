using UnityEngine;
using System.Collections;

public class GuardAnimationController : MonoBehaviour
{
    [Tooltip("Assign the Animator on the guard's model.")]
    public Animator animator;

    [Tooltip("How long after the death animation finishes before the guard is destroyed (0.2s as requested).")]
    public float destroyDelayAfterDeath = 0.2f;

    // Cached parameter hashes
    private int hashIsWalking;
    private int hashIsAttacking;
    private int hashIsDying;

    // Track whether death has been triggered so there's no double-call
    private bool isDead = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        hashIsWalking = Animator.StringToHash("isWalking");
        hashIsAttacking = Animator.StringToHash("isAttacking");
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
        animator.SetBool(hashIsAttacking, attacking);

        //Added in sound here! Is not working, I have audio manager vol set to 0 bc is was looping
        Debug.Log("Played Sound");
        AudioManager.Instance.Play(AudioManager.SoundType.GuardSwing);

        // When attacking, guard is not walking
        if (attacking)
            animator.SetBool(hashIsWalking, false);
    }

    public void SetIdle()
    {
        if (animator == null || isDead) return;
        animator.SetBool(hashIsWalking, false);
        animator.SetBool(hashIsAttacking, false);
    }

    public void PlayDeath()
    {
        if (animator == null || isDead) return;

        isDead = true;

        // Clear other states
        animator.SetBool(hashIsWalking, false);
        animator.SetBool(hashIsAttacking, false);
        animator.SetBool(hashIsDying, true);

        StartCoroutine(WaitForDeathAndDestroy());
    }

    private IEnumerator WaitForDeathAndDestroy()
    {
        // Wait until the animator enters the death state
        bool enteredDeath = false;
        float timeout = 6f; // safety — death animation is 4.18s
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

            if (info.IsName("death"))
                enteredDeath = true;

            // Wait until death animation has fully played through
            if (enteredDeath && info.IsName("death") && info.normalizedTime >= 0.90f)
                break;

            // Safety exit if state changed away unexpectedly
            if (enteredDeath && !info.IsName("death"))
                break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Small delay after guard hits the floor before disappearing
        yield return new WaitForSeconds(destroyDelayAfterDeath);

        Destroy(gameObject);
    }
}
