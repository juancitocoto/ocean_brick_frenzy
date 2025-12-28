using UnityEngine;

public class BreakApartEffect : MonoBehaviour
{
    public GameObject studParticlePrefab;
    public AudioClip[] breakSounds;

    public void PlayEffect(Vector3 position, int brickCount, Color mainColor)
    {
        ParticleSystem particles = GetComponent<ParticleSystem>();
        if (particles != null)
        {
            var main = particles.main;
            main.startColor = mainColor;

            var emission = particles.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)Mathf.Max(1, brickCount * 2)));

            transform.position = position;
            particles.Play();
        }

        if (breakSounds != null && breakSounds.Length > 0)
        {
            AudioSource.PlayClipAtPoint(breakSounds[Random.Range(0, breakSounds.Length)], position);
        }

        for (int i = 0; i < brickCount; i++)
        {
            Vector3 studPos = position + Random.insideUnitSphere * 2f;
            if (studParticlePrefab == null) continue;
            GameObject stud = Instantiate(studParticlePrefab, studPos, Random.rotation);
            Rigidbody rb = stud.GetComponent<Rigidbody>();
            if (rb == null) rb = stud.AddComponent<Rigidbody>();
            rb.AddForce(Random.insideUnitSphere * 10f, ForceMode.Impulse);
            Destroy(stud, 3f);
        }

        Destroy(gameObject, 2f);
    }
}
