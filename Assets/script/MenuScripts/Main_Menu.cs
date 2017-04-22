using UnityEngine;
using System.Collections;

public class Main_Menu : MonoBehaviour {

    void Start() {
        QualitySettings.SetQualityLevel(Settings.graphic);
        AudioListener.volume = Settings.music_volume;
   }
    public void onPlay() {
        print("play");
    }
    public void onExit() {
        print("quit");
        Application.Quit();
    }
    public void onSetting() {
		Application.LoadLevel("main_menu_settings");
    }
}
