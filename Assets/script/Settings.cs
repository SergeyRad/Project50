using UnityEngine;
using System.Collections.Generic;
using PlayerIOClient;

public abstract class Settings {

    public static float music_volume = 0.5f;
    public static float sound_volume = 0.5f;
    public static int graphic = 1;
    public static KeyCode keyShoot = KeyCode.Mouse0;
    public static KeyCode keyMenu = KeyCode.Escape;
    public static KeyCode keyReload = KeyCode.R;
    public static int cursorType = 8;

    public static string email = "";
    public static string roomId = "";

}
