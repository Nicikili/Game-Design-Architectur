using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCred : NPCManager
{
	private void Awake()
	{
		isRed = true;
	}



	void changeHue()
	{
		//rendererNPC.material.color = Color.HSVToRGB(hueNPC = 0, saturationNPC = 0, valueNPC = 100);
	}
}