using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCred : NPCManager
{
	private void Start()
	{
		isred = true;
		saturationNPC= Mathf.Lerp(0, 100, speechTime);
		rendererNPC.material.color = Color.HSVToRGB(hueNPC = 0, saturationNPC = 0, valueNPC = 100);
	}
}
