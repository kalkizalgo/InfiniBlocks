using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InfiniBlocker
{
    enum BlockType { Normal, Solid }

    class MenuButtons
    {
        #region Variables
        private Texture2D[] m_buttonArt;
        private Rectangle[] m_buttonPos;
        private int selectedButton;
        private bool[] onOrOff;
        private MenuScreen m_screen;

        //Keeps track of which textures to use for buttons
        //Due to repeats of textures
        private int[,] txrChoices = { { 0, 1, 2, 3 }, { 4, 5, 6, 11 }, { 10, 10, 10, 11 }, { 7, 8, 9, 11 } };
        #endregion

        #region Methods
        public MenuButtons(Texture2D[] txr)
        {
            selectedButton = 0;
            m_buttonArt = txr;
            onOrOff = new bool[4];
            m_buttonPos = new Rectangle[4];

            onOrOff[0] = true;
            onOrOff[1] = false;
            onOrOff[2] = false;
            onOrOff[3] = false;

            //**Need to initialise positions for all rectangles
            //Just use the same four rectangles
            //**Update later
            m_buttonPos[0] = new Rectangle(1070, 265, 225, 75);
            m_buttonPos[1] = new Rectangle(1070, 390, 225, 75);
            m_buttonPos[2] = new Rectangle(1070, 515, 225, 75);
            m_buttonPos[3] = new Rectangle(1070, 640, 225, 75);
        }

        public MenuScreen UpdateMe(KeyboardState currKb, KeyboardState oldKb, MouseState currMs, MouseState oldMS, MenuScreen menuScreen, ref GameState gameState, ref InPlay gameInPlay)
        {
            m_screen = menuScreen;

            if (currKb.IsKeyDown(Keys.Up) && oldKb.IsKeyUp(Keys.Up))
            {
                Sound.PlayEffect(0);
                selectedButton = (Math.Abs((selectedButton - 1) * 4) + (selectedButton - 1)) % 4;
            }
            if (currKb.IsKeyDown(Keys.Down) && oldKb.IsKeyUp(Keys.Down))
            {
                Sound.PlayEffect(0);
                selectedButton = (selectedButton + 1) % 4;
            }

            for (int i = 0; i < 4; i++)
            {
                if (m_buttonPos[i].Contains(currMs.X, currMs.Y))
                {
                    selectedButton = i;
                }
            }

            //Disallow movement if on HighScore screen
            if (m_screen == MenuScreen.HighScore)
            {
                selectedButton = 3;
            }

            for (int i = 0; i < 4; i++)
            {
                if (selectedButton == i)
                {
                    onOrOff[i] = true;
                }
                else
                {
                    onOrOff[i] = false;
                }
            }

            //**Add in mouse click event
            if (currKb.IsKeyDown(Keys.Enter) && oldKb.IsKeyUp(Keys.Enter))
            {
                Sound.PlayEffect(0);

                switch (m_screen)
                {
                    case MenuScreen.Main:

                        switch (selectedButton)
                        {
                            case 0:

                                m_screen = MenuScreen.NewGame;

                                break;

                            case 1:

                                m_screen = MenuScreen.HighScore;

                                break;

                            case 2:

                                m_screen = MenuScreen.Settings;

                                break;

                            case 3:

                                //Switches Game1 Update to play Outro
                                gameState = GameState.Outro;

                                break;
                        }

                        break;

                    case MenuScreen.NewGame:

                        switch (selectedButton)
                        {
                            case 0:

                                //Easy
                                gameInPlay.Strength = GameStrength.Easy;
                                gameState = GameState.Inplay;
                                m_screen = MenuScreen.Main;
                                break;

                            case 1:

                                //Medium
                                gameInPlay.Strength = GameStrength.Medium;
                                gameState = GameState.Inplay;
                                m_screen = MenuScreen.Main;
                                break;

                            case 2:

                                //Hard
                                gameInPlay.Strength = GameStrength.Hard;
                                gameState = GameState.Inplay;
                                m_screen = MenuScreen.Main;
                                break;

                            case 3:

                                m_screen = MenuScreen.Main;

                                break;
                        }

                        break;

                    case MenuScreen.HighScore:

                        //Only one choice, back to main menu
                        m_screen = MenuScreen.Main;

                        break;

                    case MenuScreen.Settings:

                        switch (selectedButton)
                        {
                            case 0:

                                //**TBD
                                //Full screen
                                //Resolution
                                //Volume: music & effects
                                //reset scores

                                break;

                            case 1:

                                if (Sound.SoundOn == true)
                                {
                                    Sound.soundOn = false;
                                    Sound.StopTrack();
                                }
                                else
                                {
                                    Sound.soundOn = true;
                                }
                                
                                break;

                            case 2:

                                ResetFile();

                                break;

                            case 3:

                                m_screen = MenuScreen.Main;

                                break;
                        }

                        break;
                }
                selectedButton = 0;

                return m_screen;



            }
            else
            {
                return m_screen;
            }
        }

        public void DrawMe(SpriteBatch sb)
        {
            for (int j = 0; j < 4; j++)
            {
                int animCell;
                if(j == selectedButton)
                {
                    animCell = 125;
                }
                else
                {
                    animCell = 0;
                }

                //Change m_buttonArt selections depending on current screen
                //Uses txrChoices to set texture used
                sb.Draw(m_buttonArt[txrChoices[(int)m_screen, j]], new Rectangle(m_buttonPos[j].X - 25, m_buttonPos[j].Y -25, 275, 125), new Rectangle(0, animCell, 275, 125), Color.White);
            }
        }

        public void ResetFile()
        {
            //File.Delete("scores.dat");
            //File.Create("scores.dat");
            FileStream scoreFile = new FileStream("scores.dat", FileMode.Open, FileAccess.Write);
            BinaryWriter newWriter = new BinaryWriter(scoreFile);
            newWriter.Write(0);
            newWriter.Flush();
            newWriter.Close();
        }
        #endregion
    }

    class Blocks
    {
        #region Variables
        private Vector2 m_pos;
        private Texture2D m_txr;
        private int m_health;
        private Rectangle m_boundingBox;
        private BlockType m_blockType;
        private Color blockColor, animColor;

        //Animation Variables
        private AnimState m_animState;
        private Rectangle m_animCell;
        private double m_frameTimer;
        private float m_fps;

        public Rectangle Rect
        {
            get
            {
                return m_boundingBox;
            }
        }

        public int Health
        {
            get
            {
                return m_health;
            }
            set
            {
                m_health = value;
            }
        }

        public AnimState Anim
        {
            get
            {
                return m_animState;
            }
            set
            {
                m_animState = value;
            }
        }

        public double Timer
        {
            get
            {
                return m_frameTimer;
            }
            set
            {
                m_frameTimer = value;
            }
        }

        public BlockType Type
        {
            get
            {
                return m_blockType;
            }
        }

        #endregion

        #region Methods
        //, Color color, BlockType blockType
        public Blocks(Texture2D txr, int xPos, int yPos, int currColor)
        {
            m_animState = AnimState.Normal;
            m_txr = txr;
            m_pos.X = (xPos * 55) + 170;
            m_pos.Y = (yPos * 35) + 34;

            //Three bounding boxes for greater accuracy.
            m_boundingBox = new Rectangle((int)m_pos.X, (int)m_pos.Y, m_txr.Width, m_txr.Height);

            blockColor = Game1.blockColors[currColor];
            animColor = blockColor;

            //Change below later
            m_health = 2;

        }

        public void UpdateMe(GameTime gt)
        {
            //Run animation if hit
            //Only switch state if solid
            //Update score if death

            //
            if (m_animState == AnimState.Hit)
            {

                m_frameTimer -= gt.ElapsedGameTime.TotalSeconds;
                ResetAnimColor(m_frameTimer);
                if (m_frameTimer <= 0)
                {
                    m_animState = AnimState.Normal;
                }
            }
            /*
            if (m_health <= 0)
            {
                m_animState = AnimState.Death;

            }
            */

        }

        public void DrawMe(SpriteBatch sb, GameTime gt)
        {
            switch (m_animState)
            {
                case AnimState.Normal:
                    //**Change Color later
                    
                    sb.Draw(m_txr, new Rectangle((int)m_pos.X, (int)m_pos.Y, m_txr.Width, m_txr.Height), blockColor);

                    break;
                case AnimState.Hit:

                    sb.Draw(m_txr, new Rectangle((int)m_pos.X, (int)m_pos.Y, m_txr.Width, m_txr.Height), animColor);

                    break;

                case AnimState.Death:

                    break;
            }
        }

        public void ResetAnimColor(double timer)
        {
            double tempR, tempG, tempB;

            tempR = blockColor.R + (1024 * timer);
            if (tempR > 254)
            {
                tempR = 254;
            }
            tempG = blockColor.G + (1024 * timer);
            if (tempG > 254)
            {
                tempG = 254;
            }
            tempB = blockColor.B + (1024 * timer);
            if (tempB > 254)
            {
                tempB = 254;
            }

            animColor.R = (byte)(tempR);
            animColor.G = (byte)(tempG);
            animColor.B = (byte)(tempB);
        }

        public void NewLevel()
        {

        }
        #endregion
    }

    class Bat
    {
        #region Variables
        private Vector2 m_pos;
        private Texture2D m_txr;
        private Rectangle m_boundingBox;
        private Rectangle m_screenBounds;
        private float m_velL, m_velR;
        //private float m_acc;

        //Animation Variables
        private AnimState m_animState;
        private Rectangle m_animCell;
        private float m_frameTimer;
        private float m_fps;

        public Rectangle Rect
        {
            get
            {
                return m_boundingBox;
            }
        }

        public float VelL
        {
            get
            {
                return m_velL;
            }
        }

        public float VelR
        {
            get
            {
                return m_velR;
            }
        }

        public float Vel
        {
            get
            {
                return m_velR - m_velL;
            }
        }
        #endregion

        #region Methods
        public Bat(Texture2D txr, Rectangle screenBounds)
        {
            m_animState = AnimState.Normal;
            m_velL = 0;
            m_velR = 0;
            m_screenBounds = screenBounds;
            //Initialise Position
            m_pos = new Vector2(540, 665);
            m_boundingBox = new Rectangle((int)m_pos.X, (int)m_pos.Y, 100, 25);
            m_txr = txr;
        }

        public void UpdateMe(KeyboardState currKb)
        {
            //Add in check for intersection with powerups

            //Adjust velocity based on input.
            if (currKb.IsKeyDown(Keys.A) || currKb.IsKeyDown(Keys.Left))
            {
                m_velL += 3;
                if (m_velL > 14)
                {
                    m_velL = 14;
                }
            }
            else
            {
                m_velL -= 2;
                if (m_velL < 0)
                {
                    m_velL = 0;
                }
            }

            if (currKb.IsKeyDown(Keys.D) || currKb.IsKeyDown(Keys.Right))
            {
                m_velR += 3;
                if (m_velR > 14)
                {
                    m_velR = 14;
                }
            }
            else
            {
                m_velR -= 2;
                if (m_velR < 0)
                {
                    m_velR = 0;
                }
            }

            //Adjust screen position from velocity
            m_pos.X += m_velR;
            m_pos.X -= m_velL;

            //Adjust collision sphere
            m_boundingBox.X = (int)m_pos.X;
            m_boundingBox.Y = (int)m_pos.Y;

            //Make sure sphere is in bounds
            //**Put in adjusted screen bounds, maybe use a rectangle
            if (m_pos.X < m_screenBounds.X)
            {
                m_pos.X = m_screenBounds.X;
                m_velL = 0;
            }
            if (m_pos.X + m_txr.Width > m_screenBounds.X + m_screenBounds.Width)
            {
                m_pos.X = m_screenBounds.X + m_screenBounds.Width - m_txr.Width;
                m_velR = 0;
            }
        }

        public void DrawMe(SpriteBatch sb, GameTime gt)
        {
            //Change to suit powerup states too..
            switch (m_animState)
            {
                case AnimState.Normal:

                    sb.Draw(m_txr, new Rectangle((int)m_pos.X, (int)m_pos.Y, m_txr.Width, m_txr.Height), Color.White);

                    break;
                case AnimState.Hit:

                    break;

                case AnimState.Death:

                    break;
            }
        }

        public void Reset()
        {
            m_pos.X = 540;
            m_pos.X = 665;

            m_velL = 0;
            m_velR = 0;

            m_boundingBox.X = (int)m_pos.X;
            m_boundingBox.Y = (int)m_pos.Y;
        }

        #endregion
    }

    class Ball
    {
        #region Variables
        private Vector2 m_pos;
        private Texture2D m_txr;
        //private BoundingSphere m_boundingSphere;
        private Rectangle[] m_boundingBoxes;
        private Vector2 m_vel;
        //Represents overall speed level
        private float m_ballAcc;
        private Rectangle m_screenBounds;
        //private Vector2 m_acc;

        //Animation Variables
        private AnimState m_animState;
        private Rectangle m_animCell;
        private float m_frameTimer;
        private float m_fps;

        private BallState ballState;

        public BallState State
        {
            get
            {
                return ballState;
            }
            set
            {
                ballState = value;
            }
        }

        public Vector2 Pos
        {
            get
            {
                return m_pos;
            }
            set
            {
                m_pos = value;
            }
        }

        public Rectangle[] Rect
        {
            get
            {
                return m_boundingBoxes;
            }
            set
            {
                m_boundingBoxes = value;
            }
        }

        public float VecX
        {
            get
            {
                return m_vel.X;
            }
            set
            {
                m_vel.X = value;
            }
        }

        public float VecY
        {
            get
            {
                return m_vel.Y;
            }
            set
            {
                m_vel.Y = value;
            }
        }

        #endregion

        #region Methods
        public Ball(Texture2D txr, Rectangle screenBounds)
        {
            m_animState = AnimState.Normal;
            ballState = BallState.Stuck;
            m_txr = txr;
            //**Adjust Later
            //Set m_pos as center of Bounding Sphere
            m_pos = new Vector2(578, 475);

            //m_boundingBoxes[0] = new Rectangle((int)m_pos.X, (int)m_pos.Y, m_txr.Width, m_txr.Height);
            m_boundingBoxes = new Rectangle[4];
            m_boundingBoxes[0] = new Rectangle((int)m_pos.X + 3, (int)m_pos.Y, 19, 25);
            m_boundingBoxes[1] = new Rectangle((int)m_pos.X, (int)m_pos.Y + 3, 25, 19);
            m_boundingBoxes[2] = new Rectangle((int)m_pos.X + 3, (int)m_pos.Y + 3, 19, 19);
            m_boundingBoxes[3] = new Rectangle((int)m_pos.X, (int)m_pos.Y, 25, 25);

            //m_boundingBox = new Rectangle((int)m_pos.X, (int)m_pos.Y, 25, 25);
            //m_boundingSphere = new BoundingSphere(new Vector3(m_pos, 0), m_txr.Width / 2);
        }

        public void UpdateMe()
        {

            double hypoTen, ratioX, ratioY;

            hypoTen = Math.Sqrt((m_vel.X * m_vel.X) + (m_vel.Y * m_vel.Y));
            ratioX = m_vel.X / hypoTen;
            ratioY = m_vel.Y / hypoTen;

            if (hypoTen > 10)
            {
                hypoTen *= 0.8;
                m_vel.X = (float)(ratioX * hypoTen);
                m_vel.Y = (float)(ratioY * hypoTen);
            }
            if (hypoTen < 5 && ballState != BallState.Stuck)
            {
                hypoTen *= 1.2;
                m_vel.X = (float)(ratioX * hypoTen);
                m_vel.Y = (float)(ratioY * hypoTen);
            }

            if (m_vel.Y > -0.1f && m_vel.Y < 0.1f)
            {
                m_vel.Y += 0.1f;
            }

            //Add velocity to position
            m_pos.X += m_vel.X;
            m_pos.Y += m_vel.Y;

            //m_boundingBox.X = (int)m_pos.X + 7;
           //m_boundingBox.Y = (int)m_pos.Y;

            m_boundingBoxes[0].X = (int)m_pos.X + 3;
            m_boundingBoxes[0].Y = (int)m_pos.Y;
            
            m_boundingBoxes[1].X = (int)m_pos.X;
            m_boundingBoxes[1].Y = (int)m_pos.Y + 3;

            m_boundingBoxes[2].X = (int)m_pos.X + 3;
            m_boundingBoxes[2].Y = (int)m_pos.Y + 3;

            m_boundingBoxes[3].X = (int)m_pos.X;
            m_boundingBoxes[3].Y = (int)m_pos.Y;
            
        }

        public void DrawMe(SpriteBatch sb, GameTime gt)
        {
            switch (m_animState)
            {
                case AnimState.Normal:

                    sb.Draw(m_txr, new Rectangle((int)m_pos.X, (int)m_pos.Y, m_txr.Width, m_txr.Height), Color.White);

                    break;
                case AnimState.Hit:

                    break;

                case AnimState.Death:

                    break;
            }
        }

        public void Reset()
        {
            m_pos.X = 0;
            m_pos.Y = 0;

            m_vel.X = -1;
            m_vel.Y = -1;
        }

        public void NewLevel()
        {

        }
        #endregion
    }

    class PowerUp
    {
        #region Variables
        private Vector2 m_pos;
        private Texture2D m_txr;
        private int m_health;
        private Rectangle m_boundingBox;
        private float m_vel;
        private PowerType m_PowerType;
        private Color powerColor;

        //Animation Variables
        private AnimState m_animState;
        private int m_animCell;
        private float m_frameTimer;

        public Rectangle Rect
        {
            get
            {
                return m_boundingBox;
            }
        }

        public AnimState Anim
        {
            get
            {
                return m_animState;
            }
            set
            {
                m_animState = value;
            }
        }

        public PowerType Power
        {
            get
            {
                return m_PowerType;
            }
            set
            {
                m_PowerType = value;
            }
        }
        #endregion

        #region Methods
        public PowerUp(Texture2D txr, int xPos, int yPos)
        {
            m_animState = AnimState.Normal;
                        m_txr = txr;
            m_pos.X = xPos;
            m_pos.Y = yPos;
            m_boundingBox = new Rectangle((int)m_pos.X, (int)m_pos.Y, 55, 25);
            m_vel = 4;
            m_frameTimer = 0.2f;
            m_animCell = 0;

            //Change depending on**
            int randPower = Game1.RNG.Next(0, 2);
            
            switch (randPower)
            {
                case 0:
                    m_PowerType = PowerType.Red;
                    break;
                case 1:
                    m_PowerType = PowerType.Green;
                    break;
                case 2:
                    m_PowerType = PowerType.Blue;
                    break;
                default:
                    m_PowerType = PowerType.Red;
                    randPower = 0;
                    break;
            }

            powerColor = Game1.powerColors[(int)m_PowerType];
        }

        public void UpdateMe()
        {
            m_boundingBox.Y += (int)m_vel;
        }

        public void DrawMe(SpriteBatch sb, GameTime gt)
        {
            switch (m_animState)
            {
                case AnimState.Normal:

                    if(m_frameTimer >= 0)
                    {
                        m_frameTimer -= (float)gt.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        m_frameTimer = 0.2f;
                        m_animCell++;
                        if(m_animCell > 4)
                        {
                            m_animCell = 0;
                        }
                    }

                    sb.Draw(m_txr, m_boundingBox, new Rectangle(0, m_animCell * 25, 55, 25), powerColor);

                    break;
                case AnimState.Hit:



                    break;

                case AnimState.Death:

                    break;
            }
        }
        #endregion
    }

    static class Sound
    {
        #region Variables

        private static SoundEffectInstance blockHit = Game1.gameEffects[2].CreateInstance();
        public static bool soundOn = true;

        public static bool SoundOn
        {
            get
            {
                return soundOn;
            }
            set
            {
                soundOn = value;
            }
        }

        #endregion

        #region Methods

        static Sound()
        {

        }

        public static void PlayEffect(int track)
        {
            if (soundOn == true)
            {
                Game1.gameEffects[track].Play();
            }
            /*
            if (track == 2)
            {
                SoundEffectInstance blockHit = Game1.gameEffects[2].CreateInstance();
                blockHit.Play();
            }
            else
            {
                Game1.gameEffects[track].Play();
            }
            */
        }

        public static void StartTrack(int track)
        {
            if (soundOn == true)
            {
                if (track == 0)
                {
                    track = Game1.RNG.Next(0, 4);
                }
                MediaPlayer.Play(Game1.gameMusic[track]);
            }
        }

        public static void StopTrack()
        {
            MediaPlayer.Stop();
        }

        #endregion
    }
}
