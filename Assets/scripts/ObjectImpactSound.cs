using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Rigidbody))]
public class ObjectImpactSound : MonoBehaviour
{
    [SerializeField] AudioClip[] clips;
    [SerializeField] float velocityScale = 1;
    [SerializeField] float soundCooldownSeconds = .05f;
    AudioSource audioSource;
    Rigidbody rb;

    bool isOnSoundCooldown = false;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (audioSource == null || clips.Length == 0 || isOnSoundCooldown) return;
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)], rb.linearVelocity.magnitude / velocityScale);
        isOnSoundCooldown = true;
        Invoke("ResetCooldown", soundCooldownSeconds);
    }

    void ResetCooldown()
    {
        isOnSoundCooldown = false;
    }
}
