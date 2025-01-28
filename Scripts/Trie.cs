using Godot;
using System;
using System.Collections.Generic;

namespace WordHunt;
public partial class Trie : Node
{
	// Called when the node enters the scene tree for the first time.
	public Dictionary<char, Trie> Children = new Dictionary<char, Trie>();
	public bool validWord = false;

	public Trie()
	{
		if (Children == null)
		{
			Children = new Dictionary<char, Trie>();
		}

	}

	public void AddWord(string word)
	{

		Trie curr = this;

		foreach (char c in word)
		{
			if (!curr.Children.ContainsKey(c))
				curr.Children[c] = new Trie();
			curr = curr.Children[c];
		}
		curr.validWord = true;
	}

	public bool ValidWord(string word)
	{

		Trie curr = this;
		foreach (char c in word)
		{
			if (curr == null)
			{
				return false;
			}
			if (!curr.Children.ContainsKey(c))
			{
				return false;
			};
			curr = curr.Children[c];
		};
		return curr.validWord;
	}

	public void PopulateFromFile(string filePath, Trie root)
	{
		try
		{
			using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
			while (!file.EofReached())
			{
				string line = file.GetLine();
				if (line.Length > 2)
				{
					root.AddWord(line.Trim());
				}
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error reading file {filePath}: {e.Message}");
		}
	}
};