using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(GRP_ManagerBase))]
public class GRP_ManagerBaseEditor : Editor
{
    protected GRP_ManagerBase rmb;

    protected int prevPreset = 0;

    protected bool showPresetNameField;
    protected string presetName;
    protected string savePresetButtonText;
    protected string presetFolderPath = "";

    protected string[] savedPresets;
    protected int presetIndex;

    protected Texture deleteTex;
    protected Texture upTex;
    protected Texture downTex;

    protected GUIStyle boxBG;
    protected GUIStyle bigButton;
    protected GUIStyle textAreastyle;

    protected Color padding = new Color(195f / 255, 207f / 255, 142f / 255);

    protected string type;
    protected GameObject lastPopup;

    private void PopulatePresetList() {
        DirectoryInfo info = new DirectoryInfo(presetFolderPath);
        FileInfo[] fileInfo = info.GetFiles();
        List<string> nameList = new List<string>();
        nameList.Add("Choose a Preset");

        foreach (FileInfo file in fileInfo)
        {
            string presetName = file.Name;

            if (presetName.EndsWith("grppreset", System.StringComparison.Ordinal))
                nameList.Add(presetName.Substring(0, presetName.Length - 10));
        }

        nameList.Add("Surprise Me");
        savedPresets = nameList.ToArray();
        presetIndex = 0;
    }

    protected void OnAwake(GRP_ManagerBase pm)
    {
        rmb = pm;

        savePresetButtonText = "Create a Preset";
        presetFolderPath = FindAbsolutePath("GRP_Presets");

        if(presetFolderPath == null)
        {
            AssetDatabase.CreateFolder("Assets", "GRP_Presets");
            presetFolderPath = "Assets/GRP_Presets";
            Debug.Log("Preset folder missing!");
            Debug.Log("New preset folder is created here:");
            Debug.Log(presetFolderPath);
        }

        PopulatePresetList();

        EditorStyles.foldout.fontStyle = FontStyle.Bold;
        deleteTex = (Texture)Resources.Load("Sprites/UI/bin");
        upTex = (Texture)Resources.Load("Sprites/UI/up");
        downTex = (Texture)Resources.Load("Sprites/UI/down");

        EditorStyles.textArea.wordWrap = true;

        if (rmb.initialize)
        {
            if(Camera.main == null)
                rmb.referenceSize = new Vector2Int(1920, 1080);
            else if (Camera.main.pixelWidth > Camera.main.pixelHeight)
                rmb.referenceSize = new Vector2Int(1920, 1080);
            else if (Camera.main.pixelWidth < Camera.main.pixelHeight)
                rmb.referenceSize = new Vector2Int(1080, 1920);
            else
                rmb.referenceSize = new Vector2Int(1080, 1080);

            rmb.worldFollowAxes = new Vector3Bool();

            rmb.title = "Hi there";
            rmb.message = "Seems like you prefer to do things manually.\n\nI like the cut of your jib.";

            //Manually load simple red.
            //Just in case.

            rmb.containerPadding = 5;
            rmb.titlePadding = 5;
            rmb.messagePadding = 5;
            rmb.buttonPadding = 5;

            rmb.backgroundTint = new Color(0, 0, 0, 0.4980392f);
            rmb.containerTint = new Color(1, 1, 1, 1);

            rmb.returnButtonTint = new Color(0.8823529f, 0, 0, 1);
            rmb.generalButtonTint = new Color(1, 1, 1, 0);
            rmb.returnButtonPressedTint = new Color(0.7058824f, 0, 0, 1);
            rmb.generalButtonPressedTint = new Color(0, 0, 0, 0.05882353f);

            rmb.titleColor = new Color(0, 0, 0, 0.9215f);
            rmb.bodyTextColor = new Color(0, 0, 0, 0.9215f);
            rmb.buttonTextColor = new Color(0, 0, 0, 0.9215f);
            rmb.returnTextColor = new Color(1, 1, 1, 0.8823529f);
            rmb.buttonTextColorPressed = new Color(0, 0, 0, 1);
            rmb.returnTextColorPressed = new Color(1, 1, 1, 0.8823529f);

            rmb.promptPlaceholderColor = new Color(0, 0, 0, 0.4f);
            rmb.promptInputfieldColor = new Color(0, 0, 0, 0.9215f);
            rmb.promptInputFieldTint = new Color(0, 0, 0, 0.1f);

            rmb.titleFontSize = 60;
            rmb.bodyFontSize = 36;
            rmb.buttonFontSize = 36;

            rmb.textAlignment = TextAlignment.Center;

            rmb.fadeDuration = 0.25f;
            rmb.fadeProfile = AnimationCurve.Linear(0, 0, 1, 1);
            rmb.fadeAnimation = FadeAnimation.Fade;

            rmb.initialize = false;
        }
    }

