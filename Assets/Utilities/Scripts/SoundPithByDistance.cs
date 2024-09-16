using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPithByDistance : MonoBehaviour
{
    [SerializeField] private AnimationCurve m_pithByDistance;

    private AudioSource m_audioSource;
    private Transform m_audioListener;

    public AnimationCurve PithByDistance
    {
        get => m_pithByDistance; set { m_pithByDistance = value; }
    }

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();

        m_audioListener = FindObjectOfType<AudioListener>().transform;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, m_audioListener.position);

        m_audioSource.pitch = m_pithByDistance.Evaluate(distance);
    }
}
