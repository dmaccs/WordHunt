using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

public partial class GameBoard : Control
{
	char[] letterDistribution = {
	'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 				// A-9
    'B', 'B',                                     				// B-2
    'C', 'C',                                     				// C-2
    'D', 'D', 'D', 'D',                           				// D-4
    'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', // E-12
    'F', 'F',                                     				// F-2
    'G', 'G', 'G',                                				// G-3
    'H', 'H',                                     				// H-2
    'I', 'I', 'I', 'I', 'I', 'I', 'I', 'I', 'I',  				// I-9
    'J',                                          				// J-1
    'K',                                          				// K-1
    'L', 'L', 'L', 'L',                           				// L-4
    'M', 'M',                                     				// M-2
    'N', 'N', 'N', 'N', 'N', 'N',                 				// N-6
    'O', 'O', 'O', 'O', 'O', 'O', 'O', 'O',       				// O-8
    'P', 'P',                                     				// P-2
    'Q',                                          				// Q-1
    'R', 'R', 'R', 'R', 'R', 'R',                 				// R-6
    'S', 'S', 'S', 'S',                           				// S-4
    'T', 'T', 'T', 'T', 'T', 'T',                 				// T-6
    'U', 'U', 'U', 'U',                           				// U-4
    'V', 'V',                                     				// V-2
    'W', 'W',                                     				// W-2
    'X',                                          				// X-1
    'Y', 'Y',                                     				// Y-2
    'Z'                                           				// Z-1
};

	string currBoard = "";
	bool gameStarted = false;
	int score = 0;
	Timer timer;

	Color defaultColor = new("#bf9a63");
	Color Yellow = new("#dbea00");
	Color Green = new("#31be34");
	Color Grey = new("#9ba49a");

	GridContainer gridContainer;
	RichTextLabel scoreLabel;
	RichTextLabel wordCountLabel;
	RichTextLabel FinalScore;
	ColorRect FinalScoreBoard;
	Button BackToMenu;
	RichTextLabel GoodGame;
	VBoxContainer wordList;
	ScrollContainer scrollContainer;
	Label timeRemaining;
	bool isDragging = false;
	int wordCount = 0;
	List<Tuple<int, int>> selectedSquares;
	HashSet<string> wordsUsed;
	Trie trie;
	[Export] public Font CustomFont;

	public override void _Ready()
	{
		gridContainer = GetNode<GridContainer>("GridContainer");
		timer = GetNode<Timer>("Timer");
		defaultColor = new Color("#bf9a63");
		Yellow = new Color("#dbea00");
		Green = new Color("#31be34");
		selectedSquares = new List<Tuple<int, int>>();
		scoreLabel = GetNode<RichTextLabel>("Score");
		wordCountLabel = GetNode<RichTextLabel>("Words");
		timeRemaining = GetNode<Label>("Time");
		wordsUsed = new HashSet<string>();
		FinalScore = GetNode<RichTextLabel>("FinalScore");
		BackToMenu = GetNode<Button>("BackToMenu");
		GoodGame = GetNode<RichTextLabel>("GoodGame");
		FinalScoreBoard = GetNode<ColorRect>("FinalScoreBoard");
		scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		wordList = scrollContainer.GetChild<VBoxContainer>(0);
	}

	public override void _Process(double delta)
	{
		if (gameStarted)
		{
			timeRemaining.Text = "Time: " + Mathf.Ceil(timer.TimeLeft).ToString();
		}

	}

	public void StartGame()
	{
		if (trie == null)
		{
			Menu menu = (Menu)GetParent();
			trie = menu.trie;
		}
		gameStarted = true;
		score = 0;
		wordCount = 0;
		wordCountLabel.Text = "[color=#000000]WORDS: 0";
		scoreLabel.Text = "[color=#000000]SCORE: 0";
		wordsUsed.Clear();
		timer.Start();
		foreach (Node child in wordList.GetChildren()){
    		wordList.RemoveChild(child);
    		child.QueueFree();
		}
		BackToMenu.Visible = false;
		FinalScore.Visible = false;
		FinalScoreBoard.Visible = false;
		GoodGame.Visible = false;
		wordList.Visible = false;
		scrollContainer.Visible = false;
	}

