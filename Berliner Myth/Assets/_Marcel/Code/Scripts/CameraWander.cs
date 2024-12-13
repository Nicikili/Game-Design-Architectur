using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWander : MonoBehaviour
{
	#region TransformsForCamera
    [SerializeField] Transform WanderingCameraTransform;
    [SerializeField] Transform targetMain;
    [SerializeField] Transform targetOptions;
    [SerializeField] Transform targetDevs;
    [SerializeField] Transform targetMusician;
    [SerializeField] Transform targetHowTo;
    #endregion

    #region RaycastStuff
    Ray ray;
	RaycastHit hit;
	public int number = 0;
	public GameObject Target;
    #endregion

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //if Mouse Button is clicked
        {
            //Debug.Log("MouseDown");
            OnMouseDown(); //Controls and changes Location of Wandering Camera
        }
    }

    //This stuff controls the Camera Movement in the TitleScene/TitleMenu
	#region PositionsWanderingCameraOnScreen
	void OnMouseDown()
    {
        // Reset ray with new mouse position
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag == "MoveToMain")
            {
                //GameObject will get hit by a Raycast
                Target = hit.collider.gameObject;
                //change camera position to position of an specific empty GameObject in CameraLocations
                WanderingCameraTransform.position = targetMain.position; //Main Menu
                //Debug.Log("Hit");
            }

            if (hit.collider.gameObject.tag == "MoveToOptions") //Options Menu
            {
                Target = hit.collider.gameObject;
                WanderingCameraTransform.position = targetOptions.position;
            }

            if (hit.collider.gameObject.tag == "MoveToDevs") //Credit Menu Devs
            {
                Target = hit.collider.gameObject;
                WanderingCameraTransform.position = targetDevs.position;
            }

            if (hit.collider.gameObject.tag == "MoveToMusican") //Credit Menu Musican
            {
                Target = hit.collider.gameObject;
                WanderingCameraTransform.position = targetMusician.position;
            }

            if (hit.collider.gameObject.tag == "MoveToHowTo") //Controls Menu Screen
            {
                Target = hit.collider.gameObject;
                WanderingCameraTransform.position = targetHowTo.position;
            }
        }
    }
    #endregion
}
