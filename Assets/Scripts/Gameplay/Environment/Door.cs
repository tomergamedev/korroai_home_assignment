using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public bool IsAutoInteract() => false;

    public void Interact()
    {
        if (LevelManager.Instance.GetPlayerInventory().HasKey())
        {
            SceneLoader.LoadNextScene();
        }
        //else display message?
    }
}
