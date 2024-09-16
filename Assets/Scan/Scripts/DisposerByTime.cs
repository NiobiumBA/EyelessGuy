using System.Collections;
using UnityEngine;

public class DisposerByTime : MonoBehaviour
{
    [SerializeField] private float m_destroyingTime;

    private void Start()
    {
        StartCoroutine(ClearingUp());
    }

    private IEnumerator ClearingUp()
    {
        WaitForSeconds waiting = new WaitForSeconds(m_destroyingTime);

        while (true)
        {
            yield return waiting;

            RenderTexturesPool.CleanUp();
        }
    }
}
