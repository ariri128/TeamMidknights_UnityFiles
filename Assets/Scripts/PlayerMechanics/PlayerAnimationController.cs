using UnityEngine;
using System.Collections;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;

    public float oneAttackDuration = 0.25f;
    public float rapidAttackDuration = 0.35f;
    public float mediumAttackDuration = 0.8f;
    public float largeAttackDuration = 1.1f;
    public float reverseTimeDuration = 0.3f;
    public float pauseTimeDuration = 1.1f;
    public float dyingDuration = 2.3f;

    private int regularAttackCount;
    private bool isBusy;
    private bool isJumping;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void SetRunning(bool value)
    {
        if (animator == null)
        {
            return;
        }

        if (isBusy || isJumping)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        animator.SetBool("isRunning", value);
    }

    public void PlayJump()
    {
        if (animator == null || isBusy)
        {
            return;
        }

        isJumping = true;
        animator.SetTrigger("Jump");
        animator.SetBool("isRunning", false);
    }

    public void EndJump()
    {
        isJumping = false;
    }

    public bool CanUseLargeAttack()
    {
        return !isBusy;
    }

    public void PlayRegularAttack()
    {
        if (animator == null || isBusy)
        {
            return;
        }

        regularAttackCount++;

        if (regularAttackCount % 6 == 0)
        {
            StartCoroutine(PlayBoolAnimation("isAtkMedi", mediumAttackDuration));
        }
        else
        {
            StartCoroutine(PlayBoolAnimation("isAtkRapid", rapidAttackDuration));
        }
    }

    public void PlayLargeAttack()
    {
        if (animator == null || isBusy)
        {
            return;
        }

        StartCoroutine(PlayBoolAnimation("isAtkLrg", largeAttackDuration));
    }

    public void PlayReverseTime()
    {
        if (animator == null)
        {
            return;
        }

        StartCoroutine(PlayBoolAnimation("isReverse", reverseTimeDuration));
    }

    public void PlayPauseTime()
    {
        if (animator == null || isBusy)
        {
            return;
        }

        StartCoroutine(PlayBoolAnimation("isPauseTime", pauseTimeDuration));
    }

    public void PlayDying()
    {
        if (animator == null)
        {
            return;
        }

        StopAllCoroutines();
        isBusy = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isDying", true);
    }

    private IEnumerator PlayBoolAnimation(string parameterName, float duration)
    {
        isBusy = true;
        animator.SetBool("isRunning", false);
        animator.SetBool(parameterName, true);

        yield return new WaitForSeconds(duration);

        animator.SetBool(parameterName, false);
        isBusy = false;
    }
}