    protected void CreateStyles()
    {
        boxBG = new GUIStyle();
        boxBG.normal.background = ColorTexture(1, 1, 1, 0.5f);
        boxBG.padding = new RectOffset(10, 10, 10, 10);

        bigButton = new GUIStyle(GUI.skin.button);
        bigButton.active.textColor = new Color(0, 0, 0, 1f);
        bigButton.active.background = ColorTexture(1, 1, 1, 1);
        bigButton.fontStyle = FontStyle.Bold;
        bigButton.fontSize = 14;
        bigButton.fixedHeight = 30;

        bigButton.normal.background = ColorTexture(rmb.returnButtonTint.r, rmb.returnButtonTint.g, rmb.returnButtonTint.b, 1);
        bigButton.normal.textColor = VisibleTextFailsafe(rmb.returnButtonTint, rmb.returnTextColor);

        textAreastyle = new GUIStyle(EditorStyles.textArea);
        textAreastyle.wordWrap = true;
    }

    public override void OnInspectorGUI()
    {
        CreateStyles();

        string generateButtonText;

        if (lastPopup == null)
            generateButtonText = "Generate " + type;
        else
            generateButtonText = "Update " + type;

        if (GUILayout.Button(generateButtonText, bigButton))
        {
            if (lastPopup != null)
                DestroyImmediate(lastPopup);

            lastPopup = rmb.CreateAndReturn();
        }

        if (lastPopup != null)
        {
            if (GUILayout.Button("Delete " + type, bigButton))
            {
                if (lastPopup != null)
                    DestroyImmediate(lastPopup);
            }
        }
    }

    protected Color VisibleTextFailsafe(Color bg, Color text) {

        if (Mathf.Abs(bg.r - text.r) < 0.01f &&
            Mathf.Abs(bg.g - text.g) < 0.01f &&
            Mathf.Abs(bg.b - text.b) < 0.01f)
        {
            if (bg.grayscale < 0.5f)
                return Color.white;

            return Color.black;
        }

        return text;
    }

