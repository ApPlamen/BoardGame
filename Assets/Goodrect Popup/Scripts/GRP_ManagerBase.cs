using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum Placement { Fullscreen, Flexible, Static, World }
public enum Anchor { topLeft, topCenter, topRight, middleLeft, center, middleRight, bottomLeft, bottomCenter, bottomRight }

public enum ReturnPosition { First, Last }
public enum FadeAnimation { Expand, Shrink, Fade }
public enum ButtonDirection { Horizontal, Vertical }

public enum TextAlignment { Left, Center, Right }

public abstract class GRP_ManagerBase : MonoBehaviour
{
    public int containerPadding;
    public float titlePadding;
    public float messagePadding;
    public float buttonPadding;

    public Sprite containerImage;
    public int containerBleed;
    public Color containerTint;

    public Sprite buttonImage;
    public Sprite buttonPressedImage;

    public Sprite returnButtonImage;
    public Sprite returnButtonPressedImage;

    public bool buttonPrefixImage;
    public int buttonBleed;
    public float buttonSeparation;
    public Color returnButtonTint;
    public Color returnButtonPressedTint;

    public Sprite backgroundImage;
    public Color backgroundTint;

    public Font titleFont;
    public Font bodyFont;
    public Font buttonFont;

    public TextAlignment textAlignment;
    public int titleFontSize;
    public int bodyFontSize;
    public int buttonFontSize;

    public Color titleColor;
    public Color bodyTextColor;
    public Color returnTextColor;
    public Color returnTextColorPressed;

    public bool makeThisParent;
    public Transform container;
    public FadeAnimation fadeAnimation;
    public AnimationCurve fadeProfile;
    public float fadeDuration;
    public Vector2Int referenceSize;

    public string title;
    public string message;

    public GameObject popupPF;

    /* placement parameters */
    public Placement placement = Placement.Flexible;
    public int flexibleWidth = 80;
    public int flexibleHeight = 80;

    public int staticWidth = 400;
    public int staticHeight = 400;
    public int staticX = 0;
    public int staticY = 0;
    public float staticAnchorX = 0.5f;
    public float staticAnchorY = 0.5f;
    public Anchor staticAnchor = Anchor.center;

    public int worldWidth;
    public int worldHeight;
    public Vector3 worldPosition;
    public Vector3 worldRotation;
    public float worldScale = 0.01f;

    public Vector3Bool worldFollowAxes;

    public int fullscreenTop = 100;
    public int fullscreenRight = 100;
    public int fullscreenBottom = 100;
    public int fullscreenLeft = 100;

    public float titleSeparator = 0.8f;
    public float buttonSeparator = 0.6f;

    //inspector foldout states
    public bool showBehaviour = true;
    public bool showButtons = false;
    public bool showTypography = false;
    public bool showDesign = false;
    public bool showRatios = false;

    public bool initialize = true;

    //prompt & multichoice
    public Color generalButtonTint;
    public Color generalButtonPressedTint;

    public Font returnFont;

    public Color buttonTextColor;
    public Color buttonTextColorPressed;

    public Sprite promptInputFieldImage;
    public Color promptInputFieldTint;
    public Color promptInputfieldColor;
    public Color promptPlaceholderColor;

    public ReturnPosition returnPosition;
    public ButtonDirection buttonDirection;

    public PromptType promptType;

    // Start is called before the first frame update
    void Start()
    {
        if (makeThisParent)
            container = transform;
    }

    public void Create(bool fadeIn = true)
    {
        CreateAndReturn("", fadeIn);
    }

    public abstract GameObject CreateAndReturn(string name_suffix = "", bool fadeIn = false);
  
  
    public void Initialize(string titleText, string bodyText)
    {
        title = titleText;
        message = bodyText;
    }

    public abstract void ResetButtons();

    protected void Place(Placement placement, GRP_PopupBase popupBase)
    {
        if (placement == Placement.Static)
        {
            popupBase.PlaceStatic(staticAnchor, staticX, staticY, staticWidth, staticHeight);
        }
        else if (placement == Placement.Flexible)
        {
            popupBase.PlaceFlexible(flexibleWidth / 100f, flexibleHeight / 100f);
        }
        else if (placement == Placement.Fullscreen)
        {
            popupBase.PlaceFullscreen(fullscreenTop, fullscreenRight, fullscreenBottom, fullscreenLeft);
        }
        else if (placement == Placement.World)
        {
            popupBase.PlaceWorld(worldFollowAxes, worldPosition, worldRotation, worldScale, worldWidth, worldHeight);
        }
    }

