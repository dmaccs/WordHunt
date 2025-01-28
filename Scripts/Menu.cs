using Godot;
using System.Collections.Generic;

namespace WordHunt;
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
	RichTextLabel HighScoreLabel; 
	public int HighScore = 0;

	private ConfigManager configManager;

	[Export] public Font CustomFont;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		configManager = new ConfigManager();
        bool firstTime = configManager.LoadFirstTimeStatus();
		HighScoreLabel = GetNode<RichTextLabel>("HighScore");
		SetHighScore(configManager.GetHighScore());
		configManager.GetHighScore();
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
			GD.Print("First Time");
			trie = new Trie();
			trie.PopulateFromFile("res://Dictionary/CollinsDict2019.txt", trie);
			trieManager.SaveTrieToFile(trie, "user://trie_data.json");
			configManager.SaveFirstTimeStatus(false);
		} else {
			GD.Print("Not First Time");
			trie = trieManager.LoadTrieFromFile("user://trie_data.json");
		}
		solver = new Solver();
		solver.instantiateSolver(trie);
		
	}

	public void SetHighScore(int previoiusScore){
		if(previoiusScore > HighScore){
			HighScoreLabel.Text = "High Score: " + previoiusScore.ToString();
			HighScore = previoiusScore;
			configManager.SaveHighScore(HighScore);
		}
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
			HighScoreLabel.Visible = false;
		}
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

	public void GameOver(string PreviousBoard){
		if(gameBoard != null){
			gameBoard.Visible = false;
			playButton.Visible = true;
			solveButton.Visible = true;
			solveInput.Visible = true;
			Background.Visible = true;
			scrollContainer.Visible = true;
			wordList.Visible = true;
			HighScoreLabel.Visible = true;
		}
		lettersAvailable.Text = PreviousBoard;
		_on_solve_button_down();
	}

	
}
