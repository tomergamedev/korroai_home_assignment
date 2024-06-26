using System.Collections;
using UnityEngine;
using TMPro;

public class LevelCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private GameObject _keyVisual;
    [SerializeField] private GameObject _interactGUI;
    [SerializeField] private GameObject _keyRequiredGUI;
    [SerializeField] private float _keyRequiredDisplayTime = 2f;

    const string SCORE_FORMAT = "Score: {0}";
    const string HEALTH_FORMAT = "Health: {0}";

    private Coroutine _keyRequiredMessageCoroutine;

    private void Start()
    {
        _interactGUI.SetActive(false);
        _keyVisual.SetActive(false);
        _keyRequiredGUI.SetActive(false);
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.text = string.Format(SCORE_FORMAT, score);
    }

    public void UpdateHealthText(int health)
    {
        _healthText.text = string.Format(HEALTH_FORMAT, health);
    }

    public void UpdateKeyCollected()
    {
        _keyVisual.SetActive(true);
    }

    public void UpdateInteractGUIActive(bool isActive)
    {
        _interactGUI.SetActive(isActive);
    }

    public void DisplayKeyRequiredMessage()
    {
        if (_keyRequiredMessageCoroutine != null)
        {
            StopCoroutine(_keyRequiredMessageCoroutine);
        }
        _keyRequiredMessageCoroutine = StartCoroutine(DisplayKeyRequiredMessageWithTimer());
    }

    private IEnumerator DisplayKeyRequiredMessageWithTimer()
    {
        _keyRequiredGUI.SetActive(true);
        yield return new WaitForSeconds(_keyRequiredDisplayTime);
        _keyRequiredGUI.SetActive(false);
    }
}
