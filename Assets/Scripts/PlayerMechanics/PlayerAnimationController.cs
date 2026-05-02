using UnityEngine;
using System.Collections;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Animation Durations (seconds)")]
    [Tooltip("Duration of the first click attack (AttackMedium - 47 frames at 60fps ≈ 0.78s)")]
    public float mediumAttackDuration = 0.78f;

    [Tooltip("Duration of rapid attack loop (AttackRapid - 40 frames at 60fps ≈ 0.67s)")]
    public float rapidAttackDuration = 0.67f;

    [Tooltip("Duration of large water ball attack (AttackLarge - 66 frames at 60fps ≈ 1.1s). Script adds slight speed boost via animator.")]
    public float largeAttackDuration = 0.85f;

    [Tooltip("Duration of time slow entry animation (PauseTime - 64 frames at 60fps ≈ 1.07s)")]
    public float pauseTimeDuration = 1.07f;

    [Tooltip("How long to wait before teleporting on rewind so ReverseTime animation shows (17 frames ≈ 0.28s)")]
    public float reverseTimeAnimDelay = 0.28f;

    [Tooltip("Full duration of ReverseTime animation for state cleanup")]
    public float reverseTimeDuration = 0.28f;

    [Tooltip("Duration of Dying animation (138 frames at 60fps ≈ 2.3s)")]
    public float dyingDuration = 2.3f;

    [Header("Jump Settings")]
    [Tooltip("How many frames into the Jumping animation (52 frames) before the landing phase starts. ~70% through = frame 36.")]
    public float jumpLandingPhaseNormalized = 0.70f;

    [Header("Attack While Moving")]
    [Tooltip("If true, sets isRunning=true during attacks when the player is moving so legs animate.")]
    public bool runLegsWhileAttacking = true;

    // Animation state
    private bool isBusy = false;
    private bool isJumping = false;
    private bool isMoving = false;
    private bool isLargeAttackPlaying = false;
    private Coroutine largeAttackCoroutine = null;
    private int regularAttackCount = 0;

    // Cached hash IDs for performance
    private int hashIsRunning;
    private int hashIsDying;
    private int hashIsAtkMedi;
    private int hashIsAtkRapid;
    private int hashIsAtkLrg;
    private int hashIsReverse;
    private int hashIsPauseTime;
    private int hashJump;
    private int hashIsGrounded;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Cache parameter hashes
        hashIsRunning = Animator.StringToHash("isRunning");
        hashIsDying = Animator.StringToHash("isDying");
        hashIsAtkMedi = Animator.StringToHash("isAtkMedium");
        hashIsAtkRapid = Animator.StringToHash("isAtkRapid");
        hashIsAtkLrg = Animator.StringToHash("isAtkLrg");
        hashIsReverse = Animator.StringToHash("isReverseTime");
        hashIsPauseTime = Animator.StringToHash("isPauseTime");
        hashJump = Animator.StringToHash("Jump");
        hashIsGrounded = Animator.StringToHash("isGrounded");
    }

    public void SetRunning(bool value)
    {
        if (animator == null) return;

        isMoving = value;

        // While jumping, never override with running
        if (isJumping) return;

        // During attacks, if runLegsWhileAttacking is on and player is moving, keep legs running
        if (isBusy)
        {
            if (runLegsWhileAttacking && isMoving)
                animator.SetBool(hashIsRunning, true);
            else
                animator.SetBool(hashIsRunning, false);
            return;
        }

        animator.SetBool(hashIsRunning, value);
    }

    public void PlayJump()
    {
        if (animator == null || isBusy) return;

        isJumping = true;
        animator.SetBool(hashIsGrounded, false);
        animator.SetBool(hashIsRunning, false);
        animator.SetTrigger(hashJump);
    }

    public void SetGrounded(bool grounded)
    {
        if (animator == null) return;

        animator.SetBool(hashIsGrounded, grounded);

        if (grounded && isJumping)
        {
            isJumping = false;
            animator.SetBool(hashIsRunning, isMoving);
        }
    }

    public void PlayRegularAttack()
    {
        if (animator == null || isBusy || isJumping) return;

        regularAttackCount++;

        // First click should play AttackMedium, every click after should play AttackRapid
        if (regularAttackCount == 1)
            StartCoroutine(PlayAttackAnimation(hashIsAtkMedi, mediumAttackDuration));
        else
            StartCoroutine(PlayAttackAnimation(hashIsAtkRapid, rapidAttackDuration));
    }

    public void ResetAttackCombo()
    {
        regularAttackCount = 0;
    }

    public bool CanUseLargeAttack()
    {
        return !isLargeAttackPlaying && !isJumping;
    }

    public void PlayLargeAttack()
    {
        if (animator == null || !CanUseLargeAttack()) return;

        // Cancel any previous stuck coroutine before starting fresh
        if (largeAttackCoroutine != null)
            StopCoroutine(largeAttackCoroutine);

        largeAttackCoroutine = StartCoroutine(PlayLargeAttackRoutine());
    }

    private IEnumerator PlayLargeAttackRoutine()
    {
        isLargeAttackPlaying = true;

        // Force-clear and re-set to avoid getting stuck from a previous interrupted play
        animator.SetBool(hashIsAtkLrg, false);
        animator.SetBool(hashIsRunning, false);
        yield return null; // Wait one frame for animator to process the false
        animator.SetBool(hashIsAtkLrg, true);

        // Wait two frames for animator to transition into AttackLarge state
        yield return null;
        yield return null;

        // Wait until the AttackLarge state has fully played through
        float timeout = largeAttackDuration + 1f; // Safety timeout
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("AttackLarge") && info.normalizedTime >= 0.95f)
                break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        animator.SetBool(hashIsAtkLrg, false);
        isLargeAttackPlaying = false;
        largeAttackCoroutine = null;

        if (!isJumping)
            animator.SetBool(hashIsRunning, isMoving);
    }

    private bool isPauseTimeBusy = false;

    public void PlayPauseTime()
    {
        if (animator == null) return;

        if (isPauseTimeBusy) return;

        StartCoroutine(PlayPauseTimeRoutine());
    }

    private IEnumerator PlayPauseTimeRoutine()
    {
        isPauseTimeBusy = true;
        animator.SetBool(hashIsPauseTime, true);

        // Wait until the animator actually enters the PauseTime state
        yield return null;
        yield return null;

        // Wait until PauseTime state is done by checking normalized time
        while (true)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            bool inPauseTime = info.IsName("PauseTime");
            if (inPauseTime && info.normalizedTime >= 1f)
                break;
            // Exit if animator moved away from PauseTime already
            if (!inPauseTime && animator.GetBool(hashIsPauseTime) == false)
                break;
            yield return null;
        }

        animator.SetBool(hashIsPauseTime, false);
        isPauseTimeBusy = false;
    }

    public bool IsPauseTimePlaying()
    {
        return isPauseTimeBusy;
    }

    public float PlayReverseTime()
    {
        if (animator == null) return 0f;

        StartCoroutine(PlayBoolForDuration(hashIsReverse, reverseTimeDuration));
        return reverseTimeAnimDelay;
    }

    public void PlayDying()
    {
        if (animator == null) return;

        StopAllCoroutines();
        isBusy = true;
        isJumping = false;

        // Clear all other states
        animator.SetBool(hashIsRunning, false);
        animator.SetBool(hashIsAtkMedi, false);
        animator.SetBool(hashIsAtkRapid, false);
        animator.SetBool(hashIsAtkLrg, false);
        animator.SetBool(hashIsReverse, false);
        animator.SetBool(hashIsPauseTime, false);

        animator.SetBool(hashIsDying, true);
    }

    private IEnumerator PlayAttackAnimation(int paramHash, float duration)
    {
        isBusy = true;
        animator.SetBool(hashIsRunning, runLegsWhileAttacking && isMoving);
        animator.SetBool(paramHash, true);

        yield return new WaitForSeconds(duration);

        animator.SetBool(paramHash, false);
        isBusy = false;

        if (!isJumping)
            animator.SetBool(hashIsRunning, isMoving);
    }

    private IEnumerator PlayBoolForDuration(int paramHash, float duration)
    {
        animator.SetBool(paramHash, true);
        yield return new WaitForSeconds(duration);
        animator.SetBool(paramHash, false);
    }
}
