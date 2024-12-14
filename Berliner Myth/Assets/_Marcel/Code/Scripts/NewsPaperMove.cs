using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class NewsPaperMove : MonoBehaviour
{
	#region GameObjects and Variables
    [SerializeField] GameObject Newspaper;
    [SerializeField] GameObject NewspaperSlapped;
    [SerializeField] GameObject Picture;

    [SerializeField] private AnimationCurve newsPaperAnimationCurve;

    float endPos = 5.5f;
    float endPosTime = 1f;
    bool snapping;

    float endPosPicture = 13f;
    float endPosTimePicture = 45f;
    #endregion

    void Update()
    {
        NewspaperRotate();
    }

    //code probably has to be made more efficient later down the road, 
    //but currently works fine for the small task it needs to fulfill.
	#region NewspaperMovement
	void NewspaperRotate()
	{
        transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.FastBeyond360).SetRelative(true) //rotates the newspaper.
            .SetEase(Ease.Linear).SetLoops(-1);

        transform.DOMoveY(endPos, endPosTime, snapping).SetEase(newsPaperAnimationCurve); //moves the newspaper to front.

       if (transform.position.y > 0) //if the newspaper reached a certain point, it will switch out with the new newspapperSlapped GameObject.
       {
           NewspaperSlapped.SetActive(true);
           Newspaper.SetActive(false);
           DOTween.Kill(transform);

			//move Newspaper Picture of Ending 3 (blue) down
		    if (SceneManager.GetActiveScene().name == "EndingScene3_blue")
            {
                Picture.transform.DOLocalMoveY(endPosPicture, endPosTimePicture, snapping).SetEase(newsPaperAnimationCurve);
            }

            //move Newspaper Picture of Ending 3 (red) down
            if (SceneManager.GetActiveScene().name == "EndingScene3_red")
            {
                Picture.transform.DOLocalMoveY(endPosPicture, endPosTimePicture, snapping).SetEase(newsPaperAnimationCurve);
            }
       }
    }
    #endregion
}
