using System.Collections.Generic;
using UnityEngine;

public class TraitSystem : MonoBehaviour
{
    [SerializeField]
    private List<string> startingTraits = new List<string>();

    private readonly HashSet<string> traits = new HashSet<string>();

    void Awake()
    {
        traits.Clear();
        for (int i = 0; i < startingTraits.Count; i++)
        {
            string trait = startingTraits[i];
            if (!string.IsNullOrEmpty(trait))
            {
                traits.Add(trait);
            }
        }
    }

    public bool HasTrait(string trait)
    {
        if (string.IsNullOrEmpty(trait))
        {
            return false;
        }
        return traits.Contains(trait);
    }

    public void GrantTrait(string trait)
    {
        if (string.IsNullOrEmpty(trait))
        {
            return;
        }
        traits.Add(trait);
    }

    public void RemoveTrait(string trait)
    {
        if (string.IsNullOrEmpty(trait))
        {
            return;
        }
        traits.Remove(trait);
    }

    public IEnumerable<string> GetAllTraits()
    {
        return traits;
    }
}


