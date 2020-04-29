using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject endPanel;
    [Space]
    [SerializeField] private Animation blackListRotate;
    [SerializeField] private LineRenderer whiteLine;

    private void OnEnable()
    {
        EventManager.OnLevelEnd += OnLevelEnd;
    }

    private void OnDisable()
    {
        EventManager.OnLevelEnd -= OnLevelEnd;
    }

    private void OnLevelEnd()
    {
        StartCoroutine(RotateList());
    }

    private IEnumerator RotateList()
    {
        yield return new WaitForSeconds(1f);
        var canvasAlpha = gamePanel.GetComponent<CanvasGroup>();
        blackListRotate.Play();
        whiteLine.enabled = false;
        while (canvasAlpha.alpha >0)
        {
            canvasAlpha.alpha -= Time.deltaTime;
            yield return null;
        }
        gamePanel.SetActive(false);
        canvasAlpha.alpha = 1;

        yield return new WaitForSeconds(0.5f);
        endPanel.SetActive(true);
    }
}