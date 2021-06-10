using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Lab3
{
    class Player
    {
        public string Name { get; set; }
        public bool AI { get; private set; }
        public int Score { get; set; } = 0;


        public Player(string name, bool aI)
        {
            Name = name;
            AI = aI;
        }

        public Player(string name, bool aI, int score)
        {
            Name = name;
            AI = aI;
            Score = score;
        }

        public static int RandomStepAI()
        {
            return new Random().Next(6);
        }

        
    }
}
