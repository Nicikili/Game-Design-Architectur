using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ApprovalMeter : MonoBehaviour
{
    public Slider slider;
    public TMP_Text approvalNumber;
    public float approvalPoints; //current Approvalrating of the Player.

    public bool isRed; //Political allignement of NPC is red.
    public bool isBlue; //Political allignement of NPC is blue.

    public bool redSpeech; //Player speech is red.
    public bool blueSpeech; //Player speech is blue.

    public bool isListening;
    
    void Start()
    {
        ResetSlider();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D)) //Approval goes down.
		{
            ApprovalUp();
		}

        if (Input.GetKeyDown(KeyCode.A)) //Approval goes up.
        {
            ApprovalDown();
        }

        if (Input.GetKeyDown(KeyCode.S)) //resets Slider to 0.
        {
            ResetSlider();
        }
    }

	#region ApprovalMeterBehaviour
	void ApprovalUp() //after adding a follower.
	{
        if(approvalPoints != slider.maxValue) //approval can't go above 100.
		{
            approvalPoints += 1;
            slider.value = approvalPoints;

            approvalNumber.SetText(approvalPoints.ToString()); //changes float to string (VISUALIZE NUMBER)
        }
        
        else //if approval would go above 100, do nothing.
		{

        }
    }

	void ApprovalDown() //after losing a follower.
    {
        if(approvalPoints != slider.minValue) //approval can't go under 0.
		{
            approvalPoints -= 1;
            slider.value = approvalPoints;

            approvalNumber.SetText(approvalPoints.ToString()); //changes float to string (VISUALIZE NUMBER)
        }

        else //if approval would go under 0, do nothing.
		{
        
		}
    }

    void ResetSlider() //resets the slider back to 0 for a new Game Run.
    {
        slider.value = slider.minValue;
        approvalPoints = 0;
        approvalNumber.SetText("0");
    }
    #endregion ApprovalMeterBehaviour
}