    protected void BehaviourGroup(bool isPopup = false)
    {
        rmb.showBehaviour = EditorGUILayout.Foldout(rmb.showBehaviour, "Canvas Behaviour");

        if (rmb.showBehaviour)
        {
            GameObjectField(ref rmb.popupPF, "Popup Prefab");

            if (rmb.placement == Placement.World)
                EditorGUI.BeginDisabledGroup(true);

            rmb.referenceSize = EditorGUILayout.Vector2IntField("Reference Screen Size", rmb.referenceSize);

            if (rmb.placement == Placement.World)
                EditorGUI.EndDisabledGroup();

            BoolField(ref rmb.makeThisParent, "Make This Parent");
            if (!rmb.makeThisParent)
                TransformField(ref rmb.container, "Parent");

            rmb.fadeAnimation = (FadeAnimation)EditorGUILayout.EnumPopup("Fade Type", rmb.fadeAnimation);

            CurveField(ref rmb.fadeProfile, "Animation Profile");
            FloatField(ref rmb.fadeDuration, "Fade Duration");

            EditorGUILayout.Space();

            if(isPopup)
            {
                rmb.returnPosition = (ReturnPosition)EditorGUILayout.EnumPopup("Return Position", rmb.returnPosition);
                rmb.buttonDirection = (ButtonDirection)EditorGUILayout.EnumPopup("Button Direction", rmb.buttonDirection);

                EditorGUILayout.Space();
            } else
            {
                rmb.promptType = (PromptType)EditorGUILayout.EnumPopup("Prompt Type", rmb.promptType);

                EditorGUILayout.Space();
            }


            EditorGUILayout.BeginVertical(boxBG);

            rmb.placement = (Placement)EditorGUILayout.EnumPopup("Placement Mode", rmb.placement);

            EditorGUILayout.Space();

            if (rmb.placement == Placement.Static)
            {
                rmb.staticAnchor = (Anchor)EditorGUILayout.EnumPopup("Anchor Position", rmb.staticAnchor);

                EditorGUILayout.LabelField("X Position");
                rmb.staticX = EditorGUILayout.IntField(rmb.staticX);

                EditorGUILayout.LabelField("Y Position");
                rmb.staticY = EditorGUILayout.IntField(rmb.staticY);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Width (pixels)");
                rmb.staticWidth = EditorGUILayout.IntField(rmb.staticWidth);

                EditorGUILayout.LabelField("Height (pixels)");
                rmb.staticHeight = EditorGUILayout.IntField(rmb.staticHeight);
            }
            else if (rmb.placement == Placement.Flexible)
            {
                RangeFieldInt(ref rmb.flexibleWidth, "Width Percent", 10, 100);
                RangeFieldInt(ref rmb.flexibleHeight, "Height Percent", 10, 100);
            }
            else if (rmb.placement == Placement.Fullscreen)
            {
                EditorGUILayout.LabelField("Top Padding");
                rmb.fullscreenTop = EditorGUILayout.IntField(rmb.fullscreenTop);

                EditorGUILayout.LabelField("Right Padding");
                rmb.fullscreenRight = EditorGUILayout.IntField(rmb.fullscreenRight);

                EditorGUILayout.LabelField("Bottom Padding");
                rmb.fullscreenBottom = EditorGUILayout.IntField(rmb.fullscreenBottom);

                EditorGUILayout.LabelField("Left Padding");
                rmb.fullscreenLeft = EditorGUILayout.IntField(rmb.fullscreenLeft);
            }
            else if (rmb.placement == Placement.World)
            {
                IntField(ref rmb.worldWidth, "Width");
                IntField(ref rmb.worldHeight, "Height");

                rmb.worldPosition = EditorGUILayout.Vector3Field("Local Position", rmb.worldPosition);
                rmb.worldRotation = EditorGUILayout.Vector3Field("Local Rotation", rmb.worldRotation);
                rmb.worldScale = EditorGUILayout.FloatField("Local Scale", rmb.worldScale);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Follow Axes:");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("X:", GUILayout.Width(20));
                rmb.worldFollowAxes.x = EditorGUILayout.Toggle(rmb.worldFollowAxes.x);
                EditorGUILayout.LabelField("Y:", GUILayout.Width(20));
                rmb.worldFollowAxes.y = EditorGUILayout.Toggle(rmb.worldFollowAxes.y);
                EditorGUILayout.LabelField("Z:", GUILayout.Width(20));
                rmb.worldFollowAxes.z = EditorGUILayout.Toggle(rmb.worldFollowAxes.z);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }
    /* HELPERS */
    protected void RangeField(ref float val, string title, float min, float max)
    {
        EditorGUILayout.LabelField(title);
        val = EditorGUILayout.Slider(val, min, max);
    }

    protected void RangeFieldInt(ref int val, string title, int min, int max)
    {
        EditorGUILayout.LabelField(title);
        val = EditorGUILayout.IntSlider(val, min, max);
    }

    protected void SpriteField(ref Sprite sprite, string title)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        sprite = (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), true);
        EditorGUILayout.EndHorizontal();
    }

