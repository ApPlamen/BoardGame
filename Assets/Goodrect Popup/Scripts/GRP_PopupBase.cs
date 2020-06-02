using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GRP_PopupBase : MonoBehaviour
{
    protected CanvasGroup cg;
    protected bool dispose = false;

    protected int buttonFontSize;
    protected Font buttonFont;
    protected Font returnFont;

    protected Color titleColor;
    protected Color bodyTextColor;
    protected Color returnTextColor;
    protected Color returnTextColorPressed;
    protected float fadeR, fadeG, fadeB;

    [Header("Transforms")]
    public RectTransform titleRT;
    public RectTransform buttonsRT;
    public RectTransform verticalPaddingRT;

    [Header("Canvas")]
    public Image background;
    public Transform container;
    public Image containerBG;
    public bool inWorld;
    public Vector3Bool followAxes;

    [Space]

    public Text titleLabel;
    public TextAnchor textAnchor;

    [Header("Return Button")]
    public GRP_Button returnButton;

    //Fade Animation
    public FadeAnimation fadeType;
    public float fadeDur;
    public AnimationCurve fadeProfile;

    // Start is called before the first frame update
    virtual public void Start()
    {
        if(FindObjectOfType<EventSystem>() == null) {
            GameObject eventListener = new GameObject();
            eventListener.name = "GRP Generated Event Listener";
            eventListener.AddComponent<EventSystem>();
        }

        if (FindObjectOfType<StandaloneInputModule>() == null)
        {
            EventSystem esObj = FindObjectOfType<EventSystem>();
            esObj.gameObject.AddComponent<StandaloneInputModule>();
        }

        returnButton.onClick.AddListener(ReturnWrapper);
    }


    // Update is called once per frame
    void Update()
    {
        if (inWorld) {
            RectTransform rt = GetComponent<RectTransform>();
            float rotx, roty, rotz;
            rotx = rt.localEulerAngles.x;
            roty = rt.localEulerAngles.y;
            rotz = rt.localEulerAngles.z;

            rt.LookAt(Camera.main.transform.position);
            float rotx_la, roty_la, rotz_la;
            rotx_la = rt.localEulerAngles.x;
            roty_la = rt.localEulerAngles.y;
            rotz_la = rt.localEulerAngles.z;

            if (followAxes.x)
                rotx = -rotx_la;

            if (followAxes.y)
                roty = roty_la + 180;

            if (followAxes.z)
                rotz = rotz_la;

            rt.eulerAngles = new Vector3(rotx, roty, rotz);
        }
    }

    virtual protected void ReturnWrapper() {
    }

    protected void PrepareButton(   GRP_Button button, float bleed, bool prefixImage,
                                    Color tint, Sprite sprite, Color tintPressed, Sprite spritePressed,
                                    Color textColor, Color textColorPressed, int textSize, Font font,
                                    float axMin, float ayMin, float axMax, float ayMax)
    {

        RectTransform rt = button.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(axMin, ayMin);
        rt.anchorMax = new Vector2(axMax, ayMax);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0);
        rt.localEulerAngles = Vector3.zero;

        Image buttonBG = button.GetComponentInChildren<Image>();
        buttonBG.sprite = sprite;
        buttonBG.color = tint;

        Text textComponent = button.GetComponentInChildren<Text>();

        if (buttonFont != null)
            textComponent.font = font;

        textComponent.fontSize = textSize;
        textComponent.color = textColor;


        RectTransform bgrt = buttonBG.GetComponent<RectTransform>();

        if (prefixImage)
        {
            AspectRatioFitter arf = buttonBG.gameObject.AddComponent<AspectRatioFitter>();
            arf.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            arf.aspectRatio = 1;

            bgrt.pivot = new Vector2(0, 0.5f);
            bgrt.offsetMin = new Vector2(-bleed, bgrt.offsetMin.y);
            buttonBG.type = Image.Type.Simple;

            RectTransform trt = textComponent.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;

            trt.offsetMin = new Vector2(bleed, 0);
        }
        else
        {
            bgrt.offsetMin = new Vector2(-bleed, -bleed);
            bgrt.offsetMax = new Vector2(bleed, bleed);
        }

        button.normalText = textColor;
        button.pressedText = textColorPressed;
        button.normalTint = tint;
        button.normalSprite = sprite;
        button.pressedTint = tintPressed;
        button.pressedSprite = spritePressed;
    }

    public void FadePopupIn()
    {
        if (fadeType == FadeAnimation.Fade || fadeType == FadeAnimation.Expand)
        {
            if (cg == null)
                cg = GetComponent<CanvasGroup>();

            cg.alpha = 0;
        }
        else if(fadeType == FadeAnimation.Shrink)
        {
            container.localScale = Vector3.zero;
        }

        StartCoroutine("FadeIn");
    }

    protected IEnumerator FadeIn()
    {
        float r, g, b, a;
        r = g = b = a = 0;
        if (fadeType == FadeAnimation.Fade || fadeType == FadeAnimation.Expand)
        {
            if (cg == null)
                cg = GetComponent<CanvasGroup>();
        }
        else if (fadeType == FadeAnimation.Shrink)
        {
            r = background.color.r;
            g = background.color.g;
            b = background.color.b;
            a = background.color.a;

            background.color = Color.clear;
        }

        int steps = Mathf.RoundToInt(60 * fadeDur);
        if (steps < 1) steps = 1;

        float curveSteps = 1f / steps;

        yield return null;

        for (int i = 0; i < steps; i++)
        {
            float val = fadeProfile.Evaluate(i * curveSteps);

            if (fadeType == FadeAnimation.Fade)
            {
                cg.alpha = val;
            }
            else if (fadeType == FadeAnimation.Shrink)
            {
                container.localScale = new Vector3(val, val, val);
                background.color = new Color(r, g, b, val * a);
            }
            else
            {
                cg.alpha = val;
                if (val < 0.01f) val = 0.01f;
                container.localScale = new Vector3(1 / val, 1 / val, 1 / val);
            }
            yield return new WaitForSeconds(.0167f); //60 steps per second
        }

        if (fadeType == FadeAnimation.Fade || fadeType == FadeAnimation.Expand)
        {
            cg.alpha = 1;
        }
        else if (fadeType == FadeAnimation.Shrink)
        {
            background.color = new Color(r,g,b,a);
        }

        container.localScale = new Vector3(1,1,1);

        yield return null;
    }


    protected IEnumerator FadeOut()
    {
        float r, g, b, a;
        r = g = b = a = 0;
        if(fadeType == FadeAnimation.Fade || fadeType == FadeAnimation.Expand)
        {
            if (cg == null)
                cg = GetComponent<CanvasGroup>();
        }
        else if(fadeType == FadeAnimation.Shrink)
        {
            r = background.color.r;
            g = background.color.g;
            b = background.color.b;
            a = background.color.a;
        }

        int steps = Mathf.RoundToInt(60 * fadeDur);
        float curveSteps = 1f / steps;

        yield return null;

        for (int i = 0; i < steps; i++) {
            float val = 1 - fadeProfile.Evaluate(i * curveSteps);

            if(fadeType == FadeAnimation.Fade)
            {
                cg.alpha = val;
            }
            else if(fadeType == FadeAnimation.Shrink)
            {
                container.localScale = new Vector3(val, val, val);
                background.color = new Color(r, g, b, val * a);
            }
            else
            {
                cg.alpha = val;
                if (val < 0.01f) val = 0.01f;
                container.localScale = new Vector3(1 / val, 1 / val, 1 / val);
            }
            yield return new WaitForSeconds(.0167f); //60 steps per second
        }

        Destroy(gameObject);
        yield return null;
    }

    virtual public void SetTypography(TextAlignment alignment, Font titleFont, Font bodyFont, int titleFontSize, int bodyFontSize, int buttonFontSize, Font buttonFont, Font returnFont = null)
    {
        if (titleFont != null)
            titleLabel.font = titleFont;

        if (titleFontSize != 0)
            titleLabel.fontSize = titleFontSize;
            
        if (buttonFontSize != 0)
            this.buttonFontSize = buttonFontSize;
        else
            this.buttonFontSize = bodyFontSize;

        if (buttonFont != null)
            this.buttonFont = buttonFont;
        else if(bodyFont != null)
            this.buttonFont = bodyFont;

        if (returnFont == null)
            this.returnFont = buttonFont;
        else
            this.returnFont = returnFont;
            
        titleLabel.color = titleColor;

        if (alignment == TextAlignment.Left)
            textAnchor = TextAnchor.MiddleLeft;
        else if (alignment == TextAlignment.Center)
            textAnchor = TextAnchor.MiddleCenter;
        else
            textAnchor = TextAnchor.MiddleRight;

    }


    public void StyleBackground(Sprite image, Color tint) {
        background.sprite = image;
        background.color = tint;
    }

    public void StyleContainer(Vector2Int referenceSize, float titleSeparator, float buttonSeparator, float titlePadding, float buttonsPadding, float verticalPadding, Sprite image, Color tint, float bleed) {
        gameObject.GetComponent<CanvasScaler>().referenceResolution = referenceSize;

        containerBG.sprite = image;
        containerBG.color = tint;

        titleSeparator /= 100f;
        buttonSeparator /= 100f;

        titlePadding /= 100f;
        buttonsPadding /= 100f;

        verticalPaddingRT.anchorMin = new Vector2(0, verticalPadding / 100f);
        verticalPaddingRT.anchorMax = new Vector2(1, 1f - verticalPadding / 100f);

        titleRT = titleLabel.GetComponent<RectTransform>();

        titleRT.anchorMin = new Vector2(titlePadding, titleSeparator);
        titleRT.anchorMax = new Vector2(1f - titlePadding, 1f);

        buttonsRT.anchorMin = new Vector2(buttonsPadding, 0);
        buttonsRT.anchorMax = new Vector2(1f - buttonsPadding, buttonSeparator);

        RectTransform rect = containerBG.GetComponent<RectTransform>();
        rect.offsetMin = new Vector2(-bleed, -bleed);
        rect.offsetMax = new Vector2( bleed,  bleed);
    }

    virtual public void SetReturn(GRP_PromptButtonOptions options)
    {
        GameObject buttonGO = returnButton.gameObject;

        buttonGO.transform.SetParent(buttonsRT.transform);

        if (options.text.Trim() != "")
        {
            buttonGO.GetComponentInChildren<Text>().text = options.text;
            buttonGO.transform.Find("Visual").GetComponent<Image>().color = Color.clear;
        }
        else if (options.image != null)
        {
            buttonGO.GetComponentInChildren<Text>().text = "";
            Image visual = buttonGO.transform.Find("Visual").GetComponent<Image>();
            visual.sprite = options.image;
            visual.color = Color.white;
        }
        else
        {
            buttonGO.GetComponentInChildren<Text>().text = "";
            buttonGO.transform.Find("Visual").GetComponent<Image>().color = Color.clear;
        }
    }

    public void PlaceFlexible(float width, float height) {
        inWorld = false;
        float widthPadding = (1f - width) / 2f;
        float heightPadding = (1f - height) / 2f;

        RectTransform rt = container.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(widthPadding, heightPadding);
        rt.anchorMax = new Vector2(1f - widthPadding, 1f - heightPadding);

        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    public void PlaceStatic(Anchor anchor, int x, int y, int width, int height)
    {
        inWorld = false;
        RectTransform rt = container.GetComponent<RectTransform>();

        if(anchor == Anchor.topLeft)            rt.anchorMin = rt.anchorMax = new Vector2( 0f,  1f);
        else if (anchor == Anchor.topCenter)    rt.anchorMin = rt.anchorMax = new Vector2(.5f,  1f);
        else if (anchor == Anchor.topRight)     rt.anchorMin = rt.anchorMax = new Vector2( 1f,  1f);

        else if (anchor == Anchor.middleLeft)   rt.anchorMin = rt.anchorMax = new Vector2( 0f, .5f);
        else if (anchor == Anchor.center)       rt.anchorMin = rt.anchorMax = new Vector2(.5f, .5f);
        else if (anchor == Anchor.middleRight)  rt.anchorMin = rt.anchorMax = new Vector2( 1f, .5f);

        else if (anchor == Anchor.bottomLeft)   rt.anchorMin = rt.anchorMax = new Vector2( 0f,  0f);
        else if (anchor == Anchor.bottomCenter) rt.anchorMin = rt.anchorMax = new Vector2(.5f,  0f);
        else if (anchor == Anchor.bottomRight)  rt.anchorMin = rt.anchorMax = new Vector2( 1f,  0f);

        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(width, height);
    }

    public void PlaceFullscreen(int top, int right, int bottom, int left)
    {
        inWorld = false;
        RectTransform rt = container.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;

        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
    }

    public void PlaceWorld(Vector3Bool followAxes, Vector3 pos, Vector3 rot, float scale, int width, int height)
    {
        inWorld = true;

        this.followAxes = new Vector3Bool(followAxes);

        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, height);
        rt.localScale = new Vector3(scale, scale, scale);
        rt.localEulerAngles = new Vector3(rot.x, rot.y, rot.z);
        rt.localPosition = new Vector3(pos.x, pos.y, pos.z);

        RectTransform crt = container.GetComponent<RectTransform>();
        crt.anchorMin = Vector2.zero;
        crt.anchorMax = Vector2.one;

        crt.offsetMin = crt.offsetMax = Vector2.zero;
        background.color = Color.clear;
    }

    public void SetFade(FadeAnimation type, float duration, AnimationCurve profile)
    {
        fadeType = type;
        fadeDur = duration;
        fadeProfile = profile;
    }
}
