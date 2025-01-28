using Godot;
using System.Collections.Generic;
using System.Linq;

namespace WordHunt;
public partial class Solver : Node
{
	
	Menu menuNode;
	Trie SolverTrie;
	int totalCalls = 0;

	int MinWordLength = 3;

	private char spaceSearchedChar = '!';
	public void instantiateSolver(Trie solverTrie){
		SolverTrie = solverTrie;
		validWords = new HashSet<string>();
	}

	public HashSet<string> validWords;
	public List<string> solve(string letters){
		List<string> res = new();
		validWords.Clear();
		if(letters.Length != 16){
			return res;
		}
		char[] board = letters.ToCharArray();
		for(int i = 0; i < 4; i++){
			for(int j = 0; j < 4; j++){
				DFS(i,j,"",board,SolverTrie);
			}
		}
		res = validWords.OrderByDescending(word => word.Length)
            .ThenBy(word => word)
            .ToList();
		return res;
	}

	public void DFS(int row, int col, string currWord, char[] board, Trie currNode){
		
		//totalCalls++; //Tracking the number of calls to the DFS function for debugging
		
		if(row < 0 || col < 0 || row >= 4 || col >= 4 || currNode == null || currNode.Children == null){
			return;
		}

		char currChar = board[4*row + col];
		
		//If the current character is already searched or the current character is not in the trie, return
		if(currChar == spaceSearchedChar || !currNode.Children.ContainsKey(currChar)){ 
			return;
		}

		currWord += currChar;

	
		if(currWord.Length >= MinWordLength){
			if(currNode.Children[currChar].validWord){
				validWords.Add(currWord);
			}
		}
		board[4*row + col] = spaceSearchedChar;
		DFS(row - 1, col - 1, currWord, board, currNode.Children[currChar]);
		DFS(row - 1, col + 0, currWord, board, currNode.Children[currChar]);
		DFS(row - 1, col + 1, currWord, board, currNode.Children[currChar]);
		DFS(row + 0, col - 1, currWord, board, currNode.Children[currChar]);
		DFS(row + 0, col + 1, currWord, board, currNode.Children[currChar]);
		DFS(row + 1, col - 1, currWord, board, currNode.Children[currChar]);
		DFS(row + 1, col + 0, currWord, board, currNode.Children[currChar]);
		DFS(row + 1, col + 1, currWord, board, currNode.Children[currChar]);						
		board[4*row + col] = currChar;
		
	}
}
