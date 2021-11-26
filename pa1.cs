#nullable enable
using System;
using static System.Console;
using static System.Math;

namespace Bme121
{
    record Player( string Colour, string Symbol, string Name );
    /*
    class Player
    {
        public readonly string Colour;
        public readonly string Symbol;
        public readonly string Name;
        
        public Player( string Colour, string Symbol, string Name )
        {
            this.Colour = Colour;
            this.Symbol = Symbol;
            this.Name = Name;
        }
    }
    */

    static partial class Program
    {
        // Display common text for the top of the screen.
        
        static void Welcome( )
        {
            WriteLine("Welcome to Othello!");
            WriteLine(" ");
        }
        
        // Collect a player name or default to form the player record.
        
        static Player NewPlayer( string colour, string symbol, string defaultName )
        {
            Write($"Type the {colour} disk ({symbol}) player name or <enter> for '{defaultName}': ");
            string pInfo = ReadLine();

            if ( pInfo.Length == 0)
            {
                return new Player(colour, symbol, defaultName);
            }
            else
            {
                pInfo = defaultName;
                return new Player(colour, symbol, defaultName);
            }
        }
        
        // Determine which player goes first or default.
        // int turn = GetFirstTurn( players, defaultFirst: 0 );
        static int GetFirstTurn( Player[ ] players, int defaultFirst )
        {
            Write($"Choose who will play first or <enter> for {players[0].Colour}/{players[0].Symbol}/{players[0].Name}: ");
            string playerFirst = ReadLine();

            if ( playerFirst == "White" || playerFirst == "white")
            {
                defaultFirst = 1;
                return GetFirstTurn( players, defaultFirst);
            }
            return defaultFirst;
        }
        
        // Get a board size (between 4 and 26 and even) or default, for one direction.
        
        static int GetBoardSize( string direction, int defaultSize )
        {
            Write($"Enter board {direction} (4 to 26, even) or <enter> for {defaultSize}: ");
            string directionValue = ReadLine();

            if (directionValue.Length > 0 )
            {
                defaultSize = int.Parse(directionValue);

                if (defaultSize >= 4 && defaultSize <= 26 && defaultSize % 2 == 0)
                {
                defaultSize = int.Parse(directionValue);
                return defaultSize;
                }
                else
                {
                    WriteLine($"(Your input is invaled! Using default value: 8)");
                    defaultSize = 8;
                    return defaultSize;
                }
            }
            else
            {
                WriteLine("Using default value: 8");
                return defaultSize;
            }
        }
        // Get a move from a player.
        
        static string GetMove( Player player )
        {
            WriteLine($"Turn is {player.Colour} disc ({player.Symbol}) player: {player.Name}");
            WriteLine("Pick a cell by its row then column name (like 'bc') to play there");
            WriteLine("Use 'skip' to give up your turn. Use 'quit' to end the game.");
            Write("Enter your choice: ");
            string move = ReadLine();
            return move;
        }

        static bool TryMove( string[ , ] board, Player player, string move )
        {

            int r = IndexAtLetter( move[0].ToString() );
            int c = IndexAtLetter( move[1].ToString() );

            if ( move == "skip" || move == "Skip")
            {
                return true;
            }
            if (move.Length != 2)
            {
                return false;
            }
            if ( r == -1 || c == -1)
            {
                return false;
            }
            if ( r > board.GetLength(0)  || c > board.GetLength(1) )
            {
                return false;
            }
            if (board[ r, c ] == player.Symbol)
            {
                return false;
            }
            if ( board[ r, c ] == " ")
            {
                bool Direction1 = TryDirection ( board, player, r, 0, c, 1);
                bool Direction2 = TryDirection ( board, player, r, 0, c, -1);
                bool Direction3 = TryDirection ( board, player, r, 1, c, 0);
                bool Direction4 = TryDirection ( board, player, r, -1, c, 0);
                bool Direction5 = TryDirection ( board, player, r, 1, c, -1);
                bool Direction6 = TryDirection ( board, player, r, 1, c, 1);
                bool Direction7 = TryDirection ( board, player, r, -1, c, -1);
                bool Direction8 = TryDirection ( board, player, r, -1, c, 1);

                if ( Direction1 || Direction2 || Direction3 || Direction4 || Direction5 || Direction6 || Direction7 || Direction8 )
                {
                    return true;
                }
            }
            return false;
        }

