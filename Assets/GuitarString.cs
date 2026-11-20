using UnityEngine;

public class GuitarString : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    private AudioClip currentNote;

    [Header("Détection VR (Main droite / Plectre)")]
    public string handTag = "RightHand";

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void SetNote(AudioClip newClip)
    {
        currentNote = newClip;
    }

    public void PlaySound()
    {
        if (audioSource != null && currentNote != null)
        {
            audioSource.PlayOneShot(currentNote);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(handTag) || (other.transform.parent != null && other.transform.parent.CompareTag(handTag)))
        {
            PlaySound();
        }
    }
}