using UnityEngine;
using Momentum;

[RequireComponent(typeof(Collider2D))]
public class PlayerWindEffect : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("PlayerWindEffect: Could not find Rigidbody2D via PlayerReferences.");
    }

    void FixedUpdate()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (var col in colliders)
        {
            WindZone2D wind = col.GetComponent<WindZone2D>();
            if (wind != null && wind.IsActive())
            {
                rb.AddForce(wind.GetWindForce(), ForceMode2D.Force);
                Debug.Log($"AddForce: {wind.GetWindForce()} | Rigidbody velocity: {rb.linearVelocity}");

            }
        }
    }
}
