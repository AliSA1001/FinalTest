#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Authoring helper: builds the "HeraldIntro" DialogueData asset (Bartleby's Act-1 opening) from code, since
/// the asset can't be hand-created here. Run via the menu; safe to re-run (rewrites the asset's nodes).
///
/// Note: n3 -> n4 is wired deliberately. The original brief sent n3's only choice straight to n5, which left
/// n4 (the answer about the princess) unreachable; routing through n4 makes that line play.
/// </summary>
public static class HeraldIntroBuilder
{
    private const string AssetPath = "Assets/Scripts/Dialogue/HeraldIntro.asset";

    [MenuItem("Tools/Act to Wish/Build HeraldIntro Dialogue")]
    public static void Build()
    {
        DialogueData data = AssetDatabase.LoadAssetAtPath<DialogueData>(AssetPath);
        bool isNew = data == null;
        if (isNew) data = ScriptableObject.CreateInstance<DialogueData>();

        data.dialogueId = "HeraldIntro";
        data.startId = "n0";
        data.nodes = BuildNodes();

        if (isNew)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(AssetPath));
            AssetDatabase.CreateAsset(data, AssetPath);
        }
        else
        {
            EditorUtility.SetDirty(data);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[HeraldIntro] {(isNew ? "Created" : "Updated")} {AssetPath} ({data.nodes.Count} nodes).");
        Selection.activeObject = data;
    }

    private static List<DialogueNode> BuildNodes()
    {
        return new List<DialogueNode>
        {
            Choice("n0", "Bartleby",
                "Oh, thank the heavens — a knight! A real one, sword and everything. Tell me quickly, ser: are you here to help, or are you just here to loot like everyone else?",
                ("I'm here to help.", "n1"),
                ("What in the world happened here?", "n1")),

            Choice("n1", "Bartleby",
                "The castle's fallen, ser. All of it. Taken by force before the morning bells.",
                ("A goblin raid?", "n2"),
                ("Bandits? Mercenaries?", "n2")),

            Linear("n2", "Bartleby",
                "Would that it were. No — this was a coup. A proper, organized, military coup. They had ranks, ser. Do you understand what I'm telling you? The goblins have learned to plan.",
                "n3"),

            Choice("n3", "Bartleby",
                "By dawn they'd seized the gates, the armory, and the tax records. They issued a decree before breakfast. It was grammatically correct. They've renamed the kingdom. Twice. There were minutes taken.",
                ("And the princess?", "n4")),

            Linear("n4", "Bartleby",
                "The princess was Taken. As a prisoner in the high tower of her own castle. Their general won't simply take the crown. He means to make her sign the kingdom over to goblin rule. Make it legal. Stamped. Filed. They're monsters, ser... but they are thorough.",
                "n5"),

            Choice("n5", "Bartleby",
                "You're the last blade left standing. If anyone's to reach that tower and bring her home, it's you. But the courtyard is crawling with their soldiers, and goblins fight dirty — they'll swing the instant your guard drops. So time your blade, ser. Meet their steel with yours — turn it aside, then strike. Stand there flinching and you won't see the second gate.",
                ("Then I'll cut my way to the tower.", "n6")),

            Linear("n6", "Bartleby",
                "That's the spirit. The crowd loves a hero. ...Did I say crowd? I meant kingdom.",
                "n7"),

            End("n7", "Bartleby",
                "Go, ser knight. For the princess. For the kingdom. And — if it isn't too much trouble — for my pension, which is currently being held by a goblin accountant named Snivel."),
        };
    }

    private static DialogueNode Linear(string id, string speaker, string text, string nextId)
    {
        return new DialogueNode { id = id, speaker = speaker, text = text, nextId = nextId, choices = new List<DialogueChoice>() };
    }

    private static DialogueNode End(string id, string speaker, string text)
    {
        return new DialogueNode { id = id, speaker = speaker, text = text, isEnd = true, choices = new List<DialogueChoice>() };
    }

    private static DialogueNode Choice(string id, string speaker, string text, params (string label, string next)[] choices)
    {
        var node = new DialogueNode { id = id, speaker = speaker, text = text, choices = new List<DialogueChoice>() };
        foreach (var c in choices)
        {
            node.choices.Add(new DialogueChoice { choiceText = c.label, nextId = c.next });
        }
        return node;
    }
}
#endif
