using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InfiniBlocker
{
    class Intro
    {
        #region Variables
        private bool fadeIn, fadeOut, animating;
        private Texture2D[] m_introArt;
        private bool init;

        //Used in Animation
        private double m_animDelay;
        private int m_currCell;
        private int m_cellOffset;

        //Use for Fade In/Out Effects
        private int m_AlphaValue;
        private int m_FadeIncrement;
        private double m_FadeDelay;
        #endregion

        #region Methods
        public Intro(Texture2D[] txr)
        {
            m_AlphaValue = 1;
            m_FadeIncrement = 10;
            m_FadeDelay = .04;
            fadeIn = true;
            fadeOut = false;
            animating = false;
            init = true;

            m_animDelay = 0.2f;
            m_currCell = 0;
            m_cellOffset = 225;

            m_introArt = txr;

            CheckFile();
        }

        public void CheckFile()
        {
            if (File.Exists("scores.dat"))
            {

            }
            else
            {
                File.Create("scores.dat");
                FileStream scoreFile = new FileStream("scores.dat", FileMode.Open, FileAccess.Write);
                BinaryWriter newWriter = new BinaryWriter(scoreFile);
                newWriter.Write(0);
                newWriter.Flush();
                newWriter.Close();
            }
        }

        public void UpdateMe(GameTime gt, ref GameState gameState, KeyboardState currKb, KeyboardState oldKb)
        {
            if (currKb.IsKeyDown(Keys.Enter) && oldKb.IsKeyUp(Keys.Enter))
            {
                gameState = GameState.Menu;
            }

            if (init)
            {
                Sound.StartTrack(5);
                init = false;
            }

            if (fadeIn)
            {
                //Decrement the delay
                m_FadeDelay -= gt.ElapsedGameTime.TotalSeconds;

                //If the Fade delays has dropped below zero, fade in/out a little more.
                if (m_FadeDelay <= 0)
                {
                    //Reset the Fade delay
                    m_FadeDelay = .04;

                    //Increment/Decrement the fade value for the image
                    m_AlphaValue += m_FadeIncrement;

                    //Stop fade in when fully realised
                    if (m_AlphaValue >= 255)
                    {
                        fadeIn = false;
                        animating = true;
                        //Switch to lowering for fadeOut
                        m_FadeIncrement *= -1;
                        m_FadeDelay = .03;
                    }
                }
            }

            if (animating)
            {
                if (m_currCell < 10)
                {
                    m_animDelay -= gt.ElapsedGameTime.TotalSeconds;

                    if (m_animDelay <= 0)
                    {
                        m_animDelay = 0.2f;
                        m_currCell++;
                    }
                }
                else
                {
                    animating = false;
                    fadeOut = true;
                }
            }

            if (fadeOut)
            {
                //Decrement the delay
                m_FadeDelay -= gt.ElapsedGameTime.TotalSeconds;

                //If the Fade delays has dropped below zero, fade in/out a little more.
                if (m_FadeDelay <= 0)
                {
                    //Reset the Fade delay
                    m_FadeDelay = .03;

                    //Increment/Decrement the fade value for the image
                    m_AlphaValue += m_FadeIncrement;

                    //Stop fade in when fully realised
                    if (m_AlphaValue <= 0)
                    {
                        Sound.StopTrack();
                        gameState = GameState.Menu;
                    }
                }
            }
        }

        public void DrawMe(SpriteBatch sb)
        {
            sb.Draw(m_introArt[0], new Rectangle(0, 0, m_introArt[0].Width, m_introArt[0].Height),
                        new Color(m_AlphaValue, m_AlphaValue, m_AlphaValue));

            sb.Draw(m_introArt[1], new Rectangle(380, 250, 550, 225),
                        new Color(m_AlphaValue, m_AlphaValue, m_AlphaValue));
            if (fadeIn)
            {
                sb.Draw(m_introArt[2], new Rectangle(380, 50 + (int)(m_AlphaValue * 2.5f), 550, 450),
                            new Color(m_AlphaValue, m_AlphaValue, m_AlphaValue));
            }
        }
        #endregion
    }

    class Menu
    {
        #region Variables
        private MenuScreen menuScreen;

        //0-3 backgrounds
        //4-7 Main Buttons
        //8-10 NewGame Buttons
        //11-13 Settings Buttons
        //14 Empty Button (for HighScore)
        //15 Back Button (in 3 submenus)
        private Texture2D[] m_backArt;
        private Texture2D[] m_buttonArt;
        private SpriteFont m_font;
        private int highScore;
        private bool init;

        private MenuButtons menuButtons;
        #endregion

        #region Methods
        public Menu(Texture2D[] txr, SpriteFont menuFont)
        {
            menuScreen = MenuScreen.Main;
            m_font = menuFont;

            m_backArt = new Texture2D[4];
            m_buttonArt = new Texture2D[12];
            init = true;
            
            //Pass background textures into array
            for (int i = 0; i < 4; i++)
            {

                m_backArt[i] = txr[i];
            }

            //Pass button textures into seperate array
            for (int i = 4; i < 16; i++)
            {
                m_buttonArt[i - 4] = txr[i];
            }

            menuButtons = new MenuButtons(m_buttonArt);
        }

        public void UpdateMe(KeyboardState currKb, KeyboardState oldKb, MouseState currMs, MouseState oldMS, ref GameState gameState, ref InPlay gameInPlay)
        {
            if (init)
            {
                Sound.StartTrack(7);
                init = false;
            }

                    menuScreen = menuButtons.UpdateMe(currKb, oldKb, currMs, oldMS, menuScreen, ref gameState, ref gameInPlay);

                    if (menuScreen == MenuScreen.HighScore)
                    {
                        highScore = GetHighScore();
                    }
        }

        public void DrawMe(SpriteBatch sb)
        {

            sb.Draw(m_backArt[(int)menuScreen], new Rectangle(0, 0, m_backArt[(int)menuScreen].Width, m_backArt[(int)menuScreen].Height), Color.White);

            menuButtons.DrawMe(sb);


            switch (menuScreen)
            {
                case MenuScreen.Main:

                    break;

                case MenuScreen.NewGame:

                    break;

                case MenuScreen.HighScore:

                    //highScore = GetHighScore();

                    sb.DrawString(m_font, "High Score: " + highScore, new Vector2(265, 193), Color.Gold);

                    break;

                case MenuScreen.Settings:

                    break;
            }

        }

        public int GetHighScore()
        {
            int number;
            FileStream scoreFile = new FileStream("scores.dat", FileMode.Open, FileAccess.Read);
            BinaryReader scoreReader = new BinaryReader(scoreFile);
            number = scoreReader.ReadInt32();
            scoreReader.Close();
            return number;
        }
        #endregion
    }

    class InPlay
    {
        #region Variables
        private InPlayState inPlayState;
        private GameStrength gameStrength;

        private int playerLives;
        private int playerScore;
        private int highScore;
        private int[] playerPowers;
        private float[] ballTimer;
        private float[] blockTimer;
        private int gameStage;

        private MainGenerator blockGenerator;

        private Texture2D batTxr;
        private Texture2D ballTxr;
        private Texture2D blockTxr;
        private Texture2D powerupTxr;

        private List<Blocks> blocks;
        private List<PowerUp> powerUp;
        private List<Ball> playerBall;
        private Bat playerBat;

        private Texture2D backgroundTxr;
        //**If implementing enemies add animated door
        private Texture2D borderTxr;
        private Texture2D fieldTxr;
        private Texture2D infoscreenTxr;
        private Texture2D pauseTxr;
        private Texture2D backTile;

        private SpriteFont scoreFont;
        private SpriteFont pauseFont;
        private bool init;
        private bool init2;

        //Animation Variables
        private float gameOverCycle;

        private Rectangle screenBounds;

        public GameStrength Strength
        {
            get
            {
                return gameStrength;
            }
            set
            {
                gameStrength = value;
            }
        }
        #endregion

        #region Methods
        public InPlay(Texture[] txr, SpriteFont scrFont, SpriteFont pseFont)
        {
            //Temp Values for debug

            highScore = GetHighScore();
            gameStage = 1;
            init2 = true;

            inPlayState = InPlayState.NewLevel;
            blockGenerator = new MainGenerator();
            blockTimer = new float[180];
            Array.Clear(blockTimer, 0, blockTimer.Length);

            blocks = new List<Blocks>();
            playerBall = new List<Ball>();
            powerUp = new List<PowerUp>();
            ballTimer = new float[5];
            Array.Clear(ballTimer, 0, ballTimer.Length);

            scoreFont = scrFont;
            pauseFont = pseFont;
            init = true;

            batTxr = (Texture2D)txr[0];
            ballTxr = (Texture2D)txr[1];
            blockTxr = (Texture2D)txr[2];
            powerupTxr = (Texture2D)txr[3];

            backgroundTxr = (Texture2D)txr[4];
            borderTxr = (Texture2D)txr[5];
            fieldTxr = (Texture2D)txr[6];
            infoscreenTxr = (Texture2D)txr[7];
            pauseTxr = (Texture2D)txr[8];
            backTile = (Texture2D)txr[9];

            playerLives = 3;
            playerScore = 0;
            screenBounds = new Rectangle(170, 34, 825, 700);

            gameOverCycle = 2f;

            playerPowers = new int[3] { 0, 0, 0 };
        }

        public void Reset()
        {
            inPlayState = InPlayState.NewLevel;
            blockGenerator = new MainGenerator();
            blockTimer = new float[180];
            Array.Clear(blockTimer, 0, blockTimer.Length);
            init2 = true;

            blocks = new List<Blocks>();
            playerBall = new List<Ball>();
            powerUp = new List<PowerUp>();
            ballTimer = new float[5];
            Array.Clear(ballTimer, 0, ballTimer.Length);

            playerLives = 3;
            playerScore = 0;
        }

        public int GetHighScore()
        {
            int number;
        start:
            FileStream scoreFile = new FileStream("scores.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //scoreFile = File.OpenRead("scores.dat");
            BinaryReader scoreReader = new BinaryReader(scoreFile);
            try
            {
                number = scoreReader.ReadInt32();
            }
            catch (EndOfStreamException)
            {
                BinaryWriter newWriter = new BinaryWriter(scoreFile);
                newWriter.Write(0);
                newWriter.Flush();
                newWriter.Close();
                goto start;
            }
            scoreFile.Close();
            scoreReader.Close();
            return number;
        }

        public void WriteHighScore(int playerscore)
        {
            FileStream scoreFile = new FileStream("scores.dat", FileMode.Create, FileAccess.Write);
            BinaryWriter scoreWriter = new BinaryWriter(scoreFile);
            scoreWriter.Write(playerscore);
            scoreWriter.Flush();
            scoreWriter.Close();
        }

        public void UpdateMe(GameTime gt, ref GameState gameState, KeyboardState currKey, KeyboardState oldKey)
        {
            switch (inPlayState)
            {
                #region NewLevel
                case InPlayState.NewLevel:

                    /*Erase remaining solid blocks
                     * initialise new block list
                     * initialise bat & ball
                     * Put all into a seperate method: InitLevel
                    */
                    int[,] tempBlocks = new int[15, 12];
                    int[,] tempColors = new int[15, 12];

                    Sound.PlayEffect(4);

                reset:

                    tempBlocks = blockGenerator.ResetBlocks();
                    tempColors = blockGenerator.ResetColors();

                    blocks.Clear();
                    playerBall.Clear();

                    for (int i = 0; i < 15; i++)
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (tempBlocks[i, j] == 1)
                            {
                                blocks.Add(new Blocks(blockTxr, i, j, tempColors[i, j]));
                            }
                        }
                    }

                    if (blocks.Count < 70 || blocks.Count > 130)
                        goto reset;

                    playerBat = new Bat(batTxr, screenBounds);
                    playerBall.Add(new Ball(ballTxr, screenBounds));

                    inPlayState = InPlayState.Running;

                    break;
                #endregion

                #region Running
                case InPlayState.Running:

                    if (init2)
                    {
                        Sound.StartTrack(0);
                        init2 = false;
                    }

                    if (playerScore > highScore)
                    {
                        highScore = playerScore;
                        WriteHighScore(highScore);
                    }

                    //**For Debug***
                    if (currKey.IsKeyDown(Keys.I) && oldKey.IsKeyUp(Keys.I))
                    {
                        Sound.StopTrack();
                        init2 = true;
                        inPlayState = InPlayState.NewLevel;
                        break;
                    }

                    //Hitting Escape pauses game
                    if (currKey.IsKeyDown(Keys.Escape) && oldKey.IsKeyUp(Keys.Escape))
                    {
                        inPlayState = InPlayState.Paused;
                        break;
                    }
                    /* Trying to let balls collisde with each other.
                    for (int i = 0; i < playerBall.Count(); i++)
                    {
                        for (int j = 0; j < playerBall.Count(); j++)
                        {
                            if (i != j)
                            {
                                float temp = playerBall[i].Pos.Length() - playerBall[j].Pos.Length();
                                if (Math.Sqrt(temp * temp) < 12) 
                                {
                                    float tempX = playerBall[i].VecX;
                                    float tempY = playerBall[i].VecY;

                                    playerBall[i].VecX = playerBall[j].VecX;
                                    playerBall[i].VecY = playerBall[j].VecY;

                                    playerBall[j].VecX = tempX;
                                    playerBall[j].VecY = tempY;
                                }
                            }
                        }
                    }
                    */
                    //Update PowerUps
                    for (int i = 0; i < powerUp.Count; i++)
                    {
                        powerUp[i].UpdateMe();
                    }

                    //Update Player Bat
                    playerBat.UpdateMe(currKey);


                    //Check for collisions btw/ powerup & bat

                    for (int i = 0; i < powerUp.Count(); i++)
                    {
                        if (powerUp[i].Rect.Intersects(playerBat.Rect))
                        {
                            powerUp[i].Anim = AnimState.Death;
                            playerPowers[(int)powerUp[i].Power] = 1;
                            powerUp.RemoveAt(i);
                        }
                    }

                    //Give player any gained powers
                    for (int i = 0; i < 3; i++)
                    {
                        if (playerPowers[i] == 1)
                        {
                            switch (i)
                            {
                                case 0:
                                    playerLives++;
                                    break;
                                case 1:
                                    playerBall.Add(new Ball(ballTxr, screenBounds));
                                    break;
                                case 2:
                                    playerLives++;
                                    break;
                            }
                            playerPowers[i] = 0;
                        }

                    }


                    int liveBlocks;
                    liveBlocks = 0;
                    //Update blocks
                    for (int i = 0; i < blocks.Count(); i++)
                    {
                        blocks[i].UpdateMe(gt);
                        if (blocks[i].Anim == AnimState.Normal || blocks[i].Anim == AnimState.Hit)
                        {
                            liveBlocks++;
                        }
                    }
                    if (liveBlocks == 0)
                    {
                        inPlayState = InPlayState.NewLevel;
                    }


                    //Cycle through balls
                    for (int i = 0; i < playerBall.Count; i++)
                    {
                        #region Ball on Bat
                        if (playerBall[i].State == BallState.Stuck)
                        {
                            playerBall[i].Pos = new Vector2(playerBat.Rect.X + 38, playerBat.Rect.Y - 25);
                            /*
                            playerBall[i].Rect.X = (int)playerBall[i].Pos.X;
                            playerBall[i].Rect.Y = (int)playerBall[i].Pos.Y;
                            /*
                            playerBall[i].Rect.X = (int)playerBall[i].Pos.X;
                            playerBall[i].Rect.Y = (int)playerBall[i].Pos.Y + 7;

                            playerBall[i].Rect.X = (int)playerBall[i].Pos.X + 3;
                            playerBall[i].Rect.Y = (int)playerBall[i].Pos.Y + 3;
                            */
                            //playerBall[i].Rect = new Rectangle(playerBat.Rect.X + 38, playerBat.Rect.Y - 25, 25, 25);

                            if (currKey.IsKeyDown(Keys.Space) && oldKey.IsKeyUp(Keys.Space))
                            {
                                Sound.PlayEffect(6);
                                playerBall[i].VecY = -4;
                                playerBall[i].VecX = playerBat.VelR - playerBat.VelL;
                                playerBall[i].State = BallState.Normal;
                            }
                        }
                        #endregion

                        #region Ball/Wall Collision
                        //Check for wall collision
                        if ((playerBall[i].Rect[1].X + playerBall[i].Rect[1].Width) >= (screenBounds.X + screenBounds.Width))
                        {
                            if (playerBall[i].VecX > 0)
                            {
                                playerBall[i].VecX *= -1;
                            }
                        }

                        if (playerBall[i].Rect[1].X <= screenBounds.X)
                        {
                            if (playerBall[i].VecX < 0)
                            {
                                playerBall[i].VecX *= -1;
                            }
                        }

                        if (playerBall[i].Rect[0].Y <= screenBounds.Y)
                        {
                            if (playerBall[i].VecY < 0)
                            {
                                playerBall[i].VecY *= -1;
                            }
                        }
                        #endregion

                        #region Ball/Bat Collision
                        //Check for collision of bat & ball
                        if (playerBat.Rect.X < (playerBall[i].Rect[3].X + playerBall[i].Rect[3].Width)
                            && playerBat.Rect.X > playerBall[i].Rect[3].X
                            && (playerBall[i].Rect[3].Y + 13) > playerBat.Rect.Y
                            && (playerBall[i].Rect[3].Y + 13) < (playerBat.Rect.Y + playerBat.Rect.Height))
                        {
                            if (playerBall[i].VecX > 0)
                            {
                                playerBall[i].VecY *= -1;
                                playerBall[i].VecX += (playerBat.VelR - playerBat.VelL);
                                Sound.PlayEffect(7);
                            }
                            goto EndBatCol;
                        }
                        if ((playerBat.Rect.X + playerBat.Rect.Width) < playerBall[i].Rect[3].X
                            && (playerBat.Rect.X + playerBat.Rect.Width) > playerBall[i].Rect[3].X
                            && (playerBall[i].Rect[3].Y + 13) > playerBat.Rect.Y
                            && (playerBall[i].Rect[3].Y + 13) < (playerBat.Rect.Y + playerBat.Rect.Height))
                        {
                            if (playerBall[i].VecX < 0)
                            {
                                playerBall[i].VecY *= -1;
                                playerBall[i].VecX += (playerBat.VelR - playerBat.VelL);
                                Sound.PlayEffect(7);
                            }
                            goto EndBatCol;
                        }
                        if (playerBat.Rect.Y < (playerBall[i].Rect[3].Y + playerBall[i].Rect[3].Height)
                            && playerBat.Rect.Y > playerBall[i].Rect[3].Y
                            && (playerBall[i].Rect[3].X + 13) > playerBat.Rect.X
                            && (playerBall[i].Rect[3].X + 13) < (playerBat.Rect.X + playerBat.Rect.Width))
                        {
                            if (playerBall[i].VecY > 0)
                            {
                                playerBall[i].VecY *= -1;
                                playerBall[i].VecX += (playerBat.VelR - playerBat.VelL);
                                Sound.PlayEffect(7);
                            }
                            goto EndBatCol;
                        }/*
                        if ((playerBat.Rect.Y + playerBat.Rect.Height) > playerBall[i].Rect[3].Y
                            && (playerBat.Rect.Y + playerBat.Rect.Height) < (playerBall[i].Rect[3].Y + playerBall[i].Rect[3].Height)
                            && (playerBall[i].Rect[3].X + 13) > playerBat.Rect.X
                            && (playerBall[i].Rect[3].X + 13) < (playerBat.Rect.X + playerBat.Rect.Width))
                        {
                            if (playerBall[i].VecY < 0)
                            {
                                playerBall[i].VecY *= -1;
                                playerBall[i].VecY += (playerBat.VelR - playerBat.VelL);
                                Sound.PlayEffect(7);
                            }
                            goto EndBatCol;
                        }*/
                    EndBatCol:
                        #endregion

                        //Setup a hit timer**
                        /*
                        if (playerBall[i].Rect[1].Intersects(playerBat.Rect)
                                || playerBall[i].Rect[0].Intersects(playerBat.Rect)
                                || playerBall[i].Rect[2].Intersects(playerBat.Rect))
                        {
                            if ((playerBall[i].Pos.X + 21) < playerBat.Rect.X
                                || (playerBall[i].Pos.X + 3) > (playerBat.Rect.X + playerBat.Rect.Width))
                            {
                                playerBall[i].VecX *= -1;
                            }
                            else
                            {
                                playerBall[i].VecY *= -1;
                            }
                        }
                        */

                        /*
                            if (playerBall[i].Rect[0].Intersects(playerBat.Rect))
                            {
                                if (playerBall[i].Rect[j].Bottom >= playerBat.Rect.Top)
                                {
                                    playerBall[i].VecX += playerBat.VelR / 3;
                                    playerBall[i].VecX -= playerBat.VelL / 3;
                                    playerBall[i].VecY *= -1;
                                }
                                if (playerBall[i].Rect[j].Left <= playerBat.Rect.Right)
                                {
                                    playerBall[i].VecX *= -1;
                                }
                                if (playerBall[i].Rect[j].Right >= playerBat.Rect.Left)
                                {
                                    playerBall[i].VecX *= -1;
                                }
                            }
                        */
                        playerBall[i].UpdateMe();
                    }

                    /*
                    for (int j = 0; j < playerBall.Count(); j++)
                    {
                        CheckTop(j);
                        CheckSide(j);
                    }
                    */

                    #region Ball/Block Collision




                    /*
                                for (int k = 0; k < 3; k++)
                                {
                                    if (playerBall[j].Rect[k].Intersects(blocks[i].Rect))
                                    {
                                        
                                        if (playerBall[j].Rect[k].Left <= blocks[i].Rect.Right || playerBall[j].Rect[k].Right >= blocks[i].Rect.Left)
                                        {
                                            playerBall[j].VecX *= -1;
                                            Sound.PlayEffect(2);
                                        }
                                        if (playerBall[j].Rect[k].Top <= blocks[i].Rect.Bottom || playerBall[j].Rect[k].Bottom >= blocks[i].Rect.Top)
                                        {
                                            playerBall[j].VecY *= -1;
                                            Sound.PlayEffect(2);
                                        }
                                        
                                        if (blockTimer[j] <= 0)
                                        {
                                            if (playerBall[j].Rect[0].Intersects(blocks[i].Rect))
                                            {
                                                playerBall[j].VecY *= -1;
                                                blockTimer[j] = 0.2f;
                                                goto doneIntersecting2;
                                            }

                                            if (playerBall[j].Rect[1].Intersects(blocks[i].Rect))
                                            {
                                                playerBall[j].VecX *= -1;
                                                blockTimer[j] = 0.2f;
                                                goto doneIntersecting2;
                                            }
                                            
                                            if (playerBall[j].Rect[2].Intersects(blocks[i].Rect))
                                            {
                                                playerBall[j].VecX *= -1;
                                                playerBall[j].VecY *= -1;
                                                blockTimer[j] = 0.2f;
                                            }
                                        }
                                        else
                                        {
                                            blockTimer[j] -= (float)gt.ElapsedGameTime.TotalSeconds;
                                        }

                                    doneIntersecting2:
                                        
                                        //If not a solid block decrement health
                                        if (blocks[i].Type == BlockType.Normal && blocks[i].Anim == AnimState.Normal)
                                        {
                                            blocks[i].Anim = AnimState.Hit;
                                            blocks[i].Timer = 0.1f;
                                            blocks[i].Health--;
                                            if(blocks[i].Health <= 0)
                                            {
                                                blocks[i].Anim = AnimState.Death;
                                                playerScore += 10;
                                                double powerRand = Game1.RNG.NextDouble();
                                                if(powerRand > 0.7)
                                                {
                                                    powerUp.Add(new PowerUp(powerupTxr, blocks[i].Rect.X, blocks[i].Rect.Y));
                                                }
                                            }
                                        }
                                        k = 3;
                                    }
                                }
                            }
                        }
                    }*/
                    #endregion


                    #region Ball/Block Collision More Bad Code
                    //Check for collision of ball & blocks
                    
                    for (int i = 0; i < blocks.Count(); i++)
                    {
                        if(blocks[i].Anim != AnimState.Death)
                        {
                            for (int j = 0; j < playerBall.Count(); j++)
                            {
                                for (int k = 0; k < 3; k++)
                                {
                                    if (playerBall[j].Rect[k].Intersects(blocks[i].Rect))
                                    {
                                        
                                        if (playerBall[j].Rect[k].Left <= blocks[i].Rect.Right || playerBall[j].Rect[k].Right >= blocks[i].Rect.Left)
                                        {
                                            playerBall[j].VecX *= -1;
                                            Sound.PlayEffect(2);
                                        }
                                        if (playerBall[j].Rect[k].Top <= blocks[i].Rect.Bottom || playerBall[j].Rect[k].Bottom >= blocks[i].Rect.Top)
                                        {
                                            playerBall[j].VecY *= -1;
                                            Sound.PlayEffect(2);
                                        }
                                        
                                        if (blockTimer[j] <= 0)
                                        {
                                            if (playerBall[j].Rect[0].Intersects(blocks[i].Rect))
                                            {
                                                playerBall[j].VecY *= -1;
                                                blockTimer[j] = 0.2f;
                                                goto doneIntersecting2;
                                            }

                                            if (playerBall[j].Rect[1].Intersects(blocks[i].Rect))
                                            {
                                                playerBall[j].VecX *= -1;
                                                blockTimer[j] = 0.2f;
                                                goto doneIntersecting2;
                                            }
                                            
                                            if (playerBall[j].Rect[2].Intersects(blocks[i].Rect))
                                            {
                                                playerBall[j].VecX *= -1;
                                                playerBall[j].VecY *= -1;
                                                blockTimer[j] = 0.2f;
                                            }
                                        }
                                        else
                                        {
                                            blockTimer[j] -= (float)gt.ElapsedGameTime.TotalSeconds;
                                        }

                                    doneIntersecting2:
                                        
                                        //If not a solid block decrement health
                                        if (blocks[i].Type == BlockType.Normal && blocks[i].Anim == AnimState.Normal)
                                        {
                                            blocks[i].Anim = AnimState.Hit;
                                            blocks[i].Timer = 0.1f;
                                            blocks[i].Health--;
                                            if(blocks[i].Health <= 0)
                                            {
                                                blocks[i].Anim = AnimState.Death;
                                                playerScore += 10;
                                                double powerRand = Game1.RNG.NextDouble();
                                                if(powerRand > 0.7)
                                                {
                                                    powerUp.Add(new PowerUp(powerupTxr, blocks[i].Rect.X, blocks[i].Rect.Y));
                                                }
                                            }
                                        }
                                        k = 3;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Ball/Block Collision: Bad Code
                    /*
                    for (int i = 0; i < blocks.Count(); i++)
                    {
                        if (blocks[i].Anim != AnimState.Death)
                        {
                            for (int j = 0; j < playerBall.Count(); j++)
                            {
                                if (blocks[i].Rect.Y < (playerBall[j].Rect[3].Y + playerBall[j].Rect[3].Height)
                                    && blocks[i].Rect.Y > playerBall[j].Rect[3].Y
                                    && (playerBall[j].Rect[3].X + 13) > blocks[i].Rect.X
                                    && (playerBall[j].Rect[3].X + 13) < (blocks[i].Rect.X + blocks[i].Rect.Width))
                                {
                                    //if (playerBall[j].VecY > 0)
                                    //{
                                        playerBall[j].VecY *= -1;
                                        HitBlock(i);
                                    //}
                                }
                                if ((blocks[i].Rect.Y + blocks[i].Rect.Height) > playerBall[j].Rect[3].Y
                                    && (blocks[i].Rect.Y + blocks[i].Rect.Height) < (playerBall[j].Rect[3].Y + playerBall[j].Rect[3].Height)
                                    && (playerBall[j].Rect[3].X + 13) > blocks[i].Rect.X
                                    && (playerBall[j].Rect[3].X + 13) < (blocks[i].Rect.X + blocks[i].Rect.Width))
                                {
                                    //if (playerBall[j].VecY < 0)
                                    //{
                                        playerBall[j].VecY *= -1;
                                        HitBlock(i);
                                    //}
                                }
                                if (blocks[i].Rect.X < (playerBall[j].Rect[3].X + playerBall[j].Rect[3].Width)
                                    && blocks[i].Rect.X > playerBall[j].Rect[3].X
                                    && (playerBall[j].Rect[3].Y + 13) > blocks[i].Rect.Y
                                    && (playerBall[j].Rect[3].Y + 13) < (blocks[i].Rect.Y + blocks[i].Rect.Height))
                                {
                                    //if (playerBall[j].VecX > 0)
                                    //{
                                        playerBall[j].VecY *= -1;
                                        HitBlock(i);
                                    //}
                                }
                                if ((blocks[i].Rect.X + blocks[i].Rect.Width) < playerBall[j].Rect[3].X
                                    && (blocks[i].Rect.X + blocks[i].Rect.Width) > playerBall[j].Rect[3].X
                                    && (playerBall[j].Rect[3].Y + 13) > blocks[i].Rect.Y
                                    && (playerBall[j].Rect[3].Y + 13) < (blocks[i].Rect.Y + blocks[i].Rect.Height))
                                {
                                    //if (playerBall[j].VecX < 0)
                                    //{
                                        playerBall[j].VecY *= -1;
                                        HitBlock(i);
                                    //}
                                }
                            }
                        }
                    }
                    */
                    #endregion

                    #region Ball out of Bounds
                    for (int i = 0; i < playerBall.Count; i++)
                    {
                        if (playerBall[i].Pos.Y + 20 > 734)
                        {
                            playerBall.RemoveAt(i);
                            goto outoftheloop;
                        }
                    }
                outoftheloop:
                    #endregion

                    //If no mas balls, initiado a nuevo vida
                    if (playerBall.Count() == 0)
                    {
                        playerLives--;
                        inPlayState = InPlayState.NewLife;
                    }

                    init = false;

                    if (currKey.IsKeyDown(Keys.O) && oldKey.IsKeyUp(Keys.O))
                    {
                        playerLives--;
                        break;
                    }

                    if (playerLives == 0)
                    {
                        Sound.PlayEffect(1);
                        inPlayState = InPlayState.GameOver;
                    }

                    break;
                #endregion

                #region NewLife
                case InPlayState.NewLife:

                    /*
                     * Reposition bat and freeze for a moment
                     * reposition ball on bat
                     * cycle through a flashing animation
                     * when over send to rRunning playState
                    */



                    playerBall.Add(new Ball(ballTxr, screenBounds));
                    playerBall[0].Reset();

                    playerBat.Reset();

                    inPlayState = InPlayState.Running;

                    break;
                #endregion

                #region Paused
                case InPlayState.Paused:

                    //Go to Main Menu
                    if (currKey.IsKeyDown(Keys.Escape) && oldKey.IsKeyUp(Keys.Escape))
                    {
                        Sound.StopTrack();
                        Sound.StartTrack(7);
                        gameState = GameState.Menu;
                        Reset();
                        break;
                    }

                    //Resume Play
                    if (currKey.IsKeyDown(Keys.Enter) && oldKey.IsKeyUp(Keys.Enter))
                    {
                        inPlayState = InPlayState.Running;
                        break;
                    }

                    break;
                #endregion

                #region GameOver
                case InPlayState.GameOver:

                    if (gameOverCycle > 0)
                    {
                        gameOverCycle -= (float)gt.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        Sound.StopTrack();
                        Sound.StartTrack(7);
                        Reset();
                        gameState = GameState.Menu;
                    }

                    break;
                #endregion
            }
        }
        /*
        public void CheckTop(int i)
        {
            for (int j = 0; j < blocks.Count(); j++)
                    {
                        if(blocks[j].Anim != AnimState.Death)
                        {
                            if (playerBall[i].Rect[0].Top <= blocks[j].Rect.Bottom)
                                        {
                                            if(playerBall[i].VecY < 0)
                                            {
                                            playerBall[j].VecY *= -1;
                                            Sound.PlayEffect(2);
                                                goto TopEnd;
                                            }
                                        }
                            if (playerBall[i].Rect[0].Bottom >= blocks[j].Rect.Top)
                                        {
                                            if(playerBall[i].VecY > 0)
                                            {
                                            playerBall[j].VecY *= -1;
                                            Sound.PlayEffect(2);
                                            goto TopEnd;
                                            }
                                        }
                        }
            }
        TopEnd:
            return;
        }

        public void CheckSide(int i)
        {
            for (int j = 0; j < blocks.Count(); j++)
                    {
                        if(blocks[j].Anim != AnimState.Death)
                        {
                            if (playerBall[j].Rect[1].Left <= blocks[j].Rect.Right)
                                        {
                                            if(playerBall[i].VecX < 0)
                                            {
                                            playerBall[j].VecY *= -1;
                                            Sound.PlayEffect(2);
                                                goto SideEnd;
                                            }
                                        }
                            if (playerBall[j].Rect[1].Right >= blocks[j].Rect.Left)
                                        {
                                            if(playerBall[i].VecX > 0)
                                            {
                                            playerBall[j].VecY *= -1;
                                            Sound.PlayEffect(2);
                                            goto SideEnd;
                                            }
                                        }
                        }
            }
        SideEnd:
            return;
        }
        */
        public void DrawMe(SpriteBatch sb, GameTime gt)
        {
            sb.Draw(backgroundTxr, new Rectangle(0, 0, backgroundTxr.Width, backgroundTxr.Height), Color.White);

            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 22; j++)
                {
                    sb.Draw(backTile, new Rectangle(170 + (i * 32), 34 + (j * 32), 32, 32), Color.White);
                }
            }

            sb.Draw(borderTxr, new Rectangle(150, 14, borderTxr.Width, borderTxr.Height), Color.White);
            //sb.Draw(fieldTxr, new Rectangle(170, 34, fieldTxr.Width, fieldTxr.Height), Color.White);
            sb.Draw(infoscreenTxr, new Rectangle(1015, 14, infoscreenTxr.Width, infoscreenTxr.Height), Color.White);

            sb.DrawString(scoreFont, "High Score", new Vector2(1035, 60), Color.Navy);
            sb.DrawString(scoreFont, highScore.ToString(), new Vector2(1085, 84), Color.Black);
            sb.DrawString(scoreFont, "Score", new Vector2(1035, 200), Color.Navy);
            sb.DrawString(scoreFont, playerScore.ToString(), new Vector2(1085, 224), Color.Black);
            //sb.DrawString(scoreFont, "High Score\n\t" + highScore, new Vector2(20,300), Color.Gold);
            sb.DrawString(scoreFont, "Lives", new Vector2(1035, 480), Color.Navy);
            sb.DrawString(scoreFont, playerLives.ToString(), new Vector2(1085, 504), Color.Black);
            sb.DrawString(scoreFont, "Stage", new Vector2(1035, 620), Color.Navy);
            sb.DrawString(scoreFont, gameStage.ToString(), new Vector2(1085, 644), Color.Black);

            if (!init)
            {
                for (int i = 0; i < playerBall.Count; i++)
                {/*
                        for (int j = 0; j < 3; j++)
                        {
                            sb.DrawString(scoreFont, playerBall[i].Rect[j].X.ToString() + " " + playerBall[i].Rect[j].Y.ToString(), new Vector2(0, 50 * j), Color.Black);
                        }
                    }*/
                    //sb.DrawString(scoreFont, playerBat.Rect.X.ToString() + " " + playerBat.Rect.Y.ToString(), new Vector2(0, 10), Color.Black);
                }

                switch (inPlayState)
                {
                    case InPlayState.NewLevel:

                        //No need to render scene

                        break;

                    case InPlayState.Running:

                        for (int i = 0; i < blocks.Count(); i++)
                        {
                            blocks[i].DrawMe(sb, gt);
                        }

                        playerBat.DrawMe(sb, gt);

                        for (int i = 0; i < playerBall.Count(); i++)
                        {
                            playerBall[i].DrawMe(sb, gt);
                        }

                        for (int i = 0; i < powerUp.Count(); i++)
                        {
                            powerUp[i].DrawMe(sb, gt);
                        }

                        break;

                    case InPlayState.NewLife:

                        //No need to render scene

                        break;

                    case InPlayState.Paused:

                        for (int i = 0; i < blocks.Count(); i++)
                        {
                            blocks[i].DrawMe(sb, gt);
                        }

                        playerBat.DrawMe(sb, gt);

                        for (int i = 0; i < playerBall.Count(); i++)
                        {
                            playerBall[i].DrawMe(sb, gt);
                        }

                        //Draw semi transparent forground while paused
                        sb.Draw(pauseTxr, new Rectangle(0, 0, pauseTxr.Width, pauseTxr.Height), Color.White);

                        sb.DrawString(pauseFont, "Game Paused", new Vector2(500, 350), Color.Yellow);
                        sb.DrawString(pauseFont, "Hit Escape to Exit", new Vector2(477, 500), Color.Orange);
                        sb.DrawString(pauseFont, "or Enter to Resume", new Vector2(460, 550), Color.Orange);

                        break;

                    case InPlayState.GameOver:

                        //**Update this code to be more unique later

                        for (int i = 0; i < blocks.Count(); i++)
                        {
                            blocks[i].DrawMe(sb, gt);
                        }

                        playerBat.DrawMe(sb, gt);

                        for (int i = 0; i < playerBall.Count(); i++)
                        {
                            playerBall[i].DrawMe(sb, gt);
                        }

                        //Draw semi transparent forground while...
                        sb.Draw(pauseTxr, new Rectangle(0, 0, pauseTxr.Width, pauseTxr.Height), Color.White);

                        break;
                }
            }
        #endregion
        }
    }

    class Outro
    {
        #region Variables
        private bool fadeIn, fadeOut, animating;
        private Texture2D[] m_outroArt;

        //Used in Animation
        private double m_animDelay;
        private int m_currCell;
        private int m_cellOffset;

        //Use for Fade In/Out Effects
        private int m_AlphaValue;
        private int m_FadeIncrement;
        private double m_FadeDelay;
        private bool init;
        private int txrOffset;
        #endregion

        #region Methods
        public Outro(Texture2D[] txr)
        {
            m_AlphaValue = 1;
            m_FadeIncrement = 15;
            m_FadeDelay = .02;
            fadeIn = true;
            fadeOut = false;
            animating = false;
            init = true;

            m_animDelay = 0.2f;
            m_currCell = 0;
            m_cellOffset = 225;
            txrOffset = 0;

            m_outroArt = txr;
        }

        public void UpdateMe(GameTime gt, ref GameState gameState)
        {
            if (init)
            {
                Sound.StartTrack(6);
                init = false;
            }

            if (fadeIn)
            {
                //Decrement the delay
                m_FadeDelay -= gt.ElapsedGameTime.TotalSeconds;

                //If the Fade delays has dropped below zero, fade in/out a little more.
                if (m_FadeDelay <= 0)
                {
                    //Reset the Fade delay
                    m_FadeDelay = .02;

                    //Increment/Decrement the fade value for the image
                    m_AlphaValue += m_FadeIncrement;

                    //Stop fade in when fully realised
                    if (m_AlphaValue >= 255)
                    {
                        fadeIn = false;
                        animating = true;
                        //Switch to lowering for fadeOut
                        m_FadeIncrement *= -1;
                        m_FadeDelay = .03;
                    }
                }
            }

            txrOffset += 13;

            if (animating)
            {
                if (m_currCell < 10)
                {
                    m_animDelay -= gt.ElapsedGameTime.TotalSeconds;

                    if (m_animDelay <= 0)
                    {
                        m_animDelay = 0.2f;
                        m_currCell++;
                    }
                }
                else
                {
                    animating = false;
                    fadeOut = true;
                }
            }

            if (fadeOut)
            {
                //Decrement the delay
                m_FadeDelay -= gt.ElapsedGameTime.TotalSeconds;

                //If the Fade delays has dropped below zero, fade in/out a little more.
                if (m_FadeDelay <= 0)
                {
                    //Reset the Fade delay
                    m_FadeDelay = .035;

                    //Increment/Decrement the fade value for the image
                    m_AlphaValue += m_FadeIncrement;

                    //Stop fade in when fully realised
                    if (m_AlphaValue <= 0)
                    {
                        Sound.StopTrack();
                        gameState = GameState.Exit;
                    }
                }
            }
        }

        public void DrawMe(SpriteBatch sb)
        {
            sb.Draw(m_outroArt[0], new Rectangle(0, 0, 1366, 768), new Rectangle(0, 1280 - txrOffset, m_outroArt[0].Width, 768),
                        new Color(m_AlphaValue, m_AlphaValue, m_AlphaValue));
        }
        #endregion
    }
}
