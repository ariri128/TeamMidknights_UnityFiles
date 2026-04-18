using UnityEngine;

public class RippleDisable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharacterController CharacterController = GetComponent<CharacterController>();
        if (CharacterController)
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Stop();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
