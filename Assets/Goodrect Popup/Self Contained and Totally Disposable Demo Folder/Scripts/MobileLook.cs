using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MobileLook : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float limitx, limity;

    private bool firstDrag;

    public float valx, valy;

    public bool snapToMiddle = false;

    public MouseLook ml;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rt = transform.parent.GetComponent<RectTransform>();
        firstDrag = true;

        limitx = rt.rect.width / 2 - 50;
        limity = rt.rect.height / 2 - 50;
        ml.isMobile = true;
    }

    float startY;
    float startX;

    public void OnDrag(PointerEventData eventData)
    {
        if(firstDrag)
        {
            startX = Input.mousePosition.x;
            startY = Input.mousePosition.y;
        }
        float x = Input.mousePosition.x - startX;
        float y = Input.mousePosition.y - startY;

        if (x < -limitx) x = -limitx;
        else if (x > limitx) x = limitx;

        if (y < -limity) y = -limity;
        else if (y > limity) y = limity;

        transform.localPosition = new Vector2(x, y);

        if (firstDrag)
        {
            firstDrag = false;
        }

        valx = x / limitx;
        valy = y / limity;

        ml.ManualLook(-valy, valx);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        firstDrag = true;
        transform.localPosition = Vector2.zero;
        ml.ManualLook(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
