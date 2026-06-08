using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HighlightFloat : MonoBehaviour
{
    public float moveY = 0.15f;
    public float duration = 0.6f;

    private Vector3 startLocalPos;
    private Tween floatTween;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    //动态显示Nala头顶的感叹号
    private void OnEnable()
    {
        transform.localPosition = startLocalPos;

        floatTween = transform
            .DOLocalMoveY(startLocalPos.y + moveY, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnDisable()
    {
        if (floatTween != null)
        {
            floatTween.Kill();
            floatTween = null;
        }

        transform.localPosition = startLocalPos;
    }
}
