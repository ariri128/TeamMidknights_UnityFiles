using UnityEngine;

public class WaterJetAttack : MonoBehaviour
{
    public float speed = 35f;
    public float lifetime = 0.35f;

    private Vector3 moveDirection;
    private Vector3 startPosition;
    private float maxDistance;
    private float timer;
    private bool hasTargetDistance = false;

    public void Launch(Vector3 direction, float distance)
    {
        AudioManager.Instance.Play(AudioManager.SoundType.WaterPowerReg);
        startPosition = transform.position;
        moveDirection = direction.normalized;
        maxDistance = distance;
        hasTargetDistance = true;

        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float moveAmount = speed * Time.deltaTime;
        transform.position += moveDirection * moveAmount;

        if (hasTargetDistance)
        {
            float traveled = Vector3.Distance(startPosition, transform.position);

            if (traveled >= maxDistance)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
