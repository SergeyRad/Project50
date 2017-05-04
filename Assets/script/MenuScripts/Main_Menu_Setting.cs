using UnityEngine;
using UnityEngine.UI;

public class Main_Menu_Setting : MonoBehaviour {

	public Slider graphics;
	public Slider sound_volume;
	public Slider music_volume;
	void Start () {
		graphics.value = Settings.graphic;
		sound_volume.value = Settings.sound_volume;
		music_volume.value = Settings.music_volume;
	}
	void Update () {
		QualitySettings.SetQualityLevel(Settings.graphic);
		AudioListener.volume = Settings.music_volume;
		Settings.sound_volume = sound_volume.value;
		Settings.music_volume = music_volume.value;
		Settings.graphic = Mathf.RoundToInt(graphics.value);
	}	public void onBack(){
		Application.LoadLevel("main_menu");
	}
}
