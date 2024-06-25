using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _levelSelectionScreen;

    private void Awake()
    {
        GameCursor.ToggleMenuMode();
    }

    public void EnableLevelSelectionScreen(bool value)
    {
        _mainMenuScreen.SetActive(!value);
        _levelSelectionScreen.SetActive(value);
    }
}
