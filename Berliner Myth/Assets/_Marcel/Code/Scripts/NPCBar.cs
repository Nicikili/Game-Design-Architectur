using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCBar : NPCManager
{

	public Slider slider;
	public float speechTime = 0f;
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		ListensToSpeech();
		Debug.Log(speechTime);
	}

	void ListensToSpeech()
	{
		isListening = true;
		slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, speechTime);

		speechTime += 0.375f * Time.deltaTime;

	}

	void ResetSlider()
	{
		slider.value = slider.minValue;
	}
}