	public void _on_timer_timeout()
	{
		gameStarted = false;
		BackToMenu.Visible = true;
		FinalScore.Text = "[center][color=#000000]SCORE: " + score.ToString();
		FinalScore.Visible = true;
		FinalScoreBoard.Visible = true;
		GoodGame.Visible = true;
		wordList.Visible = true;
		scrollContainer.Visible = true;
		List<string> sortedWords = wordsUsed
            .OrderByDescending(word => word.Length) // Sort by length
            .ThenBy(word => word)        // Optional: secondary sort alphabetically
            .ToList();
		foreach(string w in sortedWords){
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

	private void _on_back_to_menu_button_down(){
		Menu menu = (Menu)GetParent();
		menu.GameOver(currBoard);
	}

	private int indexConverter(int x, int y)
	{
		return x + y * 4;
	}

	public override void _Input(InputEvent @event)
	{

		if (!gameStarted)
		{
			return;
		}
		// Check if the event is a mouse button event (click)
		if (@event is InputEventMouseButton mouseEvent)
		{


			// Check if the left mouse button was pressed
			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
			{
				// Check if the mouse position is within the Control's rectangle
				if (GetRect().HasPoint(mouseEvent.Position))
				{
					Vector2 mousePos = GetLocalMousePosition();
					int mouseX = (int)mousePos.X - 60;
					int mouseY = (int)mousePos.Y - 80;
					if (validSquare(mouseX, mouseY))
					{
						selectSquare(mouseX / 121, mouseY / 121);
						isDragging = true;
					}

				}
			}
			if (mouseEvent.ButtonIndex == MouseButton.Left && !mouseEvent.Pressed)
			{
				isDragging = false;
				attemptWord();
			}
		}
		if (isDragging && @event is InputEventMouseMotion motionEvent)
		{
			Vector2 mousePos = GetLocalMousePosition();
			int mouseX = (int)mousePos.X - 60;
			int mouseY = (int)mousePos.Y - 80;
			if (validSquare(mouseX, mouseY))
			{
				selectSquare(mouseX / 121, mouseY / 121);
			}
		}
	}

	private bool validSquare(int x, int y)
	{
		if (x > 480 || y > 480 || x < 0 || y < 0)
		{
			return false;
		}
		if ((x > 110 && x < 128) || (x > 231 && x < 249) || (x > 352 && x < 370))
		{
			return false;
		}
		if ((y > 110 && y < 128) || (y > 231 && y < 249) || (y > 352 && y < 370))
		{
			return false;
		}
		return true;
	}

	private string getWord()
	{
		string currWord = "";
		List<int> indicies = new List<int>();
		foreach (Tuple<int, int> t in selectedSquares)
		{
			indicies.Add(indexConverter(t.Item1, t.Item2));
		}
		foreach (int i in indicies)
		{
			currWord += currBoard[i];
		}
		return currWord;
	}

	private void attemptWord()
	{
		string currWord = "";
		List<int> indicies = new List<int>();
		foreach (Tuple<int, int> t in selectedSquares)
		{
			indicies.Add(indexConverter(t.Item1, t.Item2));
		}
		if (indicies.Count > 2)
		{

			foreach (int i in indicies)
			{
				currWord += currBoard[i];
			}
			if (trie.ValidWord(currWord))
			{
				if (!wordsUsed.Contains(currWord))
				{
					wordCount++;
					if (currWord.Length == 3)
					{
						score += 100;
					}
					else
					{
						score += 400 * (currWord.Length - 3);
					}
					wordsUsed.Add(currWord);
				}

				wordCountLabel.Text = "[color=#000000]WORDS: " + wordCount.ToString();
				scoreLabel.Text = "[color=#000000]SCORE: " + score.ToString();
			}
		}
		foreach (Node child in gridContainer.GetChildren())
		{
			// Check if the child is a ColorRect
			if (child is ColorRect colorRect)
			{
				colorRect.Color = defaultColor;
			}
		}
		isDragging = false;
		selectedSquares.Clear();
	}

	private bool ValidSelection(int x, int y)
	{
		Tuple<int, int> currSquare = new Tuple<int, int>(x, y);
		if (selectedSquares.Count != 0)
		{
			if (selectedSquares.Contains(Tuple.Create(x, y)))
			{
				return false;
			}
			int lastX = selectedSquares.Last().Item1;
			int lastY = selectedSquares.Last().Item2;
			if (Math.Abs(lastX - x) <= 1 && Math.Abs(lastY - y) <= 1)
			{
				return true;
			}
		}
		else
		{
			return true;
		}
		return false;

	}

	public void selectSquare(int x, int y)
	{
		if (!ValidSelection(x, y))
		{
			return;
		}
		int squareNumber = 1 + x + 4 * y;
		string squareName = "ColorRect" + squareNumber.ToString();
		ColorRect currSquare = gridContainer.GetNode<ColorRect>(squareName);
		currSquare.Color = Grey;
		Tuple<int, int> index = new Tuple<int, int>(x, y);
		selectedSquares.Add(index);
		HandleSquares();
	}

	private void HandleSquares()
	{
		if (selectedSquares.Count < 3)
		{
			return;
		}
		string currWord = getWord();
		if (currWord.Length < 3)
		{
			return;
		}
		if (wordsUsed.Contains(currWord))
		{
			ColourSquares(Yellow);
		}
		else if (trie.ValidWord(currWord))
		{
			ColourSquares(Green);
		}
		else
		{
			ColourSquares(Grey);
		}
	}

	private void ColourSquares(Color color)
	{
		foreach (Tuple<int, int> t in selectedSquares)
		{
			int squareNumber = 1 + t.Item1 + 4 * t.Item2;
			string squareName = "ColorRect" + squareNumber.ToString();
			ColorRect currSquare = gridContainer.GetNode<ColorRect>(squareName);
			currSquare.Color = color;
		}
	}

	public void GenerateGame()
	{
		currBoard = "";
		Random random = new Random();
		char[] chars = new char[16];
		for (int i = 0; i < 16; i++)
		{
			chars[i] = letterDistribution[random.Next(0, 97)];
		}
		int count = 0;

		foreach (Node child in gridContainer.GetChildren())
		{
			if (child is ColorRect colorRect)
			{
				RichTextLabel richTextLabel = colorRect.GetNode<RichTextLabel>("RichTextLabel");
				if (richTextLabel != null)
				{
					richTextLabel.Text = "[center][color=#000000]" + chars[count].ToString();
					count++;
				}
			}
		}
		foreach (char c in chars)
		{
			currBoard += c;
		}
		StartGame();
	}
	public void GenerateGame(string GameInput)
	{
		char[] chars = new char[16];
		for (int i = 0; i < 16; i++)
		{
			chars[i] = GameInput[i];
		}
		int count = 0;
		// Loop through each child of the GridContainer
		foreach (Node child in gridContainer.GetChildren())
		{
			// Check if the child is a ColorRect
			if (child is ColorRect colorRect)
			{
				// Get the RichTextLabel child of the ColorRect
				RichTextLabel richTextLabel = colorRect.GetNode<RichTextLabel>("RichTextLabel");

				if (richTextLabel != null)
				{
					richTextLabel.Text = "[center][color=#000000]" + chars[count].ToString();
					count++;
				}
			}
		}
		currBoard = GameInput;
		StartGame();
	}
}
