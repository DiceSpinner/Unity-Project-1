using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

[Interaction(interactable :typeof(DialogueDisplayer), interactors: typeof(PlayerController))] 
public class DialogueDisplayer : InteractableObject
{
    [Header("Settings")]
    [SerializeField] private RefBool resetUponFinish;
    [SerializeField] private RefFloat displaySpeed;
    [SerializeField] private RefFloat secondsDelayBetweenLines;
    [SerializeField] private RefFloat secondsLingering;
    [SerializeField] private DialogueObject dialogue;
    [SerializeField] private RectTransform dialogueCanva;

    [Header("Dialogue Area")]
    [SerializeField] private TMP_Text mainText;
    
    [Header("Response Area")]
    [SerializeField] private RectTransform responseLayoutGroup;
    [SerializeField] private Button responseButton;

    [Header("Speaker Info")]
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private Image speakerIcon;

    private Coroutine coroutine;
    private DialogueObject currentDialogue;

    private List<Button> responseButtons = new();
    private readonly Dictionary<InteractionType, string> idleOption = new Dictionary<InteractionType, string>() {
        { InteractionType.Primary, "Talk"}
    };

    private readonly Dictionary<InteractionType, string> runningOption = new Dictionary<InteractionType, string>() {
        { InteractionType.Primary, "Stop"}
    };

    new void Start()
    {
        base.Start();
        currentDialogue = dialogue;
    }

    public override string Interact(Interactor interactor, InteractionType type) {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(DisplayText());
        }
        else {
            StopCoroutine(coroutine);
            coroutine = null;
            currentDialogue = dialogue;
            dialogueCanva.gameObject.SetActive(false);
        }
        return Setting.INTERACTION_OK;
    }

    public IEnumerator DisplayText() {
        dialogueCanva.gameObject.SetActive(true);

        // Begin displaying text in each node
        foreach (DialogueNode node in currentDialogue.Nodes) {
            speakerName.text = node.Speaker;
            speakerIcon.sprite = node.Icon;
            foreach (string str in node.Texts)
            {
                float index = 0;
                while (index < str.Length)
                {
                    mainText.text = str.Substring(0, Mathf.FloorToInt(index));
                    index += Time.deltaTime * displaySpeed.Value;
                    yield return null;
                }
                mainText.text = str;
                yield return new WaitForSeconds(secondsDelayBetweenLines.Value);
            }
        }
        
        // Instantiate response buttons
        if (currentDialogue.HasResponses) {
            DialogueResponse[] responses = currentDialogue.Responses;
            foreach (DialogueResponse response in responses)
            {
                GameObject obj = Instantiate(responseButton.gameObject, responseLayoutGroup);
                obj.SetActive(true);
                Button btn = obj.GetComponent<Button>();
                btn.onClick.AddListener(() => Respond(response));
                btn.GetComponentInChildren<TMP_Text>().text = response.Text; 
                responseButtons.Add(btn);
            }
        }

        // Execute dialogue finishers
        if (currentDialogue.HasFinishers)
        {
            currentDialogue.ExecuteFinisher();
        }
        if (resetUponFinish.Value) {
            currentDialogue = dialogue;
        }
        coroutine = null;

        yield return new WaitForSeconds(secondsLingering.Value);

        // Keep the canvas running if there's responses to display
        if (responseButtons.Count == 0) {
            dialogueCanva.gameObject.SetActive(false);
        }
    }

    public void Respond(DialogueResponse response) { 
        response.Respond();
        if (response.Dialogue != null)
        {
            currentDialogue = response.Dialogue;
            coroutine = StartCoroutine(DisplayText());
        }
        else {
            currentDialogue = dialogue;
        }
        
        foreach (Button b in responseButtons) {
            Destroy(b.gameObject);
        }
        responseButtons.Clear();
    }

    public override Dictionary<InteractionType, string> GetInteractionType(Type t)
    {
        if (coroutine != null) {
            return runningOption;
        }
        return idleOption;
    }
}