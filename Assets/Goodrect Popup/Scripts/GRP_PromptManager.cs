using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GRP_PromptManager : GRP_ManagerBase
{
    public float messagePaddingVertical;
    public GRP_PromptButtonOptions returnButton;

    public override void ResetButtons()
    {
        returnButton = new GRP_PromptButtonOptions();
        returnButton.action = new GRP_PromptEvent();
    }

    public void SetReturn(string label, UnityAction<string> action)
    {
        returnButton.action.AddRuntimeListener(action);
        returnButton.text = label;
    }

    public override GameObject CreateAndReturn(string name_suffix = "", bool fadeIn = false)
    {
        GameObject popupGO = Instantiate(popupPF);
        popupGO.transform.localScale = Vector3.one;
        popupGO.transform.SetParent(container);

        GRP_Prompt promptComponent = popupGO.GetComponent<GRP_Prompt>();

        promptComponent.SetTextColors(titleColor, returnTextColor, returnTextColorPressed);

        popupGO.transform.localScale = Vector3.one;
        popupGO.transform.SetParent(container);

        promptComponent.Initialize(title, message);

        promptComponent.SetReturn(returnButton);

        promptComponent.SetTypography(textAlignment, titleFont, bodyFont, (int)titleFontSize, (int)bodyFontSize, (int)buttonFontSize, buttonFont);
        promptComponent.StyleBackground(backgroundImage, backgroundTint);
        promptComponent.StyleContainer(referenceSize, titleSeparator, buttonSeparator, titlePadding, buttonPadding, containerPadding, containerImage, containerTint, containerBleed);
        promptComponent.SetFade(fadeAnimation, fadeDuration, fadeProfile);

        Place(placement, promptComponent);

        popupGO.name = "GRP Prompt Instance" + name_suffix;

        if (fadeIn)
            promptComponent.FadePopupIn();


        promptComponent.StyleInputField(promptType, messagePaddingVertical, titleSeparator, buttonSeparator, messagePadding, promptInputFieldImage, promptInputFieldTint, promptInputfieldColor, promptPlaceholderColor);
        promptComponent.StyleReturnButton(buttonSeparation, returnButtonImage, returnButtonPressedImage, buttonImage, buttonPressedImage, returnButtonTint, returnButtonPressedTint, buttonBleed, buttonPrefixImage);
        promptComponent.AlignText();

        return popupGO;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}