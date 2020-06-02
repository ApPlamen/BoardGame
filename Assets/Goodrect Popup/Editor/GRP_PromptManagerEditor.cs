using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(GRP_PromptManager))]
public class GRP_PromptManagerEditor : GRP_ManagerBaseEditor
{
    private GRP_PromptManager pm;

    private void Awake()
    {
        base.type = "Prompt";
        pm = target as GRP_PromptManager;
        if (pm.popupPF == null)
            pm.popupPF = AssetDatabase.LoadAssetAtPath<GameObject>(FindAbsolutePath("GRP_Prompt.prefab"));

        if(pm.initialize)
        {
            pm.buttonSeparator = 25;
            pm.titleSeparator = 80;
            pm.buttonSeparation = 10;
            pm.messagePaddingVertical = 20;

            pm.returnButton = new GRP_PromptButtonOptions();
            pm.returnButton.text = "You get me";
        }

        OnAwake(pm);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PresetGroup();

        EditorGUILayout.Space();

        if(GUILayout.Button("Collapse All")) {
            pm.showBehaviour = false;
            pm.showRatios = false;
            pm.showDesign = false;
            pm.showTypography = false;
            pm.showButtons = false;
        }

        BehaviourGroup();
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
        pm.showDesign = EditorGUILayout.Foldout(pm.showDesign, "Colors & Textures");

        if(pm.showDesign)
        {
            SpriteField(ref pm.containerImage, "Container Image");
            IntField(ref pm.containerBleed, "Container Bleed (px)");
            ColorField(ref pm.containerTint, "ContainerTint");

            EditorGUILayout.Space();

            SpriteField(ref pm.promptInputFieldImage, "Inputfield Image");
            ColorField(ref pm.promptInputFieldTint, "Inputfield Tint");

            EditorGUILayout.Space();

            BoolField(ref pm.buttonPrefixImage, "Prefix Image");
            SpriteField(ref pm.returnButtonImage, "Return Button Image");
            ColorField(ref pm.returnButtonTint, "Return Button Tint");
            SpriteField(ref pm.returnButtonPressedImage, "Return B. Pressed Image");
            ColorField(ref pm.returnButtonPressedTint, "Return B. Pressed Tint");

            EditorGUILayout.Space();

            IntField(ref pm.buttonBleed, "Button Bleed (px)");
            SpriteField(ref pm.backgroundImage, "Background Image");
            ColorField(ref pm.backgroundTint, "Background Tint");
        }
    }

    private void TypographyGroup() {
        pm.showTypography = EditorGUILayout.Foldout(pm.showTypography, "Typography & Text");

        if(pm.showTypography)
        {
            StringField(ref pm.title, "Prompt Title");
            StringField(ref pm.message, "Inputfield Placeholder", true);

            EditorGUILayout.Space();

            FontField(ref pm.titleFont, "Title Font");
            FontField(ref pm.bodyFont, "Body Font");
            FontField(ref pm.buttonFont, "Button Font");

            EditorGUILayout.Space();

            IntField(ref pm.titleFontSize, "Title Font Size", true);
            IntField(ref pm.bodyFontSize, "Body Font Size", true);
            IntField(ref pm.buttonFontSize, "Button Font Size", true);

            EditorGUILayout.Space();

            ColorField(ref pm.titleColor, "Title Color");

            ColorField(ref pm.promptInputfieldColor, "Inputfield Text Color");
            ColorField(ref pm.promptPlaceholderColor, "Placeholder Text Color");

            ColorField(ref pm.returnTextColor, "Return Text Color");
            ColorField(ref pm.returnTextColorPressed, "R. Text Color Pressed");

            EditorGUILayout.Space();

            pm.textAlignment = (TextAlignment)EditorGUILayout.EnumPopup("Text Alignment", pm.textAlignment);
        }
    }

    private void RatioGroup()
    {
        pm.showRatios = EditorGUILayout.Foldout(pm.showRatios, "Inner Ratios");
        if (pm.showRatios) {
            RangeFieldInt(ref pm.containerPadding, "Container Padding (Percent)", 0, 45);
            RangeField(ref pm.titlePadding, "Title Padding (Percent)", 0, 45);
            RangeField(ref pm.messagePadding, "Inputfield Horizontal Padding (Percent)", 0, 45);
            RangeField(ref pm.messagePaddingVertical, "Inputfield Vertical Padding (Percent)", 0, 45);
            RangeField(ref pm.buttonPadding, "Button Padding (Percent)", 0, 45);
            RangeField(ref pm.buttonSeparation, "Button Vertical Padding (Percent)", 0, 45);

            EditorGUILayout.Space();

            RangeField(ref pm.titleSeparator, "Title Separator Position (Percent)", 50f, 100f);
            RangeField(ref pm.buttonSeparator, "Button Separator Position (Percent)", 10f, 80f);

            float stretch = 2f;

            if (pm.buttonSeparator > pm.titleSeparator)
                pm.titleSeparator = pm.buttonSeparator;

            int contentHeight = (int)(stretch * (100 - pm.containerPadding * 2));

            int titlePreview = (int)(contentHeight * (1 - pm.titleSeparator / 100f));
            int buttonPreview = (int)(contentHeight * pm.buttonSeparator / 100f);
            int messagePreview = contentHeight - (titlePreview + buttonPreview);

            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Preview only shows tints");
            EditorGUILayout.LabelField("Texture colors do not show below");

            PaddingPreview(pm.containerTint, (int)(stretch * pm.containerPadding));
            SectionPreview(pm.containerTint, pm.titlePadding / 100.0f, titlePreview, pm.title, pm.titleColor);
            SectionPreview(pm.containerTint, pm.messagePadding / 100.0f, messagePreview, pm.message, pm.bodyTextColor, true, pm.messagePaddingVertical);
            ButtonSectionPreview(pm.containerTint, pm.buttonPadding / 100.0f, buttonPreview);
            PaddingPreview(pm.containerTint, (int)(stretch * pm.containerPadding));
        }
    }

    private void ButtonsGroup()
    {
        if (pm.returnButton == null)
            pm.returnButton = new GRP_PromptButtonOptions();
            
        pm.returnButton.foldoutState = EditorGUILayout.Foldout(pm.returnButton.foldoutState, "Return Button");

        if(pm.returnButton.foldoutState) {
            StringField(ref pm.returnButton.text, "Return Title");

            if (pm.returnButton.text.Trim() == "")
                SpriteField(ref pm.returnButton.image, "Return Visual");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("returnButton").FindPropertyRelative("action"), true);
        }


        serializedObject.ApplyModifiedProperties();
    }


    public void ButtonSectionPreview(Color color, float innerPadding, float height)
    {
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


        GUIStyle style = new GUIStyle();
        if(pm.textAlignment == TextAlignment.Left)
            style.alignment = TextAnchor.MiddleLeft;
        else if (pm.textAlignment == TextAlignment.Center)
            style.alignment = TextAnchor.MiddleCenter;
        else
            style.alignment = TextAnchor.MiddleRight;

        style.wordWrap = true;
        style.clipping = TextClipping.Clip;
        if (height * (1 - pm.buttonSeparation * 2 / 100) < 12)
            style.fontSize = (int)(height * (1 - pm.buttonSeparation * 2 / 100));

        r.height = height;

        r.y += r.height * pm.buttonSeparation / 100;
        r.height *= (1 - pm.buttonSeparation * 2 / 100);

        style.normal.textColor = VisibleTextFailsafe(color, pm.returnTextColor);

        EditorGUI.DrawRect(r, pm.returnButtonTint);
        EditorGUI.LabelField(r, pm.returnButton.text, style);
    }
}