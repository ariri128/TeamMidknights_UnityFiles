using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Camera mainCamera;
    public HitMarker hitMarker;
    public float attackRange = 100f;
    public int damageAmount = 25;
    public int manaCost = 5;

    private PlayerMana playerMana;

    private void Awake()
    {
        playerMana = GetComponent<PlayerMana>();
    }

    private void Update()
    {
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
        if (playerMana == null)
        {
            return;
        }

        if (!playerMana.TrySpendMana(manaCost))
        {
            return;
        }

        Vector2 aimPosition;

        if (hitMarker != null)
        {
            aimPosition = hitMarker.GetHitMarkerScreenPosition();
        }
        else
        {
            aimPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }

        Ray ray = mainCamera.ScreenPointToRay(aimPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
        {
            GuardHealth guardHealth = hit.collider.GetComponentInParent<GuardHealth>();

            if (guardHealth != null)
            {
                guardHealth.TakeDamage(damageAmount);
                Debug.Log("Guard hit.");
            }
        }
    }
}
