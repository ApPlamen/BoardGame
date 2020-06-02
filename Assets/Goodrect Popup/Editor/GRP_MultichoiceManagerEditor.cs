using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GRP_MultichoiceManager))]
public class GRP_MultichoiceManagerEditor : GRP_ManagerBaseEditor
{
    GRP_MultichoiceManager rm;

    private void Awake()
    {
        base.type = "Multichoice";
        rm = target as GRP_MultichoiceManager;
        if (rm.popupPF == null)
            rm.popupPF = AssetDatabase.LoadAssetAtPath<GameObject>(FindAbsolutePath("GRP_Multichoice.prefab"));

        if(rm.initialize)
        {
            rm.buttonSeparator = 20;
            rm.titleSeparator = 80;

            rm.returnButton = new GRP_ButtonOptions();
            rm.returnButton.text = "You get me";
        }

        OnAwake(rm);
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PresetGroup();

        EditorGUILayout.Space();

        if(GUILayout.Button("Collapse All")) {
            rm.showBehaviour = false;
            rm.showRatios = false;
            rm.showDesign = false;
            rm.showTypography = false;
            rm.showButtons = false;
        }

        BehaviourGroup(true);
        RatioGroup();
        DesignGroup();
        TypographyGroup();
        ButtonsGroup();
    }


    /* CONTROL GROUPS */
    private void PresetGroup()
    {
        EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);

        if (showPresetNameField)
        {
            presetName = GUILayout.TextField(presetName);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(savePresetButtonText))
                SavePresetAction();

            if (GUILayout.Button("Cancel"))
            {
                showPresetNameField = false;
                savePresetButtonText = "Create a Preset";
            }

            GUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button(savePresetButtonText))
                SavePresetAction();
        }

        EditorGUILayout.Space();
        presetIndex = EditorGUILayout.Popup("Load Preset:", presetIndex, savedPresets);

        if (GUI.changed) {
            if (presetIndex != prevPreset)
                LoadPreset(presetIndex);
            else
                presetIndex = 0;
        }

        prevPreset = presetIndex;

    }

    private void DesignGroup()
    {
        rm.showDesign = EditorGUILayout.Foldout(rm.showDesign, "Colors & Textures");

        if(rm.showDesign)
        {
            SpriteField(ref rm.containerImage, "Container Image");
            IntField(ref rm.containerBleed, "Container Bleed (px)");
            ColorField(ref rm.containerTint, "ContainerTint");

            EditorGUILayout.Space();

            BoolField(ref rm.buttonPrefixImage, "Prefix Image");
            SpriteField(ref rm.buttonImage, "Button Image");
            ColorField(ref rm.generalButtonTint, "Button Tint");
            SpriteField(ref rm.buttonPressedImage, "Button Pressed Image");
            ColorField(ref rm.generalButtonPressedTint, "Button Pressed Tint");

            EditorGUILayout.Space();

            SpriteField(ref rm.returnButtonImage, "Return Button Image");
            ColorField(ref rm.returnButtonTint, "Return Button Tint");
            SpriteField(ref rm.returnButtonPressedImage, "Return B. Pressed Image");
            ColorField(ref rm.returnButtonPressedTint, "Return B. Pressed Tint");

            EditorGUILayout.Space();

            IntField(ref rm.buttonBleed, "Button Bleed (px)");
            SpriteField(ref rm.backgroundImage, "Background Image");
            ColorField(ref rm.backgroundTint, "Background Tint");
        }
    }

    private void TypographyGroup() {
        rm.showTypography = EditorGUILayout.Foldout(rm.showTypography, "Typography & Text");

        if(rm.showTypography)
        {
            StringField(ref rm.title, "Multichoice Title");
            StringField(ref rm.message, "Multichoice Message", true);

            EditorGUILayout.Space();

            FontField(ref rm.titleFont, "Title Font");
            FontField(ref rm.bodyFont, "Body Font");
            FontField(ref rm.buttonFont, "Button Font");
            FontField(ref rm.returnFont, "Return Button Font");

            EditorGUILayout.Space();

            IntField(ref rm.titleFontSize, "Title Font Size", true);
            IntField(ref rm.bodyFontSize, "Body Font Size", true);
            IntField(ref rm.buttonFontSize, "Button Font Size", true);

            EditorGUILayout.Space();

            ColorField(ref rm.titleColor, "Title Color");
            ColorField(ref rm.bodyTextColor, "Body Text Color");
            ColorField(ref rm.buttonTextColor, "Button Text Color");
            ColorField(ref rm.buttonTextColorPressed, "B. Text Color Pressed");
            ColorField(ref rm.returnTextColor, "Return Text Color");
            ColorField(ref rm.returnTextColorPressed, "R. Text Color Pressed");

            EditorGUILayout.Space();

            rm.textAlignment = (TextAlignment)EditorGUILayout.EnumPopup("Text Alignment", rm.textAlignment);
        }
    }

    private void RatioGroup()
    {
        rm.showRatios = EditorGUILayout.Foldout(rm.showRatios, "Inner Ratios");
        if (rm.showRatios) {
            RangeFieldInt(ref rm.containerPadding, "Container Padding (Percent)", 0, 45);
            RangeField(ref rm.titlePadding, "Title Padding (Percent)", 0, 45);
            RangeField(ref rm.messagePadding, "Message Padding (Percent)", 0, 45);
            RangeField(ref rm.buttonPadding, "Button Padding (Percent)", 0, 45);
            RangeField(ref rm.buttonSeparation, "Button Separation (Percent)", 0, 45);

            EditorGUILayout.Space();

            RangeField(ref rm.titleSeparator, "Title Separator Position (Percent)", 50f, 100f);
            RangeField(ref rm.buttonSeparator, "Button Separator Position (Percent)", 10f, 80f);

            float stretch = 2f;

            if (rm.buttonSeparator > rm.titleSeparator)
                rm.titleSeparator = rm.buttonSeparator;

            int contentHeight = (int)(stretch * (100 - rm.containerPadding * 2));

            int titlePreview = (int)(contentHeight * (1 - rm.titleSeparator / 100f));
            int buttonPreview = (int)(contentHeight * rm.buttonSeparator / 100f);
            int messagePreview = contentHeight - (titlePreview + buttonPreview);

            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Preview only shows tints");
            EditorGUILayout.LabelField("Texture colors do not show below");

            PaddingPreview(rm.containerTint, (int)(stretch * rm.containerPadding));
            SectionPreview(rm.containerTint, rm.titlePadding / 100.0f, titlePreview, rm.title, rm.titleColor);
            SectionPreview(rm.containerTint, rm.messagePadding / 100.0f, messagePreview, rm.message, rm.bodyTextColor);
            ButtonSectionPreview(rm.containerTint, rm.buttonPadding / 100.0f, buttonPreview);
            PaddingPreview(rm.containerTint, (int)(stretch * rm.containerPadding));
        }
    }

    private void ButtonsGroup()
    {
        rm.showButtons = EditorGUILayout.Foldout(rm.showButtons, "Buttons & Actions");

        if(rm.showButtons)
        {
            if(GUILayout.Button("Collapse All")) {
                for (int i = 0; i < rm.buttonList.Count; i++)
                    rm.buttonList[i].foldoutState = false;

                rm.returnButton.foldoutState = false;
            }

            if (rm.buttonList == null)
                rm.buttonList = new List<GRP_ButtonOptions>();

            if (rm.returnButton == null)
                rm.returnButton = new GRP_ButtonOptions();

            SerializedProperty buttonList = serializedObject.FindProperty("buttonList");

            EditorGUI.indentLevel++;
            for (int i = 0; rm.buttonList != null && i < rm.buttonList.Count; i++)
            {
                string title = rm.buttonList[i].text == null || rm.buttonList[i].text.Length == 0 ? ("Button #" + (i + 1)) : rm.buttonList[i].text;

                EditorGUILayout.BeginHorizontal();
                rm.buttonList[i].foldoutState = EditorGUILayout.Foldout(rm.buttonList[i].foldoutState, title);
                if (GUILayout.Button(upTex, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    if (i > 0)
                    {
                        GRP_ButtonOptions buttonOptions = rm.buttonList[i];
                        rm.buttonList.RemoveAt(i);
                        rm.buttonList.Insert(i - 1, buttonOptions);
                        return;
                    }
                }

                if (GUILayout.Button(downTex, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    if (i < rm.buttonList.Count - 1)
                    {
                        GRP_ButtonOptions buttonOptions = rm.buttonList[i];
                        rm.buttonList.RemoveAt(i);
                        rm.buttonList.Insert(i + 1, buttonOptions);
                        return;
                    }
                }
                if (GUILayout.Button(deleteTex, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    rm.buttonList.RemoveAt(i);
                    return;
                }
                EditorGUILayout.EndHorizontal();

                if (rm.buttonList[i].foldoutState)
                {
                    StringField(ref rm.buttonList[i].text, "Button Title");

                    if (rm.buttonList[i].text.Trim() == "")
                        SpriteField(ref rm.buttonList[i].image, "Button Visual");

                    SerializedProperty buttonListElement = buttonList.GetArrayElementAtIndex(i);
                    if (buttonListElement == null)
                        return;

                    EditorGUILayout.PropertyField(buttonListElement.FindPropertyRelative("action"), true);

                    Line(new Color(0, 0, 0, 0.25f), 1, 5);
                    Rect rect = EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.EndHorizontal();
                }

                if(i < rm.buttonList.Count-1)
                    Line(new Color(0, 0, 0, 0.5f), 1, 5);
            }

            Line(new Color(0, 0, 0, 0.5f));

            bool singleButton = rm.buttonList.Count == 0;

            EditorGUI.BeginDisabledGroup(singleButton);
            BoolField(ref rm.showReturn, "Show Return Button");
            EditorGUI.EndDisabledGroup();

            if(singleButton)
                rm.showReturn = true;

            EditorGUI.BeginDisabledGroup(rm.showReturn == false);

            rm.returnButton.foldoutState = EditorGUILayout.Foldout(rm.returnButton.foldoutState, "Return Button");

            if(rm.returnButton.foldoutState) {
                StringField(ref rm.returnButton.text, "Return Title");

                if (rm.returnButton.text.Trim() == "")
                    SpriteField(ref rm.returnButton.image, "Return Visual");

                EditorGUILayout.PropertyField(serializedObject.FindProperty("returnButton").FindPropertyRelative("action"), true);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;

            Line(new Color(0, 0, 0, 0.5f));

            if (GUILayout.Button("Add Button"))
            {
                buttonList.arraySize++;
                serializedObject.ApplyModifiedProperties();
                rm.buttonList[rm.buttonList.Count - 1] = new GRP_ButtonOptions();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    public void ButtonSectionPreview(Color color, float innerPadding, float height)
    {
        if (rm.buttonList == null)
            rm.buttonList = new List<GRP_ButtonOptions>();

        Color paddingBorder = new Color(1 - color.r, 1 - color.g, 1 - color.b);
        paddingBorder = Color.Lerp(paddingBorder, color, .7f);

        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(height));
        r.height = height;

        r.y -= 1;
        r.height += 2;

        EditorGUI.DrawRect(r, color);

        r.x += r.width * innerPadding;
        r.width *= (1f - innerPadding * 2);
        EditorGUI.DrawRect(r, paddingBorder);

        r.x += 1;
        r.width -= 2;
        EditorGUI.DrawRect(r, color);

        float buttonSize;

        if (rm.buttonDirection == ButtonDirection.Vertical)
            buttonSize = height / (rm.buttonList.Count + 1);
        else
            buttonSize = r.width / (rm.buttonList.Count + 1);


        GUIStyle style = new GUIStyle();
        if(rm.textAlignment == TextAlignment.Left)
            style.alignment = TextAnchor.MiddleLeft;
        else if (rm.textAlignment == TextAlignment.Center)
            style.alignment = TextAnchor.MiddleCenter;
        else
            style.alignment = TextAnchor.MiddleRight;

        style.wordWrap = true;
        style.clipping = TextClipping.Clip;
        if (buttonSize * (1 - rm.buttonSeparation * 2 / 100) < 12)
            style.fontSize = (int)(buttonSize * (1 - rm.buttonSeparation * 2 / 100));

        if(rm.buttonDirection == ButtonDirection.Vertical)
            ButtonPreviewVertical(r, style, buttonSize);
        else
            ButtonPreviewHorizontal(r, style, buttonSize);
    }

    private void ButtonPreviewHorizontal(Rect r, GUIStyle style, float buttonWidth)
    {
        r.width = buttonWidth;

        Rect buttonRect = new Rect(r);
        r.x += r.width * rm.buttonSeparation / 100;
        r.width *= (1 - rm.buttonSeparation * 2 / 100);

        if (rm.returnPosition == ReturnPosition.First)
        {
            style.normal.textColor = VisibleTextFailsafe(rm.returnButtonTint, rm.returnTextColor);

            EditorGUI.DrawRect(r, rm.returnButtonTint);
            EditorGUI.LabelField(r, rm.returnButton.text, style);
            r.x += buttonWidth;
        }

        style.normal.textColor = VisibleTextFailsafe(rm.generalButtonTint, rm.buttonTextColor);
        for (int i = 0; i < rm.buttonList.Count; i++)
        {
            EditorGUI.DrawRect(r, rm.generalButtonTint);
            EditorGUI.LabelField(r, rm.buttonList[i].text, style);
            r.x += buttonWidth;
        }

        if (rm.returnPosition == ReturnPosition.Last)
        {
            style.normal.textColor = VisibleTextFailsafe(rm.returnButtonTint, rm.returnTextColor);

            EditorGUI.DrawRect(r, rm.returnButtonTint);
            EditorGUI.LabelField(r, rm.returnButton.text, style);
        }
    }

    private void ButtonPreviewVertical(Rect r, GUIStyle style, float buttonHeight)
    {
        r.height = buttonHeight;

        Rect buttonRect = new Rect(r);
        r.y += r.height * rm.buttonSeparation / 100;
        r.height *= (1 - rm.buttonSeparation * 2 / 100);

        if (rm.returnPosition == ReturnPosition.First)
        {
            style.normal.textColor = VisibleTextFailsafe(rm.returnButtonTint, rm.returnTextColor);

            EditorGUI.DrawRect(r, rm.returnButtonTint);
            EditorGUI.LabelField(r, rm.returnButton.text, style);
            r.y += buttonHeight;
        }

        style.normal.textColor = VisibleTextFailsafe(rm.generalButtonTint, rm.buttonTextColor);
        for (int i = 0; i < rm.buttonList.Count; i++)
        {
            EditorGUI.DrawRect(r, rm.generalButtonTint);
            EditorGUI.LabelField(r, rm.buttonList[i].text, style);
            r.y += buttonHeight;
        }

        if (rm.returnPosition == ReturnPosition.Last)
        {
            style.normal.textColor = VisibleTextFailsafe(rm.returnButtonTint, rm.returnTextColor);

            EditorGUI.DrawRect(r, rm.returnButtonTint);
            EditorGUI.LabelField(r, rm.returnButton.text, style);
        }
    }
}