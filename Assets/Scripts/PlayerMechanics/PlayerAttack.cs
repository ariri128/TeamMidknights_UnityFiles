using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Camera mainCamera;
    public HitMarker hitMarkerUI;
    public TimeSlow timeSlow;

    public GameObject waterJetPrefab;
    public Transform waterJetSpawnPoint;

    public float attackRange = 100f;
    public int damageAmount = 25;
    public int manaCost = 5;

    public float minimumJetDistance = 2f;

    private PlayerMana playerMana;

    private void Awake()
    {
        playerMana = GetComponent<PlayerMana>();
    }

    private void Update()
    {
        /*
        if (PauseManager.IsPaused)
        {
            return;
        }
        */

        if (timeSlow != null && timeSlow.IsSlowActive)
        {
            return;
        }

        if (Mouse.current == null || mainCamera == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (playerMana == null || !playerMana.TrySpendMana(manaCost))
        {
            return;
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

        Ray ray = mainCamera.ScreenPointToRay(aimPosition);

        Vector3 targetPoint = ray.origin + ray.direction * attackRange;

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
        {
            targetPoint = hit.point;

            GuardHealth guardHealth = hit.collider.GetComponentInParent<GuardHealth>();

            if (guardHealth != null)
            {
                guardHealth.TakeDamage(damageAmount);
                Debug.Log("Guard hit.");
            }
        }

        SpawnWaterJet(targetPoint);
    }

    private void SpawnWaterJet(Vector3 targetPoint)
    {
        if (waterJetPrefab == null || waterJetSpawnPoint == null)
        {
            return;
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

        Vector3 direction = targetPoint - waterJetSpawnPoint.position;

        if (direction.magnitude < minimumJetDistance)
        {
            direction = aimRay.direction;
            targetPoint = waterJetSpawnPoint.position + direction * minimumJetDistance;
        }

        direction.Normalize();

        float distance = Vector3.Distance(waterJetSpawnPoint.position, targetPoint);

        GameObject waterJet = Instantiate(
            waterJetPrefab,
            waterJetSpawnPoint.position,
            Quaternion.LookRotation(direction)
        );

        WaterJetAttack projectile = waterJet.GetComponent<WaterJetAttack>();

        if (projectile != null)
        {
            projectile.Launch(direction, distance);
        }
    }
}
