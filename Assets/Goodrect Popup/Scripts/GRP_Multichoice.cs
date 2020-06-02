using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GRP_Multichoice : GRP_PopupBase
{
    private Color buttonTextColor;
    private Color buttonTextColorPressed;

    public RectTransform messageRT;
    public Text messageLabel;

    public ReturnPosition rpos;

    public List<GRP_Button> buttons;
    public List<GRP_Event> buttonEvents;
    public GRP_Event returnEvent;

    public GameObject buttonPF;

    override public void Start()
    {
        base.Start();

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => { ButtonWrapper(index); });
        }
    }

    public void Initialize(string title, string message)
    {
        cg = GetComponent<CanvasGroup>();

        titleLabel.text = title;
        messageLabel.text = message;

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 0);
        rt.localScale = new Vector2(1, 1);
        rt.offsetMin = rt.offsetMax = new Vector2(0, 0);
        dispose = false;
    }

    private void ButtonWrapper(int index)
    {
        if (dispose) return;

        buttonEvents[index]?.Invoke();

        //dynamic events
        if (buttonEvents[index].runtimeActions != null)
            foreach (UnityAction listener in buttonEvents[index].runtimeActions)
                if(listener != null)
                    listener.Invoke();


        StartCoroutine(FadeOut());
        dispose = true;
    }

    override protected void ReturnWrapper()
    {
        if (dispose) return;

        returnEvent?.Invoke();

        //dynamic events
        if (returnEvent.runtimeActions != null)
            foreach (UnityAction listener in returnEvent.runtimeActions)
                listener.Invoke();

        StartCoroutine(FadeOut());
        dispose = true;
    }


    public void SetTextColors(Color titleColor, Color bodyTextColor, Color buttonTextColor, Color returnTextColor, Color buttonTextColorPressed, Color returnTextColorPressed)
    {
        this.titleColor = titleColor;
        this.bodyTextColor = bodyTextColor;
        this.buttonTextColor = buttonTextColor;
        this.returnTextColor = returnTextColor;
        this.buttonTextColorPressed = buttonTextColorPressed;
        this.returnTextColorPressed = returnTextColorPressed;
    }

    public void StyleMessageText(float titleSeparator, float buttonSeparator, float messagePadding)
    {
        messagePadding /= 100f;
        buttonSeparator /= 100f;
        titleSeparator /= 100;

        messageRT.anchorMin = new Vector2(messagePadding, buttonSeparator);
        messageRT.anchorMax = new Vector2(1f - messagePadding, titleSeparator);
    }


    public void StyleReturnButton(float innerPadding, Sprite returnImage, Sprite returnPressedImage, Sprite image, Sprite pressedImage, Color returnTint, Color returnTintPressed, float bleed, bool prefixImage, ButtonDirection bd = ButtonDirection.Vertical)
    {
        if (pressedImage == null) pressedImage = image;
        if (returnImage == null) returnImage = image;
        if (returnPressedImage == null) returnPressedImage = pressedImage;

            float size = 1f / (buttons == null ? 1 : buttons.Count + 1);

        float returnPos;

        if (rpos == ReturnPosition.Last)
        {
            returnPos = 0;
            returnButton.transform.SetAsLastSibling();
        }
        else
        {
            returnPos = 1f - size;
            returnButton.transform.SetAsFirstSibling();
        }

        innerPadding = size * innerPadding / 100f;

        float axMin, ayMin, axMax, ayMax;

        if (bd == ButtonDirection.Vertical)
        {
            axMin = 0f;
            ayMin = returnPos + innerPadding;

            axMax = 1f;
            ayMax = returnPos + size - innerPadding;
        }
        else
        {
            axMin = 1f - returnPos + innerPadding - size;
            ayMin = 0f;

            axMax = 1f - returnPos - innerPadding;
            ayMax = 1f;
        }

        PrepareButton(  returnButton, bleed, prefixImage,
                        returnTint, returnImage, returnTintPressed, returnPressedImage,
                        returnTextColor, returnTextColorPressed, buttonFontSize, returnFont,
                        axMin, ayMin, axMax, ayMax);
    }

    public void StyleButtons(bool showReturn, ButtonDirection bd, float innerPadding, Sprite image, Sprite pressedImage, Color generalTint, Color generalTintPressed, float bleed, bool prefixImage)
    {
        if (showReturn == false && buttons == null)
            return;

        if (pressedImage == null) pressedImage = image;

        float buttonsCount;

        if (buttons == null) buttonsCount = 1;
        else buttonsCount = buttons.Count + (showReturn ? 1 : 0);

        float size = 1f / buttonsCount;

        float start = 1f;

        if (rpos == ReturnPosition.First && showReturn)
        {
            start -= size;
        }

        innerPadding = size * innerPadding / 100f;

        if (!showReturn)
            returnButton.gameObject.SetActive(false);

        if (buttons != null)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                float axMin, ayMin, axMax, ayMax;

                if (bd == ButtonDirection.Vertical)
                {
                    axMin = 0f;
                    ayMin = start - (i + 1) * size;

                    axMax = 1f;
                    ayMax = start - i * size;

                    ayMin += innerPadding;
                    ayMax -= innerPadding;
                }
                else
                {
                    axMin = 1 - (start - i * size);
                    ayMin = 0f;

                    axMax = 1 - (start - (i + 1) * size);
                    ayMax = 1f;

                    axMin += innerPadding;
                    axMax -= innerPadding;
                }

                PrepareButton(  buttons[i], bleed, prefixImage,
                                generalTint, image, generalTintPressed, pressedImage,
                                buttonTextColor, buttonTextColorPressed, buttonFontSize, buttonFont,
                                axMin, ayMin, axMax, ayMax);
            }
        }
    }

    public void AlignText()
    {
        Text[] textFields = GetComponentsInChildren<Text>();
        for (int i = 0; i < textFields.Length; i++)
            textFields[i].alignment = textAnchor;
    }


    override public void SetTypography(TextAlignment alignment, Font titleFont, Font bodyFont, int titleFontSize, int bodyFontSize, int buttonFontSize, Font buttonFont, Font returnFont)
    {
        base.SetTypography(alignment, titleFont, bodyFont, titleFontSize, bodyFontSize, buttonFontSize, buttonFont, returnFont);

        if (bodyFont != null)
            messageLabel.font = bodyFont;

        if (bodyFontSize != 0)
            messageLabel.fontSize = bodyFontSize;

        messageLabel.color = bodyTextColor;
    }


    public void AddButton(GRP_ButtonOptions options)
    {
        GameObject buttonObj;

        buttonObj = Instantiate(buttonPF);

        if (buttons == null)
        {
            buttons = new List<GRP_Button>();
            buttonEvents = new List<GRP_Event>();
        }

        buttonObj.transform.SetParent(buttonsRT.transform);

        GRP_Button button = buttonObj.GetComponent<GRP_Button>();

        if (options.text.Trim() != "")
        {
            buttonObj.GetComponentInChildren<Text>().text = options.text;
            buttonObj.transform.Find("Visual").GetComponent<Image>().color = Color.clear;
        }
        else if (options.image != null)
        {
            buttonObj.GetComponentInChildren<Text>().text = "";
            Image visual = buttonObj.transform.Find("Visual").GetComponent<Image>();
            visual.sprite = options.image;
            visual.color = Color.white;
        }
        else
        {
            buttonObj.GetComponentInChildren<Text>().text = "";
            buttonObj.transform.Find("Visual").GetComponent<Image>().color = Color.clear;
        }

        buttonEvents.Add(options.action);
        buttons.Add(button);

        int index = buttonEvents.Count - 1;

        button.onClick.AddListener(() => { ButtonWrapper(index); });
    }

    public void SetReturn(GRP_ButtonOptions options)
    {
        GRP_PromptButtonOptions bo = new GRP_PromptButtonOptions();
        bo.text = options.text;
        bo.image = options.image;

        base.SetReturn(bo);
        returnEvent = options.action;
    }
}
