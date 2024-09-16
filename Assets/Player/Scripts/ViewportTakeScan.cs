using System.Collections;
using UnityEngine;

public class ViewportTakeScan : MonoBehaviour
{
    [SerializeField] private ScanTaker m_scanTaker;
    [SerializeField] private Camera m_camera;
    [SerializeField] private PlayerController m_controller;
    [SerializeField] private int m_pointsCount;
    [SerializeField] private float m_timeAllScan;
    [SerializeField] private float m_timeBetweenScans;
    [SerializeField] private float m_playerSpeedFactor;
    [SerializeField] private float m_playerSensitivityFactor;

    private bool m_isMousePressed;
    private bool m_isScanning;
    private float m_timeSinceStartScan;
    private float m_scannedPointsCount;

    private void Start()
    {
        m_isMousePressed = false;
        m_isScanning = false;

        StartCoroutine(ScanTaking());
    }

    private void Update()
    {
        m_isMousePressed |= Input.GetMouseButton(0) && !GamePause.IsPaused;

        if (m_isScanning && GamePause.IsPaused == false && m_scannedPointsCount != m_pointsCount)
        {
            float timeOnePont = m_timeAllScan / m_pointsCount;

            float timeToScanNewPoints = m_timeSinceStartScan - timeOnePont * m_scannedPointsCount;

            int newPointsCount = Mathf.CeilToInt(timeToScanNewPoints / timeOnePont);

            for (int i = 0; i < newPointsCount; i++)
            {
                if (m_scannedPointsCount == m_pointsCount)
                {
                    m_isScanning = false;
                    break;
                }

                ScanPoint();
                m_scannedPointsCount++;
            }

            m_timeSinceStartScan += Time.deltaTime;
        }
    }

    private IEnumerator ScanTaking()
    {
        while (true)
        {
            if (m_isMousePressed && GamePause.IsPaused == false)
            {
                yield return StartCoroutine(RandomScansInCameraView());

                m_isMousePressed = false;

                yield return new WaitForSeconds(m_timeBetweenScans);
            }

            yield return null;
        }
    }

    private IEnumerator RandomScansInCameraView()
    {
        m_timeSinceStartScan = 0;
        m_scannedPointsCount = 0;
        m_isScanning = true;

        m_controller.Speed *= m_playerSpeedFactor;
        m_controller.Sensitivity *= m_playerSensitivityFactor;

        #region
        //float timeOneScan = m_timeAllScan / m_pointsCount;

        //int scanned = 0;
        //bool isScanning = true;

        //while (isScanning)
        //{
        //    //yield return new WaitWhile(() => GamePause.IsPaused);

        //    int pointsInIter = Mathf.CeilToInt(10 * Time.deltaTime / timeOneScan);
        //    int scannedInIter;

        //    for (scannedInIter = 0; scannedInIter < pointsInIter; ++scannedInIter)
        //    {
        //        if (scanned >= m_pointsCount)
        //        {
        //            isScanning = false;
        //            break;
        //        }

        //        ScanPoint();

        //        scanned++;
        //    }

        //    yield return new WaitForSeconds(timeOneScan * scannedInIter);
        //}
        #endregion

        yield return new WaitForSeconds(m_timeAllScan);

        //m_isScanning = false;

        m_controller.Speed /= m_playerSpeedFactor;
        m_controller.Sensitivity /= m_playerSensitivityFactor;
    }

    private void ScanPoint()
    {
        Vector3 randomPointInScreen = new Vector3(Random.Range(0f, m_camera.pixelWidth),
                                                  Random.Range(0f, m_camera.pixelHeight), -1);

        Vector3 worldPos = m_camera.ScreenToWorldPoint(randomPointInScreen);
        Vector3 direction = m_camera.transform.position - worldPos;

        m_scanTaker.ScanOnePoint(m_camera.transform.position, direction.normalized);
    }
}
