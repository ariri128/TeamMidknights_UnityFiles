using UnityEngine;
using UnityEngine.InputSystem;

public class WaterBallAttack : MonoBehaviour
{
    public GameObject waterBallPrefab;
    public Transform waterBallSpawnPoint;
    public Camera mainCamera;
    public HitMarker hitMarkerUI;
    public TimeSlow timeSlow;

    public float throwForce = 12f;
    public float attackRange = 100f;
    public int manaCost = 10;

    private PlayerMana playerMana;
    private PlayerAnimationController playerAnimation;

    private void Awake()
    {
        playerMana = GetComponent<PlayerMana>();
        playerAnimation = GetComponent<PlayerAnimationController>();
    }

    private void Update()
    {
        /*
        if (PauseManager.IsPaused)
        {
            return;
        }
        */

        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryThrowWaterBall();
        }
    }

    public void TriggerWaterBall() { TryThrowWaterBall(); }

    private void TryThrowWaterBall()
    {
        if (timeSlow == null || !timeSlow.IsSlowActive)
        {
            return;
        }

        if (waterBallPrefab == null || waterBallSpawnPoint == null || mainCamera == null)
        {
            return;
        }

        if (playerAnimation != null && !playerAnimation.CanUseLargeAttack())
        {
            return;
        }

        if (playerMana == null || !playerMana.TrySpendMana(manaCost))
        {
            return;
        }

        if (playerAnimation != null)
        {
            playerAnimation.PlayLargeAttack();
        }

        Vector2 aimPosition;

        if (hitMarkerUI != null)
        {
            aimPosition = hitMarkerUI.GetHitMarkerScreenPosition();
        }
        else
        {
            aimPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }

        Ray aimRay = mainCamera.ScreenPointToRay(aimPosition);

        Vector3 targetPoint;

        if (Physics.Raycast(aimRay, out RaycastHit hit, attackRange))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = aimRay.origin + aimRay.direction * attackRange;
        }

        Vector3 throwDirection = (targetPoint - waterBallSpawnPoint.position).normalized;

        GameObject waterBall = Instantiate(
            waterBallPrefab,
            waterBallSpawnPoint.position,
            Quaternion.LookRotation(throwDirection)
        );

        WaterBallSplash projectile = waterBall.GetComponent<WaterBallSplash>();

        if (projectile != null)
        {
            projectile.Launch(throwDirection);
        }

        Debug.Log("Water ball thrown.");
    }
}