        static bool TryDirection( string[ , ] board, Player player,
            int moveRow, int deltaRow, int moveCol, int deltaCol )
        {
            int currentRow = moveRow;
            int currentCol = moveCol;
            int countFlips = 0;
            bool foundEnd = false; //true when I find my symbol

            while( ! foundEnd)
            {
                //where is next cell in this direction?
                currentRow = currentRow + deltaRow;
                currentCol = currentCol + deltaCol;

                // is this cell actually on the board?
                if (currentRow < 0                      ) return false;
                if (currentRow > board.GetLength(0) - 1 ) return false;
                if (currentCol < 0                      ) return false; 
                if (currentCol > board.GetLength(1) - 1 ) return false;

                // is this cell empty?
                if ( board[ currentRow, currentCol ] == " ") return false;

                // is this cell flippable, or me?
                if( board[ currentRow, currentCol ] != player.Symbol ) countFlips ++;
                else // found my symbol
                {
                    foundEnd = true;
                }
            }

            if ( countFlips == 0) return false;
            
            // loop to do flips

            currentRow = moveRow;
            currentCol = moveCol;

            for( int i = 0; i < countFlips + 1; i++ )
            {
                board[ currentRow, currentCol ] = player.Symbol;
                // Where is the next cell in this direction?
                currentRow = currentRow + deltaRow;
                currentCol = currentCol + deltaCol;

                // the flip
                 
                //System.Threading.Thread.Sleep(1000 /*ms */);
                //DisplayBoard( board );
            }
            return true;
        }
        
        // Count the discs to find the score for a player.
        
        static int GetScore( string[ , ] board, Player player )
        {
            int blackScore = 0;
            int whiteScore = 0;

            for( int r = 0; r < board.GetLength( 0 ); r++ )
            {
                for( int c = 0; c < board.GetLength( 1 ); c++ )
                {
                    if( board[ r, c] == "X" )
                    {
                        blackScore ++;
                    }
                    else if ( board[ r, c] == "O")
                    {
                        whiteScore ++;
                    }
                }
            }
            if ( player.Symbol == "X")
            {
                return blackScore;
            }
            else
            {
                return whiteScore;
            }
        }
        
        // Display a line of scores for all players.
        
        static void DisplayScores( string[ , ] board, Player[ ] players )
        {
            
            Write("Scores: ");
            for ( int x = 0; x < players.Length; x++)
            {
            Write("{0} {1} ", players[x].Name, GetScore(board, players[x] ) );
            }
            WriteLine();
        }
        // Display winner(s) and categorize their win over the defeated player(s).
        
        static void DisplayWinners( string[ , ] board, Player[ ] players )
        {
            int player1 = GetScore(board, players[0]);
            int player2 = GetScore(board, players[1]);

            if ( player1 > player2 )
            {
                Write($"{players[0].Name} wins!");
            }
            else if ( player1 == player2)
            {
                Write("Tie!");
            }
            else
            {
                Write($"{players[1].Name} wins!");
            }

            int compare = Abs(player1 - player2);

            if (54 < compare && compare < 64) Write("A PERFECT GAME");
            else if (40 < compare && compare < 52) Write("A WALKAWAY GAME");
            else if (26 < compare && compare < 38) Write("A FIGHT GAME");
            else if (12 < compare && compare < 24) Write("A HOT GAME");
            else if (2 < compare && compare < 10) Write("A CLOSE GAME");

            

            return;
        }
        
        static void Main( )
        {
            Welcome( );
            
            Player[ ] players = new Player[ ] 
            {
                NewPlayer( colour: "black", symbol: "X", defaultName: "Black" ),
                NewPlayer( colour: "white", symbol: "O", defaultName: "White" ),
            };
            
            int turn = GetFirstTurn( players, defaultFirst: 0 );
           
            int rows = GetBoardSize( direction: "rows",    defaultSize: 8 );
            int cols = GetBoardSize( direction: "columns", defaultSize: 8 );
            
            string[ , ] game = NewBoard( rows, cols );
            
            // Play the game.
            
            bool gameOver = false;
            while( ! gameOver )
            {
                DisplayBoard( game ); 
                DisplayScores( game, players );
                
                string move = GetMove( players[ turn ] );
                if( move == "quit" ) gameOver = true;
                else
                {
                    bool madeMove = TryMove( game, players[ turn ], move );
                    if( madeMove ) turn = ( turn + 1 ) % players.Length;
                    else 
                    {
                        Write( "Your choice didn't work!" );
                        Write( "Press <Enter> to try again." );
                        ReadLine( ); 
                    }
                }
            }
            
            // Show fhe final results.
            
            DisplayWinners( game, players );
            WriteLine( );
        }
    }
}
