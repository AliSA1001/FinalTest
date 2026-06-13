using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>One choice on a dialogue node: the button label and the node it leads to.</summary>
[Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string nextId;
}

/// <summary>
/// A single line of dialogue. A node either advances linearly (via <see cref="nextId"/>) or branches (via
/// <see cref="choices"/>); mark <see cref="isEnd"/> on the final line.
/// </summary>
[Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;
    [TextArea(2, 5)] public string text;
    [Tooltip("Linear next node. Ignored when this node has choices or is an end node.")]
    public string nextId;
    [Tooltip("Branching choices. When non-empty, these are shown instead of advancing linearly.")]
    public List<DialogueChoice> choices = new List<DialogueChoice>();
    [Tooltip("If true, confirming this line ends the conversation.")]
    public bool isEnd;

    public bool HasChoices => choices != null && choices.Count > 0;
}

/// <summary>
/// A whole conversation as an ordered set of nodes. Pure data — the <see cref="DialogueRunner"/> walks it and
/// the <see cref="DialogueView"/> displays it. References no other system.
/// </summary>
[CreateAssetMenu(fileName = "Dialogue", menuName = "Act to Wish/Dialogue")]
public class DialogueData : ScriptableObject
{
    [Tooltip("Identifier raised in DialogueEnded(id), e.g. \"HeraldIntro\".")]
    public string dialogueId;
    [Tooltip("Node to start on. Empty = the first node in the list.")]
    public string startId;
    public List<DialogueNode> nodes = new List<DialogueNode>();

    private Dictionary<string, DialogueNode> _lookup;

    /// <summary>The node the conversation begins on.</summary>
    public DialogueNode StartNode
    {
        get
        {
            if (!string.IsNullOrEmpty(startId)) return GetNode(startId);
            return nodes != null && nodes.Count > 0 ? nodes[0] : null;
        }
    }

    /// <summary>Finds a node by id (or null). Builds the lookup on first use.</summary>
    public DialogueNode GetNode(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (_lookup == null) BuildLookup();
        return _lookup.TryGetValue(id, out var node) ? node : null;
    }

    private void BuildLookup()
    {
        _lookup = new Dictionary<string, DialogueNode>();
        if (nodes == null) return;
        foreach (var n in nodes)
        {
            if (n != null && !string.IsNullOrEmpty(n.id)) _lookup[n.id] = n;
        }
    }
}
