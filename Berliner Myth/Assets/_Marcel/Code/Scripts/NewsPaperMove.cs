using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class NewsPaperMove : MonoBehaviour
{
    float endPos = 5.5f;
    float endPosTime = 1f;
    bool snapping;

    float endPosPicture = 13f;
    float endPosTimePicture = 45f;

    [SerializeField] GameObject Newspaper;
    [SerializeField] GameObject NewspaperSlapped;
    [SerializeField] GameObject Picture;

    [SerializeField] private AnimationCurve newsPaperAnimationCurve;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame.
    void Update()
    {
        NewspaperRotate();
    }

    void NewspaperRotate()
	{

        transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.FastBeyond360).SetRelative(true) //rotates the newspaper.
            .SetEase(Ease.Linear).SetLoops(-1);

        transform.DOMoveY(endPos, endPosTime, snapping).SetEase(newsPaperAnimationCurve); //moves the newspaper to front.

        Debug.Log(transform.position.y);

       if (transform.position.y > 0) //if the newspaper reached a certain point, it will switch out with the new newspapperSlapped GameObject.
        {
           NewspaperSlapped.SetActive(true);
           Newspaper.SetActive(false);
           DOTween.Kill(transform);

           if (SceneManager.GetActiveScene().name == "EndingScene3_blue")
           {
                Picture.transform.DOLocalMoveY(endPosPicture, endPosTimePicture, snapping).SetEase(newsPaperAnimationCurve);
            }

           if (SceneManager.GetActiveScene().name == "EndingScene3_red")
           {
                Picture.transform.DOLocalMoveY(endPosPicture, endPosTimePicture, snapping).SetEase(newsPaperAnimationCurve);
           }
        }
    }
}
