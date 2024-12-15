using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
//https://www.youtube.com/watch?v=rcBHIOjZDpk&ab_channel=ShapedbyRainStudios

public class AudioManagerScript : MonoBehaviour
{
	public static AudioManagerScript instance { get; private set; }

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("Found more than one Audio Manager in the scene.");
		}
		instance = this;
	}

	public void PlayOneShot(EventReference sound, Vector3 worldPos)
	{
	RuntimeManager.PlayOneShot(sound, worldPos);
	}
}
