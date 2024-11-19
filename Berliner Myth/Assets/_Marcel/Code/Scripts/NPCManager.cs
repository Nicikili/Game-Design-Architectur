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
	public float hueNPC;
	public float saturationNPC = 0;
	public float valueNPC;
	public bool isblue = false;
	public bool isred = false;

	public float speechTime = 0f; //how long the NPC has been listening
	#endregion

	public void Awake()
	{
		rendererNPC = GetComponent<Renderer>();
	}


	void Update()
	{
		ListensToSpeech();
		Debug.Log(speechTime);
	}
	void ListensToSpeech()
	{
		if (isListening && isred)
		{
			Debug.Log("Hello");
			rendererNPC.material.color = Color.HSVToRGB(hueNPC = 0, saturationNPC = Mathf.Lerp(0, 100, speechTime), valueNPC = 100);

			speechTime += 0.375f * Time.deltaTime;
		}

		if (isblue)
		{
			rendererNPC.material.color = Color.HSVToRGB(hueNPC = 242, saturationNPC = 100, valueNPC = 100);
		}
	}

	public void BehavoirChange()
	{
		if (aggressionNPCsAttackFull)
		{

		}
	}


}
