﻿using UnityEngine;
using UnityEngine.UI;

public class MessageBoxHud : MonoBehaviour
{
    [SerializeField] private GameObject _backButton;
    [SerializeField] private ButtonTextHandler _okButton;
    [SerializeField] private Image _characterPortrait;
    [SerializeField] private Image leftCharPortrait;
    [SerializeField] private Image rightCharPortrait;

    [SerializeField] private Text _characterName;
    [SerializeField] private Text _sayingText;
    [SerializeField] private Text _titleText;
    [SerializeField] private OptionsHandler _optionsHolder;

    private int _dialogId;
    private DialogManager _dialogManager;

    private float _initialHeight = 170;


    public void Construct(int dialogId, DialogManager dialogManager)
    {
        _dialogId = dialogId;
        _dialogManager = dialogManager;
        _backButton.SetActive(false);
        _okButton.SetText("ОК");
    }

    //coming form button
    public void OkayPressed()
    {
        _dialogManager.OkayPressed(_dialogId);
    }

    //coming form button
    public void BackPressed()
    {
        _dialogManager.BackPressed(_dialogId);
    }

    public void SetData(BaseDialogNode dialogNode)
    {
        ResetMessageBox();
        if (dialogNode == null)
            DialogComplete();
        else if (dialogNode is DialogStartNode)
            SetAsDialogStartNode((DialogStartNode) dialogNode);
        else if (dialogNode is DialogNode)
            SetAsDialogNode((DialogNode) dialogNode);
        else if (dialogNode is DialogMultiOptionsNode)
            SetAsMultiOptionsNode((DialogMultiOptionsNode) dialogNode);
        else
            Debug.LogError("Wrong Dialog type Sent Here");

        if (dialogNode != null)
        {
            AssignChars(dialogNode);

            Debug.Log(dialogNode.lawLevel);

            LawController.Instance.level += dialogNode.lawLevel;
            StressController.Instance.level += dialogNode.stressLevel;
        }
    }

    private void AssignChars(BaseDialogNode dialogNode)
    {
        if (dialogNode.LeftCharPortrait != null)
        {
            leftCharPortrait.sprite = dialogNode.LeftCharPortrait;
            leftCharPortrait.gameObject.SetActive(true);
        }

        if (dialogNode.RightCharPortrait != null)
        {
            rightCharPortrait.sprite = dialogNode.RightCharPortrait;
            rightCharPortrait.gameObject.SetActive(true);
        }
    }

    private void ResetMessageBox()
    {
//		Vector2 size = GetComponent<RectTransform>().sizeDelta;
//		size.y = _initialHeight;
//		GetComponent<RectTransform>().sizeDelta = size;
        _optionsHolder.ClearList();
        leftCharPortrait.gameObject.SetActive(false);
        rightCharPortrait.gameObject.SetActive(false);
    }

    private void DialogComplete()
    {
        _dialogManager.RemoveMessageBox(_dialogId);
    }

    private void SetAsDialogNode(DialogNode dialogNode)
    {
        _backButton.SetActive(dialogNode.IsBackAvailable());
        _okButton.ShowButton(true);
        _okButton.SetText(dialogNode.IsNextAvailable() ? "Далее" : "Ок");

        _sayingText.gameObject.SetActive(true);

        _characterPortrait.sprite = dialogNode.CharacterPotrait;
        _characterName.text = dialogNode.CharacterName;
        _sayingText.text = dialogNode.DialogLine;
    }

    private void SetAsDialogStartNode(DialogStartNode dialogStartNode)
    {
        _backButton.SetActive(dialogStartNode.IsBackAvailable());
        _okButton.ShowButton(true);
        _okButton.SetText(dialogStartNode.IsNextAvailable() ? "Далее" : "Ок");

        _characterPortrait.sprite = dialogStartNode.CharacterPotrait;
        _characterName.text = dialogStartNode.CharacterName;
        _sayingText.text = dialogStartNode.DialogLine;
    }


    private void SetAsMultiOptionsNode(DialogMultiOptionsNode dialogNode)
    {
        _backButton.SetActive(dialogNode.IsBackAvailable());
        _okButton.ShowButton(false);

        _characterPortrait.sprite = dialogNode.CharacterPotrait;
        _characterName.text = dialogNode.CharacterName;
        _sayingText.text = dialogNode.DialogLine;
        _sayingText.gameObject.SetActive(true);
        
        if (string.IsNullOrEmpty(_sayingText.text))
        {
            _sayingText.gameObject.SetActive(false);
        }

        _optionsHolder.CreateOptions(dialogNode.GetAllOptions(), OptionSelected);
        GrowMessageBox(dialogNode.GetAllOptions().Count);
    }

    private void GrowMessageBox(int count)
    {
//		Vector2 size = GetComponent<RectTransform>().sizeDelta;
//		size.y += (count * _optionsHolder.CellHeight());
//		GetComponent<RectTransform>().sizeDelta = size;
    }

    private void OptionSelected(int optionSelected)
    {
        _dialogManager.OptionSelected(_dialogId, optionSelected);
    }
}