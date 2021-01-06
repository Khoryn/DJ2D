using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;
    public GameObject dialogueBox;

    private Queue<string> sentences;

    Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        sentences = new Queue<string>();
        dialogueBox.gameObject.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueBox.gameObject.SetActive(true); // temporary, will add animation
        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void EndDialogue()
    {
        dialogueBox.gameObject.SetActive(false); // temporary, will add animation
        Debug.Log("End of conversation");
        player.state = State.IDLE;
    }
}
