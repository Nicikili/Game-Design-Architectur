using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
	#region 
	public bool isListening = false; //checks if NPC is listening to player speech
	public bool approvalFull = false; //checks if first bar on NPC is Full
	public bool aggressionFull = false; //checks if second bar on NPC is Full
	#endregion

	void ListensToSpeech()
	{
		isListening = true;

	}


}
