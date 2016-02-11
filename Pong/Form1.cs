/*
 * Description:     A basic PONG simulator
 * Author:   Quigley        
 * Date:     February 4th 2016       
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using WMPLib;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        bool start = true;

        //paddle position variables
        int paddle1Y, paddle2Y;

        //ball position variables
        int ballX, ballY;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions
        Boolean ballMoveRight = false;
        Boolean ballMoveDown = false;

        //constants used to set size and speed of paddles 
        int PADDLE_LENGTH1 = 60;
        int PADDLE_LENGTH2 = 60;
        const int PADDLE_WIDTH = 10;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle
        int PADDLE_SPEED1 = 10;
        int PADDLE_SPEED2 = 10;

        //constants used to set size and speed of ball 
        const int BALL_SIZE = 10;
        const int BALL_SPEED = 15;

        //player scores
        int player1Score = -1;
        int player2Score = -1;

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        //game ends after a player receives 8 points
        int gameWinScore = 8;

        //brush for paint method
        SolidBrush drawBrush = new SolidBrush(Color.Black);
        #endregion

        #region music
        //MP3 Player background
        WindowsMediaPlayer mp3Player;
        Thread musicThread;

        /// <summary>
        /// Plays the soundtrack
        /// </summary>
        public void backMusic()
        {
            mp3Player = new WindowsMediaPlayer();
            mp3Player.URL = "Chelsea Dagger HQ.mp3";
            mp3Player.settings.setMode("loop", true);
        }
        #endregion

        public Form1()
        {
            //Allows the music to play without affecting the functions of the game
            musicThread = new Thread(backMusic);
            musicThread.Start();
            while (!musicThread.IsAlive)
            {
            }
            InitializeComponent();
            SetParameters();        
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                    if (newGameOk)
                    {
                        GameStart();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
                case Keys.Space:
                    if (newGameOk)
                    {
                        GameStart();
                    }
                    break;
                default:
                    break;
            }
        }
        
        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets up the game objects in their start position, resets the scores, displays a 
        /// countdown, and then starts the game timer.
        /// </summary>
        private void GameStart()
        {

            newGameOk = true;
            SetParameters();

            // TODO create code to make a graphics object, a brush, and a font to display the countdown
            Graphics formGraphics = this.CreateGraphics();
            Font drawFont = new Font("Courier New", 30, FontStyle.Regular);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            
            //Sets the menu text and images to invisibe and makes the score labels visible
            startLabel.Visible = false;
            glassesImage.Visible = false;
            Refresh();

            //countdown to start of game
            //This counts down from 3 to give the user some time to get into gaming mode
            for (int i = 3; i > 0; i--)
            {
                formGraphics.DrawString(Convert.ToString(i), drawFont, drawBrush, 300, 200);
                Thread.Sleep(1000);
                Refresh();
            }
     
            // TODO start the gameUpdateLoop timer
            gameUpdateLoop.Enabled = true;
            newGameOk = false;
            player1Label.Visible = true;
            player2Label.Visible = true;
            Refresh();
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                // sets location of score labels to the middle of the screen
                player1Label.Location = new Point(this.Width / 2 - player1Label.Size.Width - 10, player1Label.Location.Y);
                player2Label.Location = new Point(this.Width / 2 + 10, player2Label.Location.Y);

                //set label, score variables, and ball position
                player1Score = player2Score = 0;
                player1Label.Text = "Player 1:  " + player1Score;
                player2Label.Text = "Player 2:  " + player2Score;

                //resets the paddle length to start the game
                PADDLE_LENGTH1 = 60;
                PADDLE_LENGTH2 = 60;

                paddle1Y = this.Height / 2 - PADDLE_LENGTH1 / 2;
                paddle2Y = this.Height / 2 - PADDLE_LENGTH2 / 2;

            }
            else
            {
                //sets the ball at the starting point of the screen
                ballY = this.Height / 2 - BALL_SIZE;
                ballX = this.Width / 2 + BALL_SIZE;
            }

        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            //sound player to be used for all in game sounds initially set to collision sound
            SoundPlayer player = new SoundPlayer();
            player = new SoundPlayer(Properties.Resources.collision);

            #region update ball position

            // TODO create code to move ball either left or right based on ballMoveRight and BALL_SPEED

            if (ballMoveRight == true)
            {
                ballX = ballX + BALL_SPEED;
            }
            else if (ballMoveRight == false)
            {
                ballX = ballX - BALL_SPEED;
            }

            // TODO create code move ball either down or up based on ballMoveDown and BALL_SPEED
            if (ballMoveDown == false)
            {
                ballY = ballY + BALL_SPEED;
            }
            else if (ballMoveDown == true)
            {
                ballY = ballY - BALL_SPEED;
            }
            #endregion

            #region update paddle positions
            // TODO create code to move player 1 paddle up and down using paddle1Y and PADDLE_SPEED
            if (aKeyDown == true && paddle1Y > 0)
            {
                paddle1Y = paddle1Y - PADDLE_SPEED1;
            }
            if (zKeyDown == true && paddle1Y < this.Height - 40)
            {
                paddle1Y = paddle1Y + PADDLE_SPEED1;
            }

            // TODO create an if statement and code to move player 2 paddle up using paddle2Y and PADDLE_SPEED

            if (jKeyDown == true && paddle2Y > 0)
            {
                paddle2Y = paddle2Y - PADDLE_SPEED2;
            }
            if (mKeyDown == true && paddle2Y < this.Height - 40)
            {
                paddle2Y = paddle2Y + PADDLE_SPEED2;
            }
            #endregion

            #region ball collision with top and bottom lines
            // TODO use ballMoveDown boolean to change direction
            // TODO play a collision sound
            if (ballY <= 5)
            {
                //If the ball hits the top of the screen, the  method ball moves downward
                player.Play();
                ballMoveDown = false;
            }
            if (ballY >= this.Height - 10)
            {
                //If the ball hits the bottom of the screen, the  method ball moves upward
                player.Play();
                ballMoveDown = true;
            }

            // TODO In an else if statement use ballY, this.Height, and BALL_SIZE to check for collision with bottom line
            // If true use ballMoveDown down boolean to change direction

            #endregion

            #region ball collision with paddles

            if (ballY > paddle1Y && ballY < paddle1Y + PADDLE_LENGTH1 && ballX < PADDLE_EDGE + PADDLE_WIDTH) // left paddle collision
            {
                // TODO play a "paddle hit" sound 
                // TODO use ballMoveRight boolean to change direction
                player.Play();
                ballMoveRight = true;
            }
            else if (ballY > paddle2Y && ballY < paddle2Y + PADDLE_LENGTH2 && ballX + BALL_SIZE > this.Width - PADDLE_EDGE - PADDLE_WIDTH / 2) // right paddle collision
            {
                // TODO play a "paddle hit" sound 
                // TODO use ballMoveRight boolean to change direction
                player.Play();
                ballMoveRight = false;
            }

            #endregion

            #region ball collision with side walls (point scored)

            player = new SoundPlayer(Properties.Resources.score);

            //If ball hits left wall
            if (ballX < 0) 
            {
                player.Play();
                player2Score++;
                player2Label.Text = "Player 2: " + Convert.ToString(player2Score);
                if (player2Score < 4 && PADDLE_LENGTH1 > 20)
                {
                    PADDLE_LENGTH1 = PADDLE_LENGTH1 - 5;
                }
                else if (player2Score >4 && PADDLE_LENGTH1 >20)
                {
                    PADDLE_LENGTH1 = PADDLE_LENGTH1 - 10;
                    PADDLE_SPEED1--;
                }
                Refresh();
                if (player2Score == gameWinScore)
                {
                    GameOver("Player 2");
                }
                else
                {
                    SetParameters();
                }
            }

            // if ball hits right wall logic
            if (ballX >= this.Width)  
            {
                player.Play();
                player1Score++;
                player1Label.Text = "Player 1: " + Convert.ToString(player1Score);
                if (player1Score < 4 && PADDLE_LENGTH2 > 20)
                {
                    PADDLE_LENGTH2 = PADDLE_LENGTH2 - 3;
                }
                else if (player1Score > 4 && PADDLE_LENGTH2 > 20)
                {
                    PADDLE_LENGTH2 = PADDLE_LENGTH2 - 10;
                    PADDLE_SPEED2--;
                }
                Refresh();
                if (player1Score == gameWinScore)
                {
                    GameOver("Player 1");
                }
                else
                {
                    SetParameters();
                }
                // --- use if statement to check to see if player 1 has won the game. If true run 
                // gameWinScore method. Else call SetParameters method to reset ball position.
            }

            // TODO same as above but this time check for collision with the right wall

            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;
            gameUpdateLoop.Enabled = false;
            startLabel.Text = "The winner is " + winner;
            startLabel.Visible = true;
            Refresh();
            Thread.Sleep(2000);
            startLabel.Text = "Would you like to play again? (Y/N)";
            PADDLE_SPEED1 = 10;
            PADDLE_SPEED2 = 10;
            Refresh();

        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (gameUpdateLoop.Enabled == true)
            {
                if (start == true)
                {
                    ballX = this.Width / 2;
                    ballY = this.Height / 2;
                    start = false;
                }
                //Draws the paddles using FillRectangle
                e.Graphics.FillRectangle(drawBrush, PADDLE_EDGE - PADDLE_WIDTH, paddle1Y, PADDLE_WIDTH, PADDLE_LENGTH1);
                e.Graphics.FillRectangle(drawBrush, this.Width - PADDLE_EDGE, paddle2Y, PADDLE_WIDTH, PADDLE_LENGTH2);

                //Draws the ball using FillRectangle
                e.Graphics.FillRectangle(drawBrush, ballX, ballY, BALL_SIZE, BALL_SIZE);
            }
            
        }
    }
}
