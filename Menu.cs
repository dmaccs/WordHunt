using Godot;
using System;
using System.Collections.Generic;

public partial class Menu : Control
{
	//game_board gameBoard;
	LineEdit lettersAvailable;
	public Trie trie;

	TrieManager trieManager;
	Solver solver;
	GameBoard gameBoard;
	Button playButton;
	Button solveButton;
	LineEdit solveInput;
	VBoxContainer wordList;
	ScrollContainer scrollContainer;
	ColorRect Background;

	[Export] public Font CustomFont;

	bool firstTime = false; //Set to true to create a Trie from scratch
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		lettersAvailable = GetNode<LineEdit>("LineEdit");
		trieManager = new TrieManager();
		gameBoard = GetNode<GameBoard>("GameBoard");
		playButton = GetNode<Button>("Play");
		solveButton = GetNode<Button>("Solve");
		solveInput = GetNode<LineEdit>("LineEdit");
		scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		wordList = scrollContainer.GetChild<VBoxContainer>(0);
		Background = GetNode<ColorRect>("Background");
		
		if(firstTime){
			trie = new Trie();
			trie.PopulateFromFile("res://Dictionary/CollinsDict2019.txt", trie);
			trieManager.SaveTrieToFile(trie, "user://trie_data.json");
		} else {
			trie = trieManager.LoadTrieFromFile("user://trie_data.json");
		}
		solver = new Solver();
		solver.instantiateSolver(trie);
		
	}

	public void _on_play_button_down(){
		if(gameBoard != null){
			gameBoard.GenerateGame();
			gameBoard.Visible = true;
			playButton.Visible = false;
			solveButton.Visible = false;
			solveInput.Visible = false;
			Background.Visible = false;
			scrollContainer.Visible = false;
			wordList.Visible = false;
		}
	}

	public void GameOver(string PreviousBoard){
		if(gameBoard != null){
			gameBoard.Visible = false;
			playButton.Visible = true;
			solveButton.Visible = true;
			solveInput.Visible = true;
			Background.Visible = true;
			scrollContainer.Visible = true;
			wordList.Visible = true;
		}
		lettersAvailable.Text = PreviousBoard;
		_on_solve_button_down();
	}

	public void _on_solve_button_down(){
		foreach (Node child in wordList.GetChildren()){
    		wordList.RemoveChild(child);
    		child.QueueFree();
		}
		List<string> results = solver.solve(lettersAvailable.Text.ToUpper());
		foreach(string w in results){
			string wordScore = "";
			if(w.Length == 3){
				wordScore = 100.ToString() + ": " + w;
			} else {
				wordScore = (400*(w.Length-3)).ToString() + ": " + w;
			}
            var label = new Label
            {
                Text = wordScore,
            };
			label.AddThemeFontOverride("font", CustomFont);
			label.HorizontalAlignment = HorizontalAlignment.Center;
			label.AddThemeFontSizeOverride("font_size", 40);
            wordList.AddChild(label);
		}
	}
}
