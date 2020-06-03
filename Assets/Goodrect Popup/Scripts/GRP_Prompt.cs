using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum PromptType { Standard, Integer, Float, Alphanumeric, Password };

public class GRP_Prompt : GRP_PopupBase
{
    public InputField inputField;
    public RectTransform inputFieldRT;
    public Text placeholder;
    public Text input;
    public GRP_PromptEvent returnEvent;

    override public void Start()
    {
        base.Start();
    }

    public void SetTextColors(Color titleColor, Color returnTextColor, Color returnTextColorPressed)
    {
        this.titleColor = titleColor;
        this.returnTextColor = returnTextColor;
        this.returnTextColorPressed = returnTextColorPressed;
    }

    public void Initialize(string title, string message)
    {
        cg = GetComponent<CanvasGroup>();

        titleLabel.text = title;
        placeholder.text = message;

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 0);
        rt.localScale = new Vector2(1, 1);
        rt.offsetMin = rt.offsetMax = new Vector2(0, 0);
        dispose = false;
    }

    public void StyleReturnButton(float innerPadding, Sprite returnImage, Sprite returnPressedImage, Sprite image, Sprite pressedImage, Color returnTint, Color returnTintPressed, float bleed, bool prefixImage)
    {
        if (pressedImage == null) pressedImage = image;
        if (returnImage == null) returnImage = image;
        if (returnPressedImage == null) returnPressedImage = pressedImage;

        innerPadding /= 100f;

        PrepareButton(  returnButton, bleed, prefixImage,
                        returnTint, returnImage, returnTintPressed, returnPressedImage,
                        returnTextColor, returnTextColorPressed, buttonFontSize, buttonFont,
                        0f, innerPadding, 1f, 1f - innerPadding);

    }

    public void StyleInputField(PromptType pt, float inputfieldVerticalPadding, float titleSeparator, float buttonSeparator, float messagePadding, Sprite inputfieldBG, Color inputfieldTint, Color inputfieldTextColor, Color placeholderTextColor)
    {
        if (pt == PromptType.Standard)
            inputField.contentType = InputField.ContentType.Standard;
        else if (pt == PromptType.Integer)
            inputField.contentType = InputField.ContentType.IntegerNumber;
        else if (pt == PromptType.Float)
            inputField.contentType = InputField.ContentType.DecimalNumber;
        else if (pt == PromptType.Alphanumeric)
            inputField.contentType = InputField.ContentType.Alphanumeric;
        else if (pt == PromptType.Password)
            inputField.contentType = InputField.ContentType.Password;

        messagePadding /= 100f;
        buttonSeparator /= 100f;
        titleSeparator /= 100;
        inputfieldVerticalPadding /= 100;

        float vertPad = titleSeparator - buttonSeparator;
        vertPad *= inputfieldVerticalPadding;

        inputFieldRT.anchorMin = new Vector2(messagePadding, buttonSeparator + vertPad);
        inputFieldRT.anchorMax = new Vector2(1f - messagePadding, titleSeparator - vertPad);

        Image inputfieldImage = inputField.GetComponent<Image>();
        inputfieldImage.sprite = inputfieldBG;
        inputfieldImage.color = inputfieldTint;

        placeholder.color = placeholderTextColor;
        input.color = inputfieldTextColor;
    }

    public void AlignText()
    {
        Text[] textFields = GetComponentsInChildren<Text>();
        for (int i = 0; i < textFields.Length; i++)
            textFields[i].alignment = textAnchor;
    }

    override public void SetTypography(TextAlignment alignment, Font titleFont, Font bodyFont, int titleFontSize, int bodyFontSize, int buttonFontSize, Font buttonFont, Font returnFont = null)
    {
        base.SetTypography(alignment, titleFont, bodyFont, titleFontSize, bodyFontSize, buttonFontSize, buttonFont, returnFont);

        if (bodyFont != null)
        {
            placeholder.font = bodyFont;
            input.font = bodyFont;
        }

        if (bodyFontSize != 0)
        {
            placeholder.fontSize = bodyFontSize;
            input.fontSize = bodyFontSize;
        }

        placeholder.color = bodyTextColor;
        input.color = bodyTextColor;
    }

    override protected void ReturnWrapper()
    {
        if (dispose) return;

        //persistent events
        for (int i = 0; i < returnEvent.GetPersistentEventCount(); i++)
        {
            Component target = (Component)returnEvent.GetPersistentTarget(i);
            string method = returnEvent.GetPersistentMethodName(i);

            target.SendMessage(method, inputField.text);
        }

        //dynamic events
        if(returnEvent.runtimePromptActions != null)
            foreach(UnityAction<string> listener in returnEvent.runtimePromptActions)
                listener.Invoke(inputField.text);

        StartCoroutine(FadeOut());
        dispose = true;
    }

    public override void SetReturn(GRP_PromptButtonOptions options)
    {
        base.SetReturn(options);
        returnEvent = options.action;
    }
}
