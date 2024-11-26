using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NPCBar : MonoBehaviour
{
#region
	public Slider slider;
	public float approvalPoints;

	public bool isRed; //Political allignement of NPC is red.
	public bool isBlue; //Political allignement of NPC is blue.

	public bool redSpeech; //Player speech is red.
	public bool blueSpeech; //Player speech is blue.
#endregion

void ListensToSpeechRed()
{

	//slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, approvalPoints);

	//approvalPoints += 0.375f * Time.deltaTime; //how fast the bar fills while listening
}

void ListensToSpeech()
{
		slider.value = Mathf.Lerp(slider.maxValue, slider.minValue, approvalPoints);

		//slider.value = Mathf.Lerp(slider.maxValue, slider.minValue, approvalPoints);


		//approvalPoints += 0.375f * Time.deltaTime; //how fast the bar fills while listening
	}

	void ApprovalUpDown()
	{
		if (isRed)
		{

		}

		if (isBlue)
		{

		}
	}



void ResetSlider()
{
	slider.value = slider.minValue;
}
}
