using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionValueLoader : MonoBehaviour {
	[SerializeField] private Slider s_master;
	[SerializeField] private Slider s_music;
	[SerializeField] private Slider s_sfx;
	[SerializeField] private Slider s_voice;
	

	// Use this for initialization
	void OnLevelWasLoaded () {
		s_master.value = Constants.VolOptions.C_MasterVolume;
		s_music.value = Constants.VolOptions.C_BGMVolume;
		s_sfx.value = Constants.VolOptions.C_SFXVolume;
		s_voice.value = Constants.VolOptions.C_VOIVolume;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
