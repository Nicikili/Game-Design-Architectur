using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCblue : NPCManager
{
	private void Awake()
	{
		isBlue = true;
	}



	void changeHue()
	{
		rendererNPC.material.color = Color.HSVToRGB(hueNPC = 242, saturationNPC = 0, valueNPC = 100);
	}
}
