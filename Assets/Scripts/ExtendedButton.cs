using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExtendedButton : Button, IPointerUpHandler, IPointerDownHandler
{
    private Image bgImage;

    private Coroutine LongTouch;

    private float longTocuhTimer = 0f;
    private float longTouchGoal = 1f;
    private bool isPressed = false;
    private bool readyToTouch = true;

    public Action<bool> LongTouchEventListener { get; set; }

    protected override void Awake()
    {
        base.Awake();

        bgImage = transform.parent.GetComponent<Image>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (readyToTouch == false || LongTouchEventListener == null)
            return;

        if (LongTouch != null)
            StopCoroutine(LongTouch);
        LongTouch = StartCoroutine(LongTouchCheckRoutine());
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if ( LongTouchEventListener == null)
            return;

        isPressed = false;
        bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, 1f);
    }

    private IEnumerator LongTouchCheckRoutine()
    {
        readyToTouch = false;
        yield return null;

        longTocuhTimer = 0f;
        isPressed = true;

        var originColor = bgImage.color;
        var ratio = 1f;
        while (isPressed)
        {
            longTocuhTimer += Time.deltaTime;
            if (longTocuhTimer > longTouchGoal)
                longTocuhTimer = longTouchGoal;
            ratio = (longTouchGoal - longTocuhTimer) / longTouchGoal;
            bgImage.color = new Color(originColor.r, originColor.g, originColor.b, ratio);
            yield return null;
        }

        LongTouchEventListener?.Invoke(longTocuhTimer >= longTouchGoal);

        yield return null;
        readyToTouch = true;
    }
}
