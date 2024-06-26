using UnityEngine;

public static class GameCursor
{
    // Start is called before the first frame update
    public static void ToggleGameMode()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void ToggleMenuMode()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