    protected void FontField(ref Font font, string title)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        font = (Font)EditorGUILayout.ObjectField(font, typeof(Font), true);
        EditorGUILayout.EndHorizontal();
    }

    protected void FloatField(ref float val, string title)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        val = EditorGUILayout.FloatField(val);
        EditorGUILayout.EndHorizontal();
    }

    protected void IntField(ref int val, string title, bool isUnsigned = false)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        val = EditorGUILayout.IntField(val);
        EditorGUILayout.EndHorizontal();
        if (val < 0 && isUnsigned) val = 0;
    }

    protected void ColorField(ref Color color, string title)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        color = EditorGUILayout.ColorField(color);
        EditorGUILayout.EndHorizontal();
    }

    protected void BoolField(ref bool val, string title)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        val = EditorGUILayout.Toggle(val);
        EditorGUILayout.EndHorizontal();
    }

    protected void TransformField(ref Transform transform, string title)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        transform = (Transform)EditorGUILayout.ObjectField(transform, typeof(Transform), true);
        EditorGUILayout.EndHorizontal();
    }

    protected void CurveField(ref AnimationCurve curve, string title)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        curve = EditorGUILayout.CurveField(curve);
        EditorGUILayout.EndHorizontal();
    }

    protected void GameObjectField(ref GameObject gameObject, string title)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
    }

    protected void StringField(ref string val, string title, bool multiline = false, bool inline = false)
    {
        GUIStyle textAreastyle = new GUIStyle(EditorStyles.textArea);
        textAreastyle.wordWrap = true;

        if (inline) EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(title);
        if (multiline)
            val = EditorGUILayout.TextArea(val, textAreastyle);
        else
            val = EditorGUILayout.TextField(val);
        if (inline) EditorGUILayout.EndHorizontal();
    }

    protected static void Line(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        EditorGUI.DrawRect(r, color);
    }

    protected static void PaddingPreview(Color color, float height)
    {
        Color paddingBorder = new Color(1 - color.r, 1 - color.g, 1 - color.b);
        paddingBorder = Color.Lerp(paddingBorder, color, .7f);

        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(height));
        r.height = height;

        r.y -= 1;
        r.height += 2;

        EditorGUI.DrawRect(r, paddingBorder);

        EditorGUI.DrawRect(r, color);
    }


    protected Texture2D ColorTexture(float r, float g, float b, float a)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(1, 1, new Color(r, g, b, a));
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        return tex;
    }

    /* preview */
    public void SectionPreview(Color color, float innerPadding, float height, string text, Color textColor, bool inputfield = false, float ifPadding = 0)
    {
        if (text == null)
            text = "";

        if (text.Length > 15)
            text = text.Substring(0, 15) + "[...]";

        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(height));
        r.height = height;

        r.y -= 1;
        r.height += 2;

        Color paddingBorder = new Color(1 - color.r, 1 - color.g, 1 - color.b);
        paddingBorder = Color.Lerp(paddingBorder, color, .7f);

        EditorGUI.DrawRect(r, color);

        r.x += r.width * innerPadding;
        r.width *= (1f - innerPadding * 2);

        if (inputfield) {
            ifPadding /= 100;
            r.y += r.height * ifPadding;
            r.height *= (1f - ifPadding * 2);
        }

        EditorGUI.DrawRect(r, paddingBorder);

        if(inputfield) {
            r.y += 1;
            r.height -= 2;
        }

        r.x += 1;
        r.width -= 2;
        EditorGUI.DrawRect(r, color);

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = VisibleTextFailsafe(color, textColor);
        style.wordWrap = true;
        style.clipping = TextClipping.Clip;

        if (rmb.textAlignment == TextAlignment.Left)
            style.alignment = TextAnchor.MiddleLeft;
        else if (rmb.textAlignment == TextAlignment.Center)
            style.alignment = TextAnchor.MiddleCenter;
        else
            style.alignment = TextAnchor.MiddleRight;


        if (height < 12)
            style.fontSize = (int)height;

        EditorGUI.LabelField(r, text, style);
    }

    /* IO */
    protected string FindAbsolutePath(string filename)
    {
        if (filename == null)
            return null;

        string[] allPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string p in allPaths)
        {
            if (p.EndsWith(filename, System.StringComparison.Ordinal))
                return p;
        }

        return null;
    }

    protected float R(float min = 0f)
    {
        return Random.Range(min, 1f);
    }

    protected void LoadPreset(int index)
    {
        if (index == 0)
            return;

        if (index == savedPresets.Length - 1)
        {
            rmb.backgroundTint = new Color(R(), R(), R(), R());
            rmb.containerTint = new Color(R(), R(), R());

            rmb.returnButtonTint = new Color(R(), R(), R());
            rmb.generalButtonTint = new Color(R(), R(), R());
            rmb.returnButtonPressedTint = new Color(R(), R(), R());
            rmb.generalButtonPressedTint = new Color(R(), R(), R());

            rmb.titleColor = new Color(R(), R(), R());
            rmb.bodyTextColor = new Color(R(), R(), R());
            rmb.buttonTextColor = new Color(R(), R(), R());
            rmb.returnTextColor = new Color(R(), R(), R());
            rmb.buttonTextColorPressed = new Color(R(), R(), R());
            rmb.returnTextColorPressed = new Color(R(), R(), R());

            presetIndex = 0;
            return;
        }

        if (index >= savedPresets.Length)
            return;

        string filename = presetFolderPath + "/" + savedPresets[index] + ".grppreset";
        StreamReader sr = new StreamReader(filename);

        GRP_PresetPackager pp = JsonUtility.FromJson<GRP_PresetPackager>(sr.ReadToEnd());

        rmb.backgroundTint = pp.bg_tint;
        rmb.containerTint = pp.container_tint;

        rmb.returnButtonTint = pp.return_tint;
        rmb.generalButtonTint = pp.btn_tint;
        rmb.returnButtonPressedTint = pp.return_pressed_tint;
        rmb.generalButtonPressedTint = pp.btn_pressed_tint;

        rmb.titleColor = pp.title_text_color;
        rmb.bodyTextColor = pp.body_text_color;
        rmb.buttonTextColor = pp.btn_text_color;
        rmb.returnTextColor = pp.return_text_color;
        rmb.buttonTextColorPressed = pp.btn_text_pressed_color;
        rmb.returnTextColorPressed = pp.return_text_pressed_color;
        rmb.promptInputfieldColor = pp.prompt_inputfield_text_color;
        rmb.promptPlaceholderColor = pp.prompt_placeholder_text_color;

        rmb.backgroundImage = AssetDatabase.LoadAssetAtPath<Sprite>(FindAbsolutePath(pp.bg_image));
        rmb.containerImage = AssetDatabase.LoadAssetAtPath<Sprite>(FindAbsolutePath(pp.container_image));
        rmb.promptInputFieldImage = AssetDatabase.LoadAssetAtPath<Sprite>(FindAbsolutePath(pp.prompt_inputfield_image));
        rmb.promptInputFieldTint = pp.prompt_inputfield_tint;
        rmb.buttonPrefixImage = pp.prefix_image;
        rmb.buttonImage = AssetDatabase.LoadAssetAtPath<Sprite>(FindAbsolutePath(pp.btn_image));
        rmb.buttonPressedImage = AssetDatabase.LoadAssetAtPath<Sprite>(FindAbsolutePath(pp.btn_pressed_image));
        rmb.returnButtonImage = AssetDatabase.LoadAssetAtPath<Sprite>(FindAbsolutePath(pp.rtn_btn_image));
        rmb.returnButtonPressedImage = AssetDatabase.LoadAssetAtPath<Sprite>(FindAbsolutePath(pp.rtn_btn_pressed_image));

        rmb.containerBleed = pp.container_bleed;
        rmb.buttonBleed = pp.btn_bleed;

        rmb.titleFontSize = pp.title_text_size;
        rmb.bodyFontSize = pp.body_text_size;
        rmb.buttonFontSize = pp.button_text_size;

        rmb.titleFont = AssetDatabase.LoadAssetAtPath<Font>(FindAbsolutePath(pp.title_font));
        rmb.bodyFont = AssetDatabase.LoadAssetAtPath<Font>(FindAbsolutePath(pp.body_font));
        rmb.buttonFont = AssetDatabase.LoadAssetAtPath<Font>(FindAbsolutePath(pp.button_font));
        rmb.returnFont = AssetDatabase.LoadAssetAtPath<Font>(FindAbsolutePath(pp.return_font));
        rmb.textAlignment = pp.alignment;

        rmb.fadeAnimation = pp.fade_animation;
        rmb.fadeProfile = pp.fade_curve;
        rmb.fadeDuration = pp.fade_dur;

        if (rmb.returnButtonImage == null)
            rmb.returnButtonImage = rmb.buttonImage;

        if (rmb.returnButtonPressedImage == null)
            rmb.returnButtonPressedImage = rmb.buttonPressedImage;

    }

    protected Color StringToColor(string line)
    {
        string[] val = line.Split(',');
        return new Color(int.Parse(val[0]) / 255f, int.Parse(val[1]) / 255f, int.Parse(val[2]) / 255f, int.Parse(val[3]) / 255f);
    }

    protected string ColorToString(Color color)
    {
        int r = (int)(color.r * 255);
        int g = (int)(color.g * 255);
        int b = (int)(color.b * 255);
        int a = (int)(color.a * 255);

        return r + "," + g + "," + b + "," + a;
    }

    protected string EncodePreset()
    {
        GRP_PresetPackager pp = new GRP_PresetPackager();

        pp.bg_tint = rmb.backgroundTint;
        pp.container_tint = rmb.containerTint;

        pp.return_tint = rmb.returnButtonTint;
        pp.btn_tint = rmb.generalButtonTint;
        pp.return_pressed_tint = rmb.returnButtonPressedTint;
        pp.btn_pressed_tint = rmb.generalButtonPressedTint;

        pp.title_text_color = rmb.titleColor;
        pp.body_text_color = rmb.bodyTextColor;
        pp.btn_text_color = rmb.buttonTextColor;
        pp.return_text_color = rmb.returnTextColor;
        pp.btn_text_pressed_color = rmb.buttonTextColorPressed;
        pp.return_text_pressed_color = rmb.returnTextColorPressed;
        pp.prompt_inputfield_text_color = rmb.promptInputfieldColor;
        pp.prompt_placeholder_text_color = rmb.promptPlaceholderColor;

        pp.bg_image = RelativeAssetPath(rmb.backgroundImage, "");
        pp.container_image = RelativeAssetPath(rmb.containerImage, "");
        pp.prompt_inputfield_image = RelativeAssetPath(rmb.promptInputFieldImage, "");
        pp.prompt_inputfield_tint = rmb.promptInputFieldTint;
        pp.prefix_image = rmb.buttonPrefixImage;
        pp.btn_image = RelativeAssetPath(rmb.buttonImage, "");
        pp.btn_pressed_image = RelativeAssetPath(rmb.buttonPressedImage, "");
        pp.rtn_btn_image = RelativeAssetPath(rmb.returnButtonImage, "");
        pp.rtn_btn_pressed_image = RelativeAssetPath(rmb.returnButtonPressedImage, "");
        pp.container_bleed = rmb.containerBleed;
        pp.btn_bleed = rmb.buttonBleed;

        pp.title_text_size = rmb.titleFontSize;
        pp.body_text_size = rmb.bodyFontSize;
        pp.button_text_size = rmb.buttonFontSize;

        pp.title_font = RelativeAssetPath(rmb.titleFont, "");
        pp.body_font = RelativeAssetPath(rmb.bodyFont, "");
        pp.button_font = RelativeAssetPath(rmb.buttonFont, "");
        pp.return_font = RelativeAssetPath(rmb.returnFont, "");

        pp.alignment = rmb.textAlignment;

        pp.fade_animation = rmb.fadeAnimation;
        pp.fade_curve = rmb.fadeProfile;
        pp.fade_dur = rmb.fadeDuration;

        string encoded = JsonUtility.ToJson(pp);

        return encoded;
    }

    protected string RelativeAssetPath(Object obj, string relativePath)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        string[] pathSplit = path.Split('/');
        string filename = pathSplit[pathSplit.Length - 1];

        return filename;
    }

    protected void SavePresetAction()
    {
        if (showPresetNameField && presetName != "")
        {
            string path = FindAbsolutePath("GRP_Presets");
            if (path == null)
                return;

            path = path + "/" + presetName + ".grppreset";

            StreamWriter writer = new StreamWriter(path, false);
            string encoded = EncodePreset();
            writer.WriteLine(encoded);
            writer.Close();

            Debug.Log("Preset saved to " + path);
            showPresetNameField = false;
            AssetDatabase.Refresh();
            PopulatePresetList();
        }
        else
        {
            savePresetButtonText = "Save Preset";
            showPresetNameField = true;
            presetName = "";
        }
    }
}
