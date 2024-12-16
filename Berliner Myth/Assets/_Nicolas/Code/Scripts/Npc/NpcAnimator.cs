using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using DG.Tweening;

public class NpcAnimator : NpcComponent
{

    [SerializeField] private float squishAmount = 0.8f; //scale factor of the squish
    [SerializeField] private float bobAmount = 0.1f; // Amount to move up and down
    [SerializeField] private float duration = 1.5f; //duration of the squish cycle

    [SerializeField] private Transform animatedPart;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private void Start()
    {

        if (animatedPart != null)
        {
            DOTween.SetTweensCapacity(2500, 50);

            //store npc's origninal scale
            originalScale = animatedPart.localScale;
            originalPosition = animatedPart.localPosition;

            //starz the squish loop
            StartSquishAnimation();
        }
    }

    private void StartSquishAnimation()
    {

        float randomDelay = Random.Range(0f, duration); // Randomize delay within the duration range

        //create looping animation:
        animatedPart.DOScale(new Vector3(originalScale.x, originalScale.y * squishAmount, originalScale.z), duration)
            .SetEase(Ease.InOutSine) //smooooooth
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(randomDelay); // Random start delay

        animatedPart.DOLocalMove(new Vector3(originalPosition.x, originalPosition.y + bobAmount, originalPosition.z), duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(randomDelay); // Random start delay

    }
}