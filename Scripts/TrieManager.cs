using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace WordHunt;
public partial class TrieManager : Node
{
    public Trie LoadTrieFromFile(string filePath)
    {
        string jsonData = "";
        try
        {
            // Read the file contents
            using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
            {
                jsonData = file.GetAsText();
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error reading file {filePath}: {e.Message}");
        }
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData); // Deserialize JSON data to a Dictionary
        return DeserializeTrie(data);
    }

    public Trie DeserializeTrie(Dictionary<string, object> data)
    {
        Trie node = new();
        if (data.ContainsKey("validWord"))
        {

            JsonElement validWordElement = (JsonElement)data["validWord"];

            if (validWordElement.ValueKind == JsonValueKind.True || validWordElement.ValueKind == JsonValueKind.False)
            {
                node.validWord = validWordElement.GetBoolean();  // Get the boolean value
            }
            else
            {
                GD.PrintErr("Invalid value for 'validWord'. Expected a boolean.");
            }
        }

        // Deserialize the children property
        if (data.ContainsKey("children"))
        {
            JsonElement childrenElement = (JsonElement)data["children"];
            if (childrenElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var childEntry in childrenElement.EnumerateObject())
                {

                    char childKey = Convert.ToChar(childEntry.Name);

                    if (childEntry.Value.ValueKind == JsonValueKind.Object)
                    {
                        Dictionary<string, object> childData = new Dictionary<string, object>();
                        foreach (var item in childEntry.Value.EnumerateObject())
                        {
                            childData[item.Name] = item.Value;
                        }
                        node.Children[childKey] = DeserializeTrie(childData); // Recursively deserialize the child node
                    }
                }
            }
        }
        return node;
    }

    public void SaveTrieToFile(Trie root, string filePath)
    {

        try
        {
            string json = JsonSerializer.Serialize(SerializeTrie(root));

            using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write))
            {
                file.StoreString(json);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error saving file {filePath}: {e.Message}");
        }
    }

    public Dictionary<string, object> SerializeTrie(Trie node)
    {

        var children = new Dictionary<char, Dictionary<string, object>>();
        foreach (var kvp in node.Children)
        {
            children[kvp.Key] = SerializeTrie(kvp.Value);
        };

        return new Dictionary<string, object>
        {
            { "validWord", node.validWord },
            { "children", children }
        };
    }
}
