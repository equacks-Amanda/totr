using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarratorVoicePulse : MonoBehaviour {
	
	private AudioSource as_voi;
	[SerializeField] private Material m_mat;
	[SerializeField] private float f_brightnessMin;
	[SerializeField] private float f_brightnessMax;
	private float[] clipSampleData;
	private float f_rawAmplitude;
	private float f_scaledAmplitude;
	[SerializeField] private int i_sampleDataLength = 1024;
	[SerializeField] private float f_updateStep = 0.1f;
	private float f_currentUpdateTime = 0f;

	// Use this for initialization
	void Start () {
		Maestro maestro = Maestro.Instance;
		as_voi = maestro.As_voi;
		clipSampleData = new float[i_sampleDataLength];
	}
	
	// Update is called once per frame
	void Update () {
		f_currentUpdateTime += Time.unscaledDeltaTime;
		if (f_currentUpdateTime >= f_updateStep) {
			f_currentUpdateTime = 0f;
			as_voi.clip.GetData(clipSampleData,as_voi.timeSamples);
			f_rawAmplitude = 0f;
			foreach(float sample in clipSampleData) 
				f_rawAmplitude += Mathf.Abs(sample);
			f_rawAmplitude /= i_sampleDataLength;
			f_scaledAmplitude = f_rawAmplitude / 0.15f * (f_brightnessMax - f_brightnessMin) + f_brightnessMin;
			m_mat.SetFloat("_Brightness",f_scaledAmplitude);
		}
	}
}
