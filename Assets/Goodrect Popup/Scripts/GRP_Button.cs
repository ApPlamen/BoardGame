using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GRP_Button : Button
{
    private Text text;
    public Color normalText;
    public Color pressedText;

    public Sprite normalSprite;
    public Sprite pressedSprite;

    public Color normalTint;
    public Color pressedTint;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if(text == null)
            text = gameObject.GetComponentInChildren<Text>();

        switch(state)
        {
            case Selectable.SelectionState.Normal:
                text.color = normalText;
                base.image.sprite = normalSprite;
                base.image.color = normalTint;
                break;
            case Selectable.SelectionState.Highlighted:
                text.color = normalText;
                base.image.sprite = normalSprite;
                base.image.color = normalTint;
                break;
            case Selectable.SelectionState.Pressed:
                text.color = pressedText;
                base.image.sprite = pressedSprite;
                base.image.color = pressedTint;
                break;
            case Selectable.SelectionState.Disabled:
                text.color = normalText;
                base.image.sprite = normalSprite;
                base.image.color = normalTint;
                break;
            default:
                text.color = normalText;
                base.image.sprite = normalSprite;
                base.image.color = normalTint;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
