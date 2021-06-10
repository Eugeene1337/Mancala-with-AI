using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Lab3
{
    class GameManager
    {
        public int[] TopPockets { get; set; }
        public int[] BottomPockets { get; set; }
        public bool BottomMove { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public int DecisionChosen { get; set; }
        public int BestMove { get; set; }
        public bool MaximizingBottom { get; set; }

        private bool _endsInEmpty { get; set; }
        private bool _endsInBase { get; set; }

        public static int DifficultyLevel { get; set; } = 1; // 1 - Normal (Default); 2 - Hard


        public GameManager(int[] topPockets, int[] bottomPockets, Player player1, Player player2)
        {
            TopPockets = topPockets;
            BottomPockets = bottomPockets;
            Player1 = player1;
            Player2 = player2;
            BottomMove = RandomStartTurn();
        }

        public int AlphaBeta(GameManager state, int depth,int alpha,int beta, bool maximizingPlayer)
        {
            if (depth == 0)
            {
                return EstimateState(state);
            }
            if (maximizingPlayer)
            {
                List<GameManager> childrenStates;
                if (MaximizingBottom)
                {
                    childrenStates = CreateChildrenStates(state.BottomMove, state);
                }
                else
                {
                    childrenStates = CreateChildrenStates(!state.BottomMove, state);
                }
                int value;
                int bestMove = 0;
                foreach (GameManager child in childrenStates)
                {
                    if (MaximizingBottom)
                    {
                        value = AlphaBeta(child, depth - 1,alpha,beta, child.BottomMove);
                    }
                    else
                    {
                        value = AlphaBeta(child, depth - 1,alpha,beta, !child.BottomMove);
                    }
                    //bestValue = Math.Max(bestValue, value);
                    if (value > alpha)
                    {
                        alpha = value;
                        bestMove = child.DecisionChosen;
                    }
                    if (alpha > beta)
                    {
                        break;
                    }

                }
                BestMove = bestMove;
                return alpha;
            }
            else
            {
                List<GameManager> childrenStates;
                if (MaximizingBottom)
                {
                    childrenStates = CreateChildrenStates(state.BottomMove, state);
                }
                else
                {
                    childrenStates = CreateChildrenStates(!state.BottomMove, state);
                }
                
                int value;
                foreach (GameManager child in childrenStates)
                {
                    if (MaximizingBottom)
                    {
                        value = AlphaBeta(child, depth - 1, alpha, beta, child.BottomMove);
                    }
                    else
                    {
                        value = AlphaBeta(child, depth - 1, alpha, beta, !child.BottomMove);
                    }

                    beta = Math.Min(beta, value);
                    if (alpha > beta)
                    {
                        break;
                    }
                }
                return beta;
            }
        }

        public int MinMax(GameManager state,int depth,bool maximizingPlayer)
        {
            if(depth == 0) 
            {
                return EstimateState(state);
            }
            if (maximizingPlayer)
            {
                List<GameManager> childrenStates;
                if (MaximizingBottom)
                {
                    childrenStates = CreateChildrenStates(state.BottomMove, state);
                }
                else
                {
                    childrenStates = CreateChildrenStates(!state.BottomMove, state);
                }
                int bestValue = Int32.MinValue;
                int value;
                int bestMove = 0;
                foreach(GameManager child in childrenStates)
                {
                    if (MaximizingBottom)
                    {
                        value = MinMax(child, depth - 1, child.BottomMove);
                    }
                    else
                    {
                        value = MinMax(child, depth - 1, !child.BottomMove);
                    }
                    //bestValue = Math.Max(bestValue, value);
                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestMove = child.DecisionChosen;
                    }
                    
                }
                BestMove = bestMove;
                return bestValue;
            }
            else
            {
                List<GameManager> childrenStates;
                if (MaximizingBottom)
                {
                    childrenStates = CreateChildrenStates(state.BottomMove, state);
                }
                else
                {
                    childrenStates = CreateChildrenStates(!state.BottomMove, state);
                }
                int bestValue = Int32.MaxValue;
                int value;
                foreach (GameManager child in childrenStates)
                {
                    if (MaximizingBottom)
                    {
                        value = MinMax(child, depth - 1, child.BottomMove);
                    }
                    else
                    {
                        value = MinMax(child, depth - 1, !child.BottomMove);
                    }
                    
                    bestValue = Math.Min(bestValue, value);
                }
                return bestValue;
            }
        }

        private List<GameManager> CreateChildrenStates(bool maximizingPlayer, GameManager state)
        {
            List<GameManager> childrenStates = new List<GameManager>();
            List<int> choisesMovement = new List<int>();
            if (MaximizingBottom)
            {
                if (maximizingPlayer)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (BottomPockets[i] != 0)
                        {
                            choisesMovement.Add(i);
                        }
                    }
                    foreach (int move in choisesMovement)
                    {
                        int[] topPocketsCopy = new int[6];
                        TopPockets.CopyTo(topPocketsCopy, 0);
                        int[] botPocketsCopy = new int[6];
                        BottomPockets.CopyTo(botPocketsCopy, 0);
                        GameManager child = new GameManager(topPocketsCopy, botPocketsCopy, new Player(state.Player1.Name, state.Player1.AI, state.Player1.Score), new Player(state.Player2.Name, state.Player2.AI, state.Player2.Score))
                        {
                            BottomMove = true,
                            DecisionChosen = move
                        };
                        child.Step(move);
                        childrenStates.Add(child);
                    }
                }
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (TopPockets[i] != 0)
                        {
                            choisesMovement.Add(i);
                        }
                    }
                    foreach (int move in choisesMovement)
                    {
                        int[] topPocketsCopy = new int[6];
                        TopPockets.CopyTo(topPocketsCopy, 0);
                        int[] botPocketsCopy = new int[6];
                        BottomPockets.CopyTo(botPocketsCopy, 0);
                        GameManager child = new GameManager(topPocketsCopy, botPocketsCopy, new Player(state.Player1.Name, state.Player1.AI, state.Player1.Score), new Player(state.Player2.Name, state.Player2.AI, state.Player2.Score))
                        {
                            BottomMove = false,
                            DecisionChosen = move
                        };
                        child.ReflectiveStep(move);
                        childrenStates.Add(child);
                    }
                }
            }
            else
            {
                if (maximizingPlayer)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (TopPockets[i] != 0)
                        {
                            choisesMovement.Add(i);
                        }
                    }
                    foreach (int move in choisesMovement)
                    {
                        int[] topPocketsCopy = new int[6];
                        TopPockets.CopyTo(topPocketsCopy, 0);
                        int[] botPocketsCopy = new int[6];
                        BottomPockets.CopyTo(botPocketsCopy, 0);
                        GameManager child = new GameManager(topPocketsCopy, botPocketsCopy, new Player(state.Player1.Name, state.Player1.AI, state.Player1.Score), new Player(state.Player2.Name, state.Player2.AI, state.Player2.Score))
                        {
                            BottomMove = false,
                            DecisionChosen = move
                        };
                        child.ReflectiveStep(move);
                        childrenStates.Add(child);
                    }
                }
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (BottomPockets[i] != 0)
                        {
                            choisesMovement.Add(i);
                        }
                    }
                    foreach (int move in choisesMovement)
                    {
                        int[] topPocketsCopy = new int[6];
                        TopPockets.CopyTo(topPocketsCopy, 0);
                        int[] botPocketsCopy = new int[6];
                        BottomPockets.CopyTo(botPocketsCopy, 0);
                        GameManager child = new GameManager(topPocketsCopy, botPocketsCopy, new Player(state.Player1.Name, state.Player1.AI, state.Player1.Score), new Player(state.Player2.Name, state.Player2.AI, state.Player2.Score))
                        {
                            BottomMove = true,
                            DecisionChosen = move
                        };
                        child.Step(move);
                        childrenStates.Add(child);
                    }
                }
            }
            
            return childrenStates;
        }

        int EstimateState(GameManager state)
        {
            int rating = 0;
            if (MaximizingBottom)
            {
               rating = state.Player1.Score - state.Player2.Score;
            }
            else
            {
                rating = state.Player2.Score - state.Player1.Score;
            }
            if (DifficultyLevel == 1)
            {
                return rating;
            }
            if (_endsInEmpty)
            {
                rating += 10;
            }
            if (_endsInBase)
            {
                rating += 5;
            }
            return rating;
        }

        bool RandomStartTurn() => new Random().Next(2) == 1;

        public bool Step(int index)
        {
            bool success = false;
            bool endsInBase = false;
            bool endsInEmpty = false;
            if (BottomMove)
            {
                //if index out of range -> exit
                if(!(index>=0) || !(index < 6))
                {
                    return false;
                }
                //if pocket is empty -> exit
                if (BottomPockets[index] == 0)
                {
                    return false;
                }

                success = true;
                int stones = BottomPockets[index];
                BottomPockets[index] = 0;
                endsInBase = EndsInBase(index, stones);
                endsInEmpty = EndsInEmpty(index, stones);
                _endsInEmpty = endsInEmpty;
                _endsInBase = endsInBase;
                int lastIndex = LastIndex(index, stones);
                if (lastIndex > 12 && lastIndex < 19)
                {
                    lastIndex %= 13;
                }

                while (stones > 0)
                {
                    stones = MoveStones(index+1, stones, BottomPockets);
                    if (stones > 0)
                    {
                        stones = ScoreIncrement(stones, Player1);
                    }
                    if (stones > 0)
                    {
                        stones = MoveStones(0, stones, TopPockets);
                    }
                }

                if (endsInEmpty)
                {
                    TakeOppositeStones(lastIndex);
                }
            }
            if (endsInBase)
            {
                BottomMove = true;
            }
            else
            {
                BottomMove = false;
            }
            return success;
        }

        public bool ReflectiveStep(int index)
        {
            bool success = false;
            bool endsInBase = false;
            bool endsInEmpty = false;
            if (!BottomMove)
            {
                //if index out of range -> exit
                if (!(index >= 0) || !(index < 6))
                {
                    return false;
                }
                //if pocket is empty -> exit
                if (TopPockets[index] == 0)
                {
                    return false;
                }

                success = true;
                int stones = TopPockets[index];
                TopPockets[index] = 0;
                endsInBase = EndsInBase(index, stones);
                endsInEmpty = EndsInEmpty(index, stones);
                _endsInEmpty = endsInEmpty;
                _endsInBase = endsInBase;

                int lastIndex = LastIndex(index, stones);
                if (lastIndex > 12 && lastIndex < 19)
                {
                    lastIndex %= 13;
                }

                while (stones > 0)
                {
                    stones = MoveStones(index+1, stones, TopPockets);
                    if (stones > 0)
                    {
                        stones = ScoreIncrement(stones, Player2);
                    }
                    if (stones > 0)
                    {
                        stones = MoveStones(0, stones, BottomPockets);
                    }
                }

                if (endsInEmpty)
                {
                    TakeOppositeStones(lastIndex);
                }
            }
            if (endsInBase)
            {
                BottomMove = false;
            }
            else
            {
                BottomMove = true;
            }
            return success;
        }

        int ScoreIncrement(int stones, Player player)
        {
            if (stones > 0)
            {
                player.Score++;
                return --stones;
            }
            return 0;
        }
        int MoveStones(int index, int stones, int [] array)
        {
            int stonesCounter = stones;
            if(index >= 0 && index < 6)
            {
                for (int i = index; i < array.Length; i++)
                {
                    if (stonesCounter > 0)
                    {
                        array[i]++;
                        stonesCounter--;
                    }
                }
            }
            else
            {
                return stonesCounter;
            }

            return stonesCounter;
            
        }

        bool EndsInBase(int index, int stones)
        {
            if (index + stones < 7)
            {
                if ((index + stones) % 6 == 0) { return true; }
            }
            else if(index + stones>12)
            {
                if (index + stones != 13)
                {
                    if ((index + stones) % 6 == 1) { return true; }
                } 
            }
            return false;     
            
        }
        int LastIndex(int index, int stones) => index + stones % 13;
       
        bool EndsInEmpty(int index, int stones)
        {
            int lastIndex = LastIndex(index, stones);
            if (BottomMove)
            {
                if (lastIndex < 6)
                {
                    if(BottomPockets[lastIndex] == 0)
                    {
                        return true;
                    }
                }
                else if (lastIndex > 12 && lastIndex < 19)
                {
                    lastIndex %= 13;
                    if (BottomPockets[lastIndex] == 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (lastIndex < 6)
                {
                    if (TopPockets[lastIndex] == 0)
                    {
                        return true;
                    }
                }
                else if(lastIndex>12 && lastIndex < 19)
                {
                    lastIndex %= 13;
                    if (TopPockets[lastIndex] == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        void TakeOppositeStones(int index)
        {
            if (BottomMove)
            {
                if(TopPockets[5 - index] != 0)
                {
                    BottomPockets[index] = 0;
                    Player1.Score += TopPockets[5 - index] + 1;
                    TopPockets[5 - index] = 0;
                }
                
            }
            else
            {
                if (BottomPockets[5 - index] != 0)
                {
                    TopPockets[index] = 0;
                    Player2.Score += BottomPockets[5 - index] + 1;
                    BottomPockets[5 - index] = 0;
                }
                    
            }
        }

        public bool Finish()
        {
            if(ArrayEqualsZeros(BottomPockets) || ArrayEqualsZeros(TopPockets))
            {
                return true;
            }
            return false;
        }

        bool ArrayEqualsZeros(int[] arr)
        {
            bool zeros = true;
            foreach (int i in arr)
            {
                if (i != 0)
                {
                    zeros = false;
                }
            }
            return zeros;
        }

        public int CountStonesOnBoard(int [] arr)
        {
            int sum = 0;
            foreach(int i in arr)
            {
                sum += i;
            }
            return sum;
        }
    }
}
