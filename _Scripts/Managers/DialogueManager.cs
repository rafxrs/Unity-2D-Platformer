using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;
    public Animator animator;
    

    [SerializeField] bool inDialogue=false;

    Queue<string> _sentences;
    // Start is called before the first frame update
    void Start()
    {
        _sentences = new Queue<string>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && inDialogue)
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    
    {
        inDialogue = true;
        animator.SetBool("isOpen", true);
        nameText.text = dialogue.name;
        // Debug.Log("Starting conversation with "+dialogue.name);

        _sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        else 
        {
            string sentence = _sentences.Dequeue();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));
        }

    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        inDialogue = false;
        animator.SetBool("isOpen", false);
        Debug.Log("End of conversation");
        FindObjectOfType<GameManager>().EnablePlayerControl();
    }

    
}