using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NewsPaperMove : MonoBehaviour
{
    float endPos = 0;
    float endPosTime = 1f;
    bool snapping;

    [SerializeField] private AnimationCurve newsPaperAnimationCurve;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        NewspaperRotate();
    }

    void NewspaperRotate()
	{

        transform.DORotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360).SetRelative(true)
            .SetEase(Ease.Linear).SetLoops(-1);

        transform.DOMoveY(endPos, endPosTime, snapping).SetEase(newsPaperAnimationCurve);
    }
}
