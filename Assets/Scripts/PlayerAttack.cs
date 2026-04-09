using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Camera mainCamera;
    public HitMarker hitMarker;
    public float attackRange = 100f;
    public int damageAmount = 25;

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

    /*
    public Camera mainCamera;
    public float attackRange = 100f;
    public int damageAmount = 25;

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
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);

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
    */

    /*
    public Camera mainCamera;
    public float attackRange = 100f;
    public int damageAmount = 25;

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
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

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
    */
}
