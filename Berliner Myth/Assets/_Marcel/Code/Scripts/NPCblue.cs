using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCblue : NPCManager
{
    // Start is called before the first frame update
    private void Start()
	{
		isblue = true;
		rendererNPC.material.color = Color.HSVToRGB(hueNPC = 242, saturationNPC = 0, valueNPC = 100);
	}
}
