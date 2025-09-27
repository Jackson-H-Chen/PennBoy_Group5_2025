using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogOption
{
    public string text;
    public string nextNodeId;
    public bool endsDialog;

    // Trait effects (placeholders)
    public string grantTrait;
    public string removeTrait;

    // Simple gating
    public string requiredTrait;
    public string forbiddenTrait;

    public bool IsAvailable(TraitSystem traitSystem)
    {
        if (traitSystem == null)
        {
            return true;
        }

        if (!string.IsNullOrEmpty(requiredTrait) && !traitSystem.HasTrait(requiredTrait))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(forbiddenTrait) && traitSystem.HasTrait(forbiddenTrait))
        {
            return false;
        }

        return true;
    }
}

[Serializable]
public class DialogNode
{
    public string id;
    public string speaker;
    [TextArea(3, 8)]
    public string body;
    public List<DialogOption> options = new List<DialogOption>();
}

[Serializable]
public class DialogConversation
{
    public string startNodeId;
    public List<DialogNode> nodes = new List<DialogNode>();

    public DialogNode GetNode(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId) || nodes == null)
        {
            return null;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] != null && nodes[i].id == nodeId)
            {
                return nodes[i];
            }
        }
        return null;
    }
}