    public void Match(GRP_ManagerBase source)
    {
        backgroundImage = source.backgroundImage;
        backgroundTint = source.backgroundTint;
        bodyFont = source.bodyFont;
        bodyFontSize = source.bodyFontSize;
        bodyTextColor = source.bodyTextColor;
        buttonBleed = source.buttonBleed;
        buttonDirection = source.buttonDirection;
        buttonFont = source.buttonFont;
        buttonFontSize = source.buttonFontSize;
        buttonImage = source.buttonImage;
        buttonPadding = source.buttonPadding;
        buttonPrefixImage = source.buttonPrefixImage;
        buttonPressedImage = source.buttonPressedImage;
        buttonSeparation = source.buttonSeparation;
        buttonSeparator = source.buttonSeparator;
        buttonTextColor = source.buttonTextColor;
        buttonTextColorPressed = source.buttonTextColorPressed;
        container = source.container;
        containerBleed = source.containerBleed;
        containerImage = source.containerImage;
        containerPadding = source.containerPadding;
        containerTint = source.containerTint;
        fadeAnimation = source.fadeAnimation;
        fadeDuration = source.fadeDuration;
        fadeProfile = source.fadeProfile;
        flexibleHeight = source.flexibleHeight;
        flexibleWidth = source.flexibleWidth;
        fullscreenBottom = source.fullscreenBottom;
        fullscreenLeft = source.fullscreenLeft;
        fullscreenRight = source.fullscreenRight;
        fullscreenTop = source.fullscreenTop;
        generalButtonPressedTint = source.generalButtonPressedTint;
        generalButtonTint = source.generalButtonTint;
        initialize = source.initialize;
        makeThisParent = source.makeThisParent;
        messagePadding = source.messagePadding;
        placement = source.placement;
        promptInputFieldImage = source.promptInputFieldImage;
        promptInputFieldTint = source.promptInputFieldTint;
        promptInputfieldColor = source.promptInputfieldColor;
        promptPlaceholderColor = source.promptPlaceholderColor;
        promptType = source.promptType;
        referenceSize = source.referenceSize;
        returnButtonImage = source.returnButtonImage;
        returnButtonPressedImage = source.returnButtonPressedImage;
        returnButtonPressedTint = source.returnButtonPressedTint;
        returnButtonTint = source.returnButtonTint;
        returnPosition = source.returnPosition;
        returnTextColor = source.returnTextColor;
        returnTextColorPressed = source.returnTextColorPressed;
        showBehaviour = source.showBehaviour;
        showButtons = source.showButtons;
        showDesign = source.showDesign;
        showRatios = source.showRatios;
        showTypography = source.showTypography;
        staticAnchor = source.staticAnchor;
        staticAnchorX = source.staticAnchorX;
        staticAnchorY = source.staticAnchorY;
        staticHeight = source.staticHeight;
        staticWidth = source.staticWidth;
        staticX = source.staticX;
        staticY = source.staticY;
        textAlignment = source.textAlignment;
        titleColor = source.titleColor;
        titleFont = source.titleFont;
        titleFontSize = source.titleFontSize;
        titlePadding = source.titlePadding;
        titleSeparator = source.titleSeparator;
        worldFollowAxes = source.worldFollowAxes;
        worldHeight = source.worldHeight;
        worldPosition = source.worldPosition;
        worldRotation = source.worldRotation;
        worldScale = source.worldScale;
        worldWidth = source.worldWidth;
    }
}

[System.Serializable]
public class GRP_PromptButtonOptions
{
    public string text;
    public Sprite image;
    public GRP_PromptEvent action;
    public bool foldoutState = false;

    public GRP_PromptButtonOptions()
    {
        text = "";
    }
};

[System.Serializable]
public class GRP_ButtonOptions
{
    public string text;
    public Sprite image;
    public GRP_Event action;
    public bool foldoutState = false;

    public GRP_ButtonOptions()
    {
        text = "";
    }
};


[System.Serializable]
public class GRP_PromptEvent: UnityEvent<string> {
    public List<UnityAction<string>> runtimePromptActions;
    public List<UnityAction> runtimeActions;

    public void AddRuntimeListener(UnityAction<string> listener) {
        if (runtimePromptActions == null) runtimePromptActions = new List<UnityAction<string>>();
        runtimePromptActions.Add(listener);
    }

    public void AddRuntimeListener(UnityAction listener) {
        if (runtimeActions == null) runtimeActions = new List<UnityAction>();
        runtimeActions.Add(listener);
    }
}

[System.Serializable]
public class GRP_Event : UnityEvent
{
    public List<UnityAction> runtimeActions;

    public void AddRuntimeListener(UnityAction listener)
    {
        if (runtimeActions == null) runtimeActions = new List<UnityAction>();
        runtimeActions.Add(listener);
    }
}

[System.Serializable]
public class Vector3Bool {
    public bool x;
    public bool y;
    public bool z;

    public Vector3Bool()
    {
        x = y = z = false;
    }

    public Vector3Bool(int x, int y, int z)
    {
        this.x = x != 0;
        this.y = y != 0;
        this.z = z != 0;
    }

    public Vector3Bool(bool x, bool y, bool z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Bool(Vector3Bool source)
    {
        x = source.x;
        y = source.y;
        z = source.z;
    }
}