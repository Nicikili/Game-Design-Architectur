using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEvents : MonoBehaviour
{
	[field: Header("UI")]
	[field: SerializeField] public EventReference SE_SwitchAround { get; private set; }

	[field: Header("UI")]
	[field: SerializeField] public EventReference SE_MouseKlick { get; private set; }

	#region ambiente
	[field: Header("ambiente")]
	[field: SerializeField] public EventReference BackgroundNoise { get; private set; }
	#endregion

	#region music
	[field: Header("music")]
	[field: SerializeField] public EventReference ST_TitleScreen { get; private set; }

	[field: Header("music")]
	[field: SerializeField] public EventReference ST_InGame { get; private set; }

	[field: Header("music")]
	[field: SerializeField] public EventReference ST_Ending1 { get; private set; }

	[field: Header("music")]
	[field: SerializeField] public EventReference ST_Ending2 { get; private set; }

	[field: Header("music")]
	[field: SerializeField] public EventReference ST_Ending3 { get; private set; }
	#endregion

	#region NPC
	[field: Header("NPC")]
	[field: SerializeField] public EventReference VL_NPC_GoesToOtherNPC { get; private set; }
	#endregion

	#region Player
	[field: Header("player")]
	[field: SerializeField] public EventReference VL_Player_Speech { get; private set; }
	[field: SerializeField] public EventReference VL_Player_ScreamsInPain { get; private set; }
	[field: SerializeField] public EventReference SE_Player_TakeBox { get; private set; }
	[field: SerializeField] public EventReference SE_Player_PlaceBox { get; private set; }
	[field: SerializeField] public EventReference SE_Player_FahneAufstellen { get; private set; }
	[field: SerializeField] public EventReference SE_Player_FahneAbbauen { get; private set; }
	#endregion


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
