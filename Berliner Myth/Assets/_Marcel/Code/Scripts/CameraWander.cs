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

    [SerializeField] GameObject SFX_Slider;
    [SerializeField] GameObject Ambiente_Slider;
    [SerializeField] GameObject Music_Slider;

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
	public void OnMouseDown()
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

                SFX_Slider.SetActive(false);
                Ambiente_Slider.SetActive(false);
                Music_Slider.SetActive(false);

                AudioManager.instance.PlayOneShot(FMODEvents.instance.SE_SwitchAround, this.transform.position);
            }

            if (hit.collider.gameObject.tag == "MoveToOptions") //Options Menu
            {
                Target = hit.collider.gameObject;
                WanderingCameraTransform.position = targetOptions.position;

                SFX_Slider.SetActive(true);
                Ambiente_Slider.SetActive(true);
                Music_Slider.SetActive(true);

                AudioManager.instance.PlayOneShot(FMODEvents.instance.SE_SwitchAround, this.transform.position);
            }

            if (hit.collider.gameObject.tag == "MoveToDevs") //Credit Menu Devs
            {
                Target = hit.collider.gameObject;
                WanderingCameraTransform.position = targetDevs.position;

                SFX_Slider.SetActive(false);
                Ambiente_Slider.SetActive(false);
                Music_Slider.SetActive(false);

                AudioManager.instance.PlayOneShot(FMODEvents.instance.SE_SwitchAround, this.transform.position);
            }


            if (hit.collider.gameObject.tag == "MoveToHowTo") //Controls Menu Screen
            {
                Target = hit.collider.gameObject;
                WanderingCameraTransform.position = targetHowTo.position;

                SFX_Slider.SetActive(false);
                Ambiente_Slider.SetActive(false);
                Music_Slider.SetActive(false);

                AudioManager.instance.PlayOneShot(FMODEvents.instance.SE_SwitchAround, this.transform.position);
            }
        }
    }
    #endregion
}
