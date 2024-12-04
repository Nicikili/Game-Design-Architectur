using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovementShadowNPC : MonoBehaviour
{
	public GameObject ShadowNPC; //Shadow of NPC that can be seen in TitleScene

    #region Parameters for ShadowNPC
    public float movement; //20f, this is the xAxis, changes how big the swing from left to right/ right to left is
    public float shadowY; //-5, this is the yAxis, places character in scene
    public float shadowZ; //-25, this is the zAxis, places character in scene
    public float offset; //0, offset of the movement/xAxis, if 0 then nothing is affected
    public float offsetTime; //0, offset of the Time, if 0 then nothing is affected
    #endregion

    void Start()
    {
        //to start at zero
        StartCoroutine(Oscillate(OccilationFuntion.Sine, movement)); //USE THIS ONE! WORKS! :D
        #region OccilationFunction_Cosine_Inactive
        //to start at scalar value
        //StartCoroutine (Oscillate (OccilationFuntion.Cosine, 1f)); //THIS DOES NOT WORK AHHH D:
        #endregion
    }

    #region ShadowNPC_Movement
    public enum OccilationFuntion { Sine, Cosine }

    private IEnumerator Oscillate(OccilationFuntion method, float scalar)
    {
        while (true)
        {
            if (method == OccilationFuntion.Sine)
            {
                transform.position = new Vector3(Mathf.Sin(Time.time + offsetTime) * scalar + offset, shadowY, shadowZ); //move gameobject from left to right and right to left
            }
            #region OccilationFunction_Cosine_Inactive
            else if (method == OccilationFuntion.Cosine)
            {
                transform.position = new Vector3(Mathf.Cos(Time.time + offsetTime) * scalar + offset, shadowY, shadowZ); //this one is currently inactive, can be ignored
            }
            #endregion
            yield return new WaitForEndOfFrame(); //Waits, then starts again
        }
    }
    #endregion
}

