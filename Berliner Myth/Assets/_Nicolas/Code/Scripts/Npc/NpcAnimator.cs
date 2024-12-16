using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using DG.Tweening;

public class NpcAnimator : NpcComponent
{

    [SerializeField] private float squishAmount = 0.8f; //scale factor of the squish
    [SerializeField] private float duration = 0.5f; //duration of the squish cycle

    private Vector3 originalScale;
    private void Update()
    {
        //store npc's origninal scale
        originalScale = transform.localScale;

        //starz the squish loop
        StartSquishAnimation();
    }

    private void StartSquishAnimation()
    {
        //create looping animation:
        transform.DOScale(new Vector3(originalScale.x, originalScale.y * squishAmount, originalScale.z), duration)
            .SetEase(Ease.InOutSine) //smooooooth
            .SetLoops(-1, LoopType.Yoyo);
    }
}