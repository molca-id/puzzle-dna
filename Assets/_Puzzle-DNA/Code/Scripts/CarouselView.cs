using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarouselView : MonoBehaviour
{
    public float imageGap = 30;
    public int swipeThrustHold = 30;
    public Transform fadingScreen;
    public RectTransform viewWindow;
    public List<RectTransform> images;

    bool canSwipe;
    float image_width;
    float lerpTimer;
    float lerpPosition;
    float mousePositionStartX;
    float mousePositionEndX;
    float dragAmount;
    float screenPosition;
    float lastScreenPosition;
    int m_currentIndex;

    void Start()
    {
        image_width = viewWindow.rect.width;
        for (int i = 1; i < images.Count; i++)
        {
            images[i].anchoredPosition = new Vector2(((image_width + imageGap) * i), 0);
        }

        UpdateFadingScreen();
    }

    void Update()
    {
        UpdateCarouselView();
    }

    void UpdateFadingScreen()
    {
        fadingScreen.SetAsLastSibling();
        images[m_currentIndex].SetAsLastSibling();
    }

    void UpdateCarouselView()
    {
        lerpTimer += Time.deltaTime;

        if (lerpTimer < 0.333f)
        {
            screenPosition = Mathf.Lerp(lastScreenPosition, lerpPosition * -1, lerpTimer * 3);
            lastScreenPosition = screenPosition;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    canSwipe = true;
                    mousePositionStartX = touch.position.x;
                    break;

                case TouchPhase.Moved:
                    if (canSwipe)
                    {
                        mousePositionEndX = touch.position.x;
                        dragAmount = mousePositionEndX - mousePositionStartX;
                        screenPosition = lastScreenPosition + dragAmount;
                    }
                    break;

                case TouchPhase.Ended:
                    if (Mathf.Abs(dragAmount) > swipeThrustHold && canSwipe)
                    {
                        canSwipe = false;
                        lastScreenPosition = screenPosition;
                        if (m_currentIndex < images.Count) OnSwipeComplete();
                        else if (m_currentIndex == images.Count && dragAmount < 0) lerpTimer = 0;
                        else if (m_currentIndex == images.Count && dragAmount > 0) OnSwipeComplete();
                        UpdateFadingScreen();
                    }
                    break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            canSwipe = true;
            mousePositionStartX = Input.mousePosition.x;
        }

        if (Input.GetMouseButton(0))
        {
            if (canSwipe)
            {
                mousePositionEndX = Input.mousePosition.x;
                dragAmount = mousePositionEndX - mousePositionStartX;
                screenPosition = lastScreenPosition + dragAmount;
            }
        }

        if (Mathf.Abs(dragAmount) > swipeThrustHold && canSwipe)
        {
            canSwipe = false;
            lastScreenPosition = screenPosition;
            if (m_currentIndex < images.Count) OnSwipeComplete();
            else if (m_currentIndex == images.Count && dragAmount < 0) lerpTimer = 0;
            else if (m_currentIndex == images.Count && dragAmount > 0) OnSwipeComplete();
            UpdateFadingScreen();
        }

        for (int i = 0; i < images.Count; i++)
        {
            images[i].anchoredPosition = new Vector2(screenPosition + ((image_width + imageGap) * i), 0);
            if (i == m_currentIndex)
            {
                images[i].localScale = Vector3.Lerp(images[i].localScale, new Vector3(1.2f, 1.2f, 1.2f), Time.deltaTime * 5);
            }
            else
            {
                images[i].localScale = Vector3.Lerp(images[i].localScale, new Vector3(0.7f, 0.7f, 0.7f), Time.deltaTime * 5);
            }
        }
    }

    void OnSwipeComplete()
    {
        lastScreenPosition = screenPosition;

        if (dragAmount > 0)
        {
            if (dragAmount >= swipeThrustHold)
            {
                if (m_currentIndex == 0)
                {
                    lerpTimer = 0; lerpPosition = 0;
                }
                else
                {
                    m_currentIndex--;
                    lerpTimer = 0;
                    if (m_currentIndex < 0) m_currentIndex = 0;
                    lerpPosition = (image_width + imageGap) * m_currentIndex;
                }
            }
            else
            {
                lerpTimer = 0;
            }
        }
        else if (dragAmount < 0)
        {
            if (Mathf.Abs(dragAmount) >= swipeThrustHold)
            {
                if (m_currentIndex == images.Count - 1)
                {
                    lerpTimer = 0;
                    lerpPosition = (image_width + imageGap) * m_currentIndex;
                }
                else
                {
                    lerpTimer = 0;
                    m_currentIndex++;
                    lerpPosition = (image_width + imageGap) * m_currentIndex;
                }
            }
            else
            {
                lerpTimer = 0;
            }
        }

        dragAmount = 0;
    }
}