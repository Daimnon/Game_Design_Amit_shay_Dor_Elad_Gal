using UnityEngine;
using System.Collections.Generic;

public class CustomTag : MonoBehaviour
{
    [SerializeField]
    private List<string> tags = new List<string>();

    public bool IsEnabled = true;

    public int Count { get => tags.Count; }
    
    public bool HasTag(string tag)
    {
        return tags.Contains(tag);
    }

    public IEnumerable<string> GetTags()
    {
        return tags;
    }

    public void Rename(int index, string tagName)
    {
        tags[index] = tagName;
    }

    public string GetAtIndex(int index)
    {
        return tags[index];
    }
}
