using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
	[field: Header("SE_SwitchAround")]
	[field: SerializeField] public EventReference SE_SwitchAround { get; private set; }
	public static FMODEvents instance { get; private set; }
	private void Awake()
	{
		if (instance != null)
		{
			Debug.Log("Found more than one FMOD Events instance in the scene.");
		}

		instance = this;
	}
}
