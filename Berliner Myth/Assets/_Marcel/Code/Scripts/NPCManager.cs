using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
	#region 
	public bool isListening = false; //checks if NPC is listening to player speech

	public bool approvalFull = false; //checks if first bar on NPC is Full (right side)
	public bool aggressionNPCsAttackFull = false; //checks if second bar on NPC is Full (right side)

	public bool disapprovalFull = false; //checks if first bar on NPC is Full (left side)
	public bool aggressionPlayerAttackFull = false; //checks if second bar on NPC is Full (left side)

	public Renderer rendererNPC;
	public float hueNPC = 0;
	public float saturationNPC = 0;
	public float valueNPC = 0;
	public bool isBlue = false;
	public bool isRed = false;

	public bool speechRed = false;
	public bool speechBlue = false;
	public float Approval = 0f; //how long the NPC has been listening
	#endregion

	public void Start()
	{
		rendererNPC = GetComponent<Renderer>();
		rendererNPC.material.color = Color.HSVToRGB(hueNPC, saturationNPC, valueNPC);
	}

	void Update()
	{
		if (isListening)
		{
			ListensToSpeech();
		}
	}
	void ListensToSpeech()
	{
		if (isListening && isRed && speechRed)
		{
			Approval += 1 * Time.deltaTime;
		}

		if (isListening && isRed && speechBlue)
		{
			Approval -= 1 * Time.deltaTime;
		}

		if (isListening && isBlue && speechBlue)
		{
			Approval += 1 * Time.deltaTime;
		}

		if (isListening && isBlue && speechRed)
		{
			Approval -= 1 * Time.deltaTime;
		}
	}

	public void BehavoirChange()
	{
		// start following
		if (Approval == 100)
		{

		}

		// start attacking isBlue/isRed
		if (Approval == 200)
		{

		}

		//
		if (Approval == -100)
		{

		}

		//start attacking the Player
		if (Approval == -200)
		{

		}
	}
}
