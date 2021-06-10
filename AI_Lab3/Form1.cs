using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI_Lab3
{
    //GUI
    public partial class Form1 : Form
    {
        List<Button> buttons;
        bool Finish = false;
        bool GameEnabled;
        bool GameStarted = false;
        bool FirstMove = true;
        int mode = 1; // 1 - AI vs AI; 2 - Player vs AI
        int algorithm = 1; // 1 - AlphaBeta; 2 - MinMax
        int deep = 2;

        Player player1;
        Player player2;
        GameManager gameManager { get; set; }
        public Form1()
        {
            InitializeComponent();
            buttons = new List<Button>()
            {
                button1,button2,button3,button4,button5,button6,button7,button8,button9,button10,button11,button12
            };  
        }

        //Start button
        private void button15_Click(object sender, EventArgs e)
        {
            if (!GameStarted)
            {
                GameStarted = true;
                if (mode == 2)
                {
                    player1 = new Player("Player", false);
                    player2 = new Player("AI", true);
                    
                }
                else
                {
                    player1 = new Player("AI1", true);
                    player2 = new Player("AI2", true);
                }
                gameManager = new GameManager(new int[6] { 4, 4, 4, 4, 4, 4 }, new int[6] { 4, 4, 4, 4, 4, 4 }, player1, player2);
                label1.Text = player1.Name;
                label2.Text = player2.Name;
                comboBox1.Enabled=false;
                button15.Text = "Stop game";
                timer1.Start();
                UpdateScene();
            }
            else
            {
                if (GameEnabled)
                {
                    GameEnabled = false;
                    timer1.Stop();
                    button15.Text = "Resume game";
                    UpdateScene();
                }
                else
                {
                    GameEnabled = true;
                    timer1.Start();
                    button15.Text = "Stop game";
                    UpdateScene();
                }
            }
        }

        //Timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateScene();
            if (gameManager.BottomMove)
            {
                if (gameManager.Player1.AI)
                {
                    if (FirstMove && mode==1)
                    {
                        FirstMove = false;
                        int stepAI = Player.RandomStepAI();
                        gameManager.Step(stepAI);
                        LogListBox.Items.Add($"{gameManager.Player1.Name} move: {stepAI}");
                        UpdateScene();
                        return;
                    }
                    gameManager.MaximizingBottom = true;
                    if (algorithm == 1)
                    {
                        gameManager.AlphaBeta(gameManager, deep, Int32.MinValue, Int32.MaxValue, true);
                    }
                    else
                    {
                        gameManager.MinMax(gameManager, deep, true);
                    }
                    
                    gameManager.Step(gameManager.BestMove);
                    LogListBox.Items.Add($"{gameManager.Player1.Name} move: {gameManager.BestMove}");
                    UpdateScene();
                }
                else
                {
                    EnableButtons();
                }
                
            }
            else
            {
                if (FirstMove && mode == 1)
                {
                    FirstMove = false;
                    int stepAI = Player.RandomStepAI();
                    gameManager.ReflectiveStep(stepAI);
                    LogListBox.Items.Add($"{gameManager.Player2.Name} move: {stepAI}");
                    UpdateScene();
                    return;
                }
                gameManager.MaximizingBottom = false;
                if (algorithm == 1)
                {
                    gameManager.AlphaBeta(gameManager, deep, Int32.MinValue, Int32.MaxValue, true);
                }
                else
                {
                    gameManager.MinMax(gameManager, deep, true);
                }
                gameManager.ReflectiveStep(gameManager.BestMove);
                LogListBox.Items.Add($"{gameManager.Player2.Name} move: {gameManager.BestMove}");
                UpdateScene();

            }

            if (gameManager.Finish())
            {
                Finish = true;
            }
            if (Finish)
            {
                Player player1 = gameManager.Player1;
                Player player2 = gameManager.Player2;
                player1.Score += gameManager.CountStonesOnBoard(gameManager.BottomPockets);
                player2.Score += gameManager.CountStonesOnBoard(gameManager.TopPockets);
                UpdateScene();
                if (player1.Score > player2.Score)
                {
                    AutoClosingMessageBox.Show($"Player {player1.Name} win ! \n {player1.Name}: {player1.Score} \n {player2.Name}: {player2.Score}",null, 4000);
                    timer1.Stop();
                    PlayerMoveLabel.Text = $"Player {player1.Name} win !";
                    
                }
                else if(player1.Score == player2.Score)
                {
                    AutoClosingMessageBox.Show($"Draw ! \n {player1.Name}: {player1.Score} \n {player2.Name}: {player2.Score}", null, 4000);
                    timer1.Stop();
                    PlayerMoveLabel.Text = $"Draw !";
                }
                else
                {
                    AutoClosingMessageBox.Show($"Player {player2.Name} win ! \n {player1.Name}: {player1.Score} \n {player2.Name}: {player2.Score}",null,4000);
                    timer1.Stop();
                    PlayerMoveLabel.Text = $"Player {player2.Name} win !";
                }
                
                ClearBoard();
                Finish = false;
                
            } 
        }

        private void UpdateScene()
        {
            if (gameManager.BottomMove && !player1.AI)
            {
                EnableButtons();
            }
            else
            {
                DisableButtons();
            }
            ChangeButtonsText();
            ChangeScore(gameManager.Player1, gameManager.Player2);
            if (gameManager.BottomMove)
            {
                PlayerMoveLabel.Text = $"{gameManager.Player1.Name} move";
            }
            else
            {
                PlayerMoveLabel.Text = $"{gameManager.Player2.Name} move";
            }
        }

        private void ChangeButtonsText()
        {
            //concatenate two arrays
            int [] allPockets = new int[gameManager.BottomPockets.Length + gameManager.TopPockets.Length];
            gameManager.BottomPockets.CopyTo(allPockets, 0);
            gameManager.TopPockets.CopyTo(allPockets, gameManager.BottomPockets.Length);

            for(int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Text = allPockets[i].ToString();
            }
        }

        private void ChangeScore(Player player1,Player player2)
        {
            
            button13.Text = player1.Score.ToString();
            button14.Text = player2.Score.ToString();
        }

        private void DisableButtons()
        {
            for(int i = 0; i < 6; i++)
            {
                buttons[i].Enabled = false;
            }
        }

        private void EnableButtons()
        {
            for (int i = 0; i < 6; i++)
            {
                buttons[i].Enabled = true;
            }
        }

        private void ClearBoard()
        {
            foreach(Button b in buttons)
            {
                b.Text = "0";
            }
        }

        //Dropdown menu
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox1.SelectedItem.ToString();
            if (selectedState == "AI vs AI")
            {
                mode = 1;
            }
            else
            {
                mode = 2;
            }
        }

        

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox2.SelectedItem.ToString();
            if (selectedState == "AlphaBeta")
            {
                algorithm = 1;
            }
            else
            {
                algorithm = 2;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox3.SelectedItem.ToString();
            int result =0;
            int.TryParse(selectedState, out result);
            deep = result;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox4.SelectedItem.ToString();
            if (selectedState == "Normal")
            {
                GameManager.DifficultyLevel = 1;
            }
            else
            {
                GameManager.DifficultyLevel = 2;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            gameManager.Step(0);
            LogListBox.Items.Add($"{gameManager.Player1.Name} move: 0");
            UpdateScene();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gameManager.Step(1);
            LogListBox.Items.Add($"{gameManager.Player1.Name} move: 1");
            UpdateScene();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            gameManager.Step(2);
            LogListBox.Items.Add($"{gameManager.Player1.Name} move: 2");
            UpdateScene();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            gameManager.Step(3);
            LogListBox.Items.Add($"{gameManager.Player1.Name} move: 3");
            UpdateScene();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            gameManager.Step(4);
            LogListBox.Items.Add($"{gameManager.Player1.Name} move: 4");
            UpdateScene();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            gameManager.Step(5);
            LogListBox.Items.Add($"{gameManager.Player1.Name} move: 5");
            UpdateScene();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        
    }
}
