using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Solver : Node
{
	
	Menu menuNode;
	Trie SolverTrie;
	int totalCalls = 0;
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
		totalCalls++;
		if(row < 0 || col < 0 || row >= 4 || col >= 4){
			return;
		}
		char currChar = board[4*row + col];
		if(currChar == '!'){
			return;
		}
		currWord += currChar;
		if(currNode.Children == null){
			return;
		}

		if(!currNode.Children.ContainsKey(currChar)){
			return;
		}
		if(currWord.Length >= 3){
			if(currNode.Children[currChar].validWord){
				validWords.Add(currWord);
			}
		}
		board[4*row + col] = '!';
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
