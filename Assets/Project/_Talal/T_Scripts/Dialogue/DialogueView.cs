using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Shows the current dialogue line: speaker, body text and choice buttons. Thin MonoBehaviour driven entirely
/// by <see cref="DialogueEvents"/> — it never references the runner. Choice buttons raise
/// <see cref="DialogueEvents.SelectChoice"/>.
/// </summary>
public class DialogueView : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _speaker;
    [SerializeField] private TMP_Text _body;
    [Tooltip("One button per possible choice; extras are hidden. Each needs a child TMP_Text label.")]
    [SerializeField] private Button[] _choiceButtons = new Button[0];

    private TMP_Text[] _choiceLabels;

    private void Awake()
    {
        _choiceLabels = new TMP_Text[_choiceButtons.Length];
        for (int i = 0; i < _choiceButtons.Length; i++)
        {
            if (_choiceButtons[i] == null) continue;
            int index = i; // capture per button
            _choiceButtons[i].onClick.AddListener(() => DialogueEvents.SelectChoice(index));
            _choiceLabels[i] = _choiceButtons[i].GetComponentInChildren<TMP_Text>(true);
        }
        if (_panel != null) _panel.SetActive(false);
    }

    private void OnEnable()
    {
        DialogueEvents.DialogueStarted += OnStarted;
        DialogueEvents.DialogueLineShown += OnLineShown;
        DialogueEvents.DialogueEnded += OnEnded;
    }

    private void OnDisable()
    {
        DialogueEvents.DialogueStarted -= OnStarted;
        DialogueEvents.DialogueLineShown -= OnLineShown;
        DialogueEvents.DialogueEnded -= OnEnded;
    }

    private void OnStarted(string id)
    {
        if (_panel != null) _panel.SetActive(true);
    }

    private void OnLineShown(DialogueNode node)
    {
        if (node == null) return;
        if (_speaker != null) _speaker.text = node.speaker;
        if (_body != null) _body.text = node.text;

        int count = node.HasChoices ? node.choices.Count : 0;
        for (int i = 0; i < _choiceButtons.Length; i++)
        {
            if (_choiceButtons[i] == null) continue;
            bool active = i < count;
            _choiceButtons[i].gameObject.SetActive(active);
            if (active && _choiceLabels[i] != null)
            {
                _choiceLabels[i].text = node.choices[i].choiceText;
            }
        }
    }

    private void OnEnded(string id)
    {
        if (_panel != null) _panel.SetActive(false);
    }
}
