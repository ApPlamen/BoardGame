using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GRP_MultichoiceManager : GRP_ManagerBase
{
    public List<GRP_ButtonOptions> buttonList;
    public bool showReturn = true;
    public GRP_ButtonOptions returnButton;

    public void SetReturn(string label, UnityAction action)
    {
        returnButton.text = label;

        if(action != null)
            returnButton.action.AddRuntimeListener(action);
    }

    public void AddButton(string label, UnityAction action) {
        GRP_ButtonOptions bo = new GRP_ButtonOptions();
        bo.action = new GRP_Event();
        bo.text = label;
        bo.action.AddRuntimeListener(action);

        buttonList.Add(bo);
    }

    public void AddButton(Sprite image, UnityAction action)
    {
        GRP_ButtonOptions bo = new GRP_ButtonOptions();
        bo.action = new GRP_Event();
        bo.text = "";
        bo.image = image;
        bo.action.AddRuntimeListener(action);

        buttonList.Add(bo);
    }

    public override void ResetButtons()
    {
        returnButton = new GRP_ButtonOptions();
        returnButton.action = new GRP_Event();
        buttonList = new List<GRP_ButtonOptions>();
    }

    public override GameObject CreateAndReturn(string name_suffix = "", bool fadeIn = false) {
        GameObject popupGO = Instantiate(popupPF);
        popupGO.transform.localScale = Vector3.one;
        popupGO.transform.SetParent(container);

        GRP_Multichoice multichoiceComponent = popupGO.GetComponent<GRP_Multichoice>();

        multichoiceComponent.rpos = returnPosition;
        multichoiceComponent.SetTextColors(titleColor, bodyTextColor, buttonTextColor, returnTextColor, buttonTextColorPressed, returnTextColorPressed);

        popupGO.transform.localScale = Vector3.one;
        popupGO.transform.SetParent(container);

        multichoiceComponent.Initialize(title, message);

        multichoiceComponent.SetReturn(returnButton);

        multichoiceComponent.SetTypography(textAlignment, titleFont, bodyFont, (int)titleFontSize, (int)bodyFontSize, (int)buttonFontSize, buttonFont, returnFont);
        multichoiceComponent.StyleBackground(backgroundImage, backgroundTint);
        multichoiceComponent.StyleContainer(referenceSize, titleSeparator, buttonSeparator, titlePadding, buttonPadding, containerPadding, containerImage, containerTint, containerBleed);
        multichoiceComponent.SetFade(fadeAnimation, fadeDuration, fadeProfile);

        popupGO.name = "GRP Multichoice Instance" + name_suffix;

        Place(placement, multichoiceComponent);

        if (fadeIn)
            multichoiceComponent.FadePopupIn();

        for (int i = 0; buttonList != null && i < buttonList.Count; i++)
        {
            multichoiceComponent.AddButton(buttonList[i]);
        }

        multichoiceComponent.StyleMessageText(titleSeparator, buttonSeparator, messagePadding);
        multichoiceComponent.StyleButtons(showReturn, buttonDirection, buttonSeparation, buttonImage, buttonPressedImage, generalButtonTint, generalButtonPressedTint, buttonBleed, buttonPrefixImage);

        if(showReturn)
            multichoiceComponent.StyleReturnButton(buttonSeparation, returnButtonImage, returnButtonPressedImage, buttonImage, buttonPressedImage, returnButtonTint, returnButtonPressedTint, buttonBleed, buttonPrefixImage, buttonDirection);
        multichoiceComponent.AlignText();

        return popupGO;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}