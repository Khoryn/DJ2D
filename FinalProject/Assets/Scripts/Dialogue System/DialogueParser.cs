using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Subtegral.DialogueSystem.DataContainers;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class DialogueParser : MonoBehaviour
{
    public DialogueContainer dialogue;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Text npcName;
    [SerializeField] private Button choicePrefab;
    [SerializeField] private Transform buttonContainer;

    [HideInInspector]
    public bool hasTalked = false;

    [HideInInspector]
    public NodeLinkData narrativeData;

    GuiManager guiManager;
    NPC npc;

    private void Start()
    {
        guiManager = FindObjectOfType<GuiManager>();
        npc = GetComponent<NPC>();

        narrativeData = dialogue.NodeLinks.First();
    }

    private void Update()
    {
        if (buttonContainer.childCount <= 0 && GameState.IsTalking)
        {
            StartCoroutine(EndDialogue(4));
        }
    }

    public IEnumerator EndDialogue(float time)
    {
        yield return new WaitForSeconds(time);
        GameState.ChangeState(GameState.States.Idle);
        guiManager.dialogueContainer.SetActive(false);
    }

    public void ProceedToNarrative(string narrativeDataGUID)
    {
        string text = dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        IEnumerable<NodeLinkData> choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
        dialogueText.text = ProcessProperties(text);
        npcName.text = gameObject.name;
        Button[] buttons = buttonContainer.GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            Destroy(buttons[i].gameObject);
        }

        foreach (NodeLinkData choice in choices)
        {
            Button button = Instantiate(choicePrefab, buttonContainer);
            button.GetComponentInChildren<Text>().text = ProcessProperties(choice.PortName);
            button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGUID));
            button.onClick.AddListener(delegate { ChangeFriendshipLevel(choice.PortName, ConvertPropertyToFloat(choice.PortName)); });
            StartCoroutine(SetNewDialogueButton());
        }
    }

    private void ChangeFriendshipLevel(string text, float amount)
    {
        int start = text.IndexOf("[") + 1;
        int end = text.IndexOf("]", start);

        if (text.Contains("-") || text.Contains("+"))
        {
            npc.friendshipLevel += amount;

            if (npc.friendshipLevel >= 100)
            {
                npc.friendshipLevel = 100;
                Debug.Log($"{npc.name}'s friendship is maxed out");
            }
            if (npc.friendshipLevel <= 0)
            {
                npc.friendshipLevel = 0;
                Debug.Log($"{npc.name}'s friendship is at it's lowest");
            }
        }
        else
        {
            Debug.Log("Do nothing");
        }
    }

    private IEnumerator SetNewDialogueButton()
    {
        EventSystem.current.SetSelectedGameObject(null);

        yield return new WaitForSeconds(0.1f);

        guiManager.dialogueButton = buttonContainer.GetChild(0).gameObject;
        EventSystem.current.SetSelectedGameObject(guiManager.dialogueButton);
    }

    private string ProcessProperties(string text)
    {
        int start = text.IndexOf("[") + 1;
        int end = text.IndexOf("]", start);

        foreach (ExposedProperty exposedProperty in dialogue.ExposedProperties)
        {
            if (float.TryParse(exposedProperty.PropertyValue, out float n))
            {
                text = Regex.Replace(text, @"\[.*?\]", "");
            }
            else
            {
                text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
            }
        }
        return text;
    }

    private float ConvertPropertyToFloat(string text)
    {
        string textResult = "";

        int start = text.IndexOf("[") + 1;
        int end = text.IndexOf("]", start);

        if (float.TryParse(text.Substring(start, end - start), out float n))
        {
            textResult = text.Substring(start, end - start);
        }
        return float.Parse(textResult);
    }
}
