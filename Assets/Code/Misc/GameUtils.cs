using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagList //This is like a dictionary but nicer to use
{
    private Dictionary<string, Tag> tags;

    public Tag this[string tagName]
    {
        get
        {
            if (!tags.ContainsKey(tagName))
                return null;

            return tags[tagName];
        }
        set
        {
            if (tags.ContainsKey(tagName))
                tags[tagName].value = value;
            else
                tags.Add(tagName, new Tag(tagName, value));
        }
    }

    public bool HasTag(string tagName)
    {
        return tags.ContainsKey(tagName);
    }
}
public class Tag //Snippet of information. Useful for stuff that may apply to an entity but isn't worth keeping track of on all of them. e.g. status effects.
{
    public string name;
    public object value;

    public Tag(string tagName, object tagValue)
    {
        name = tagName;
        value = tagValue;
    }
}