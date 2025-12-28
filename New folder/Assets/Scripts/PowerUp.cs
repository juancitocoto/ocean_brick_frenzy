using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;

    public enum PowerUpType
    {
        SpeedBoost,      // Temporary speed increase
        Magnet,          // Attract nearby small bricks
        Shield,          // Temporary invincibility
        SizeBoost,       // Instant size increase
        SlowMotion,      // Slow all enemies
        Demolisher       // Break apart one larger enemy
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerBrick>();
            if (player != null)
            {
                ApplyPowerUp(player);
                Destroy(gameObject);
            }
        }
    }

    void ApplyPowerUp(PlayerBrick player)
    {
        switch (type)
        {
            case PowerUpType.SpeedBoost:
                player.StartCoroutine(SpeedBoost(player, 5f));
                break;
            case PowerUpType.Magnet:
                player.StartCoroutine(MagnetEffect(player, 8f));
                break;
            case PowerUpType.Shield:
                player.StartCoroutine(ShieldEffect(player, 5f));
                break;
            case PowerUpType.SizeBoost:
                player.currentSize += 10;
                break;
            case PowerUpType.SlowMotion:
                StartCoroutine(SlowMotionEffect(5f));
                break;
            case PowerUpType.Demolisher:
                // Simple demolisher: find one larger enemy and reduce its size
                DemolishOneEnemy();
                break;
        }
    }

    IEnumerator SpeedBoost(PlayerBrick player, float duration)
    {
        float originalSpeed = player.moveSpeed;
        player.moveSpeed *= 2f;
        yield return new WaitForSeconds(duration);
        player.moveSpeed = originalSpeed;
    }

    IEnumerator MagnetEffect(PlayerBrick player, float duration)
    {
        float end = Time.time + duration;
        while (Time.time < end)
        {
            Collider[] hits = Physics.OverlapSphere(player.transform.position, 5f);
            foreach (var c in hits)
            {
                if (c.CompareTag("Brick"))
                {
                    Vector3 dir = (player.transform.position - c.transform.position).normalized;
                    c.transform.position += dir * Time.deltaTime * 5f;
                }
            }
            yield return null;
        }
    }

    IEnumerator ShieldEffect(PlayerBrick player, float duration)
    {
        // Simple flag on player (user can extend PlayerBrick to react to this)
        player.isShielded = true;
        yield return new WaitForSeconds(duration);
        player.isShielded = false;
    }

    IEnumerator SlowMotionEffect(float duration)
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(duration * 0.5f);
        Time.timeScale = 1f;
    }

    void DemolishOneEnemy()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies != null && enemies.Length > 0)
        {
            // pick the largest by approximate size (bounding box)
            GameObject target = null;
            float maxVol = 0f;
            foreach (var e in enemies)
            {
                var r = e.GetComponent<Renderer>();
                if (r != null)
                {
                    var vol = r.bounds.size.x * r.bounds.size.y * r.bounds.size.z;
                    if (vol > maxVol)
                    {
                        maxVol = vol;
                        target = e;
                    }
                }
            }

            if (target != null)
            {
                // simple demolish effect: destroy it
                Destroy(target);
            }
        }
    }
}
