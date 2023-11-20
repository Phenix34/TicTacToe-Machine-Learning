using System;
using System.Collections.Generic;
public class DecisionNode
{
    public int[,] Board { get; set; }
    public int Move { get; set; }
    public List<DecisionNode> Children { get; set; }

    public DecisionNode(int[,] board, int move)
    {
        Board = board;
        Move = move;
        Children = new List<DecisionNode>();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("¡Bienvenido al juego del tres en raya!");

        int[,] initialBoard = new int[3, 3];
        int centralCell = 5; // Celda central

        // Inicializar el tablero con el movimiento de la computadora en la celda central
        ApplyMove(initialBoard, centralCell, 2);

        while (true)
        {
            // Imprimir el tablero antes de que el jugador haga su movimiento
            PrintBoard(initialBoard);

            // Turno del jugador
            Console.WriteLine("Tu turno. Ingresa el número de la celda (1-9):");
            int playerMove = int.Parse(Console.ReadLine());

            if (!IsValidMove(initialBoard, playerMove))
            {
                Console.WriteLine("Movimiento inválido. Intenta de nuevo.");
                continue;
            }

            ApplyMove(initialBoard, playerMove, 1);

            if (IsGameOver(initialBoard))
            {
                // Imprimir el tablero después del movimiento del jugador
                PrintBoard(initialBoard);
                if (CheckWinner(initialBoard) == 1)
                    Console.WriteLine("¡Ganaste!");
                else
                    Console.WriteLine("¡Empate!");
                break;
            }

            // Generar un nuevo árbol de decisiones para el próximo movimiento de la computadora
            DecisionNode rootNode = new DecisionNode(initialBoard, centralCell);
            GenerateDecisionTree(rootNode, 2); // Comienza con el jugador 2 (computadora)


            // Turno de la computadora
            int computerMove = ChooseBestMove(rootNode);
            ApplyMove(initialBoard, computerMove, 2);

            if (IsGameOver(initialBoard))
            {
                // Imprimir el tablero después del movimiento de la computadora
                PrintBoard(initialBoard);
                if (CheckWinner(initialBoard) == 2)
                    Console.WriteLine("¡La computadora gana!");
                else
                    Console.WriteLine("¡Empate!");
                break;
            }
        }

        Console.ReadKey();
    }


    static void PrintBoard(int[,] board)
    {
        Console.WriteLine("-------------");
        for (int row = 0; row < 3; row++)
        {
            Console.Write("| ");
            for (int col = 0; col < 3; col++)
            {
                if (board[row, col] == 0)
                    Console.Write(" ");
                else if (board[row, col] == 1)
                    Console.Write("X");
                else
                    Console.Write("O");

                Console.Write(" | ");
            }
            Console.WriteLine();
            Console.WriteLine("-------------");
        }
    }

    static void GenerateDecisionTree(DecisionNode node, int currentPlayer)
    {
        for (int i = 1; i <= 9; i++)
        {
            if (IsValidMove(node.Board, i))
            {
                int[,] newBoard = (int[,])node.Board.Clone();
                ApplyMove(newBoard, i, currentPlayer);

                DecisionNode childNode = new DecisionNode(newBoard, i);
                node.Children.Add(childNode);

                if (!IsGameOver(newBoard))
                {
                    int nextPlayer = currentPlayer == 1 ? 2 : 1;
                    GenerateDecisionTree(childNode, nextPlayer);
                }
            }
        }
    }


    static bool IsValidMove(int[,] board, int move)
    {
        int row = (move - 1) / 3;
        int col = (move - 1) % 3;

        return board[row, col] == 0;
    }

    static void ApplyMove(int[,] board, int move, int player)
    {
        int row = (move - 1) / 3;
        int col = (move - 1) % 3;

        board[row, col] = player;
    }

    static int GetPlayer(DecisionNode node)
    {
        // Alternar entre jugadores
        return node.Children.Count % 2 == 0 ? 1 : 2;
    }

    static bool IsGameOver(int[,] board)
    {
        return CheckWinner(board) != 0 || IsBoardFull(board);
    }

    static int CheckWinner(int[,] board)
    {
        // Verificar filas, columnas y diagonales para determinar el ganador
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] && board[i, 0] != 0)
            {
                return board[i, 0]; // Ganador en la fila
            }

            if (board[0, i] == board[1, i] && board[1, i] == board[2, i] && board[0, i] != 0)
            {
                return board[0, i]; // Ganador en la columna
            }
        }

        if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[0, 0] != 0)
        {
            return board[0, 0]; // Ganador en la diagonal principal
        }

        if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] && board[0, 2] != 0)
        {
            return board[0, 2]; // Ganador en la diagonal secundaria
        }
        return 0; // No hay ganador
    }

    static bool IsBoardFull(int[,] board)
    {
        // Verificar si el tablero está lleno
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == 0)
                {
                    return false; // Todavía hay celdas vacías
                }
            }
        }
        return true; // El tablero está lleno
    }

    static int ChooseBestMove(DecisionNode node)
    {
        int currentPlayer = GetPlayer(node);
        int opponent = currentPlayer == 1 ? 2 : 1;

        foreach (DecisionNode child in node.Children)
        {
            if (IsWinningMove(child.Board, currentPlayer))
            {
                return child.Move; // Si el movimiento actual lleva a la victoria, selecciónalo
            }
        }

        foreach (DecisionNode child in node.Children)
        {
            if (IsWinningMove(child.Board, opponent))
            {
                return child.Move; // Si bloquea al jugador, selecciónalo
            }
        }

        // Si no es posible ganar ni bloquear al jugador, utiliza DFS para encontrar un movimiento
        return DepthFirstSearch(node, currentPlayer);
    }

    static int DepthFirstSearch(DecisionNode node, int currentPlayer)
    {
        foreach (DecisionNode child in node.Children)
        {
            // Realiza DFS recursivo para encontrar un movimiento
            int result = DepthFirstSearch(child, currentPlayer);
            if (result != -1)
            {
                return result; // Si se encuentra un movimiento, devuélvelo
            }
        }

        // Si no se encuentra ningún movimiento en el DFS, elige el primer movimiento disponible
        return node.Children.Count > 0 ? node.Children[0].Move : -1;
    }





    static bool IsWinningMove(int[,] board, int player)
    {
        // Verificar si el jugador actual ha ganado en el tablero dado
        return CheckWinner(board) == player;
    }

}
