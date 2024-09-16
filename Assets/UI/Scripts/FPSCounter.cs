using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private Text m_textComponent;
    [SerializeField] private float m_timeToUpdate;
    [SerializeField] private string m_FPSTagInText;
    [SerializeField] private string m_text;

    private void Start()
    {
        StartCoroutine(UpdateFPS());
    }

    private IEnumerator UpdateFPS()
    {
        while (true)
        {
            float fps = 1f / Time.smoothDeltaTime;
            if (fps == float.PositiveInfinity)
                yield return null;

            m_textComponent.text = m_text.Replace(m_FPSTagInText, Mathf.RoundToInt(fps).ToString());

            yield return new WaitForSeconds(m_timeToUpdate);
        }
    }
}
