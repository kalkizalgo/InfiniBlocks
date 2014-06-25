using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace InfiniBlocker
{
    enum GameState { Intro, Menu, Inplay, Outro, Exit }
    enum InPlayState { NewLevel, Running, NewLife, Paused, GameOver }
    enum MenuScreen { Main, NewGame, HighScore, Settings }
    enum AnimState { Normal, Hit, Death }
    enum GameStrength { Easy, Medium, Hard }
    enum BallState { Normal, Stuck, Powered }
    enum PowerType { Red, Green, Blue }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables
        public static readonly Random RNG = new Random();
        public static readonly Color[] blockColors = new Color[]{Color.Yellow, Color.DarkOrange, Color.Red, 
                                                                 Color.Lime, Color.DodgerBlue, Color.BlueViolet, 
                                                                 Color.LightGoldenrodYellow, Color.PeachPuff, 
                                                                 Color.LightSalmon, Color.PaleGreen, Color.LightSkyBlue, 
                                                                 Color.MediumPurple, Color.Gold, Color.Lavender};
        public static readonly Color[] powerColors = new Color[3]{Color.Red, Color.Green, Color.Blue};
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameState gameState;
        KeyboardState currKey, oldKey;
        MouseState currMouse, oldMouse;

        Intro gameIntro;
        Menu gameMenu;
        InPlay gameInPlay;
        Outro gameOutro;

        //Art Assets
        public Texture2D[] introArt;
        public Texture2D[] menuArt;
        public Texture2D[] inplayArt;
        public Texture2D[] outroArt;
        private SpriteFont debugFont;
        private SpriteFont scoreFont;
        private SpriteFont menuFont;
        private SpriteFont pauseFont;
        public static SoundEffect[] gameEffects;
        public static Song[] gameMusic;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            
            //Set graphics preferences
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1366;
            graphics.IsFullScreen = true;
            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            gameState = GameState.Intro;
            //Init inputs
            currKey = Keyboard.GetState();
            oldKey = currKey;
            currMouse = Mouse.GetState();
            oldMouse = currMouse;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            #region Loading Art Content
            debugFont = Content.Load<SpriteFont>(".\\Fonts\\DebugFont");
            scoreFont = Content.Load<SpriteFont>(".\\Fonts\\ScoreFont");
            menuFont = Content.Load<SpriteFont>(".\\Fonts\\MenuFont");
            pauseFont = Content.Load<SpriteFont>(".\\Fonts\\PauseFont");

            introArt = new Texture2D[3];
            introArt[0] = Content.Load<Texture2D>(".\\Images\\Intro\\IntroBackground");
            introArt[1] = Content.Load<Texture2D>(".\\Images\\Intro\\Logo");
            introArt[2] = Content.Load<Texture2D>(".\\Images\\Intro\\LogoMask");

            menuArt = new Texture2D[16];
            menuArt[0] = Content.Load<Texture2D>(".\\Images\\Menu\\MainMenuBack");
            menuArt[1] = Content.Load<Texture2D>(".\\Images\\Menu\\NewGameBack");
            menuArt[2] = Content.Load<Texture2D>(".\\Images\\Menu\\HighScoreBack");
            menuArt[3] = Content.Load<Texture2D>(".\\Images\\Menu\\SettingsBack");
            menuArt[4] = Content.Load<Texture2D>(".\\Images\\Menu\\NewGameButton");
            menuArt[5] = Content.Load<Texture2D>(".\\Images\\Menu\\HighScoreButton");
            menuArt[6] = Content.Load<Texture2D>(".\\Images\\Menu\\SettingsButton");
            menuArt[7] = Content.Load<Texture2D>(".\\Images\\Menu\\ExitGameButton");
            menuArt[8] = Content.Load<Texture2D>(".\\Images\\Menu\\EasyGameButton");
            menuArt[9] = Content.Load<Texture2D>(".\\Images\\Menu\\MediumGameButton");
            menuArt[10] = Content.Load<Texture2D>(".\\Images\\Menu\\HardGameButton");
            menuArt[11] = Content.Load<Texture2D>(".\\Images\\Menu\\Setting1Button");
            menuArt[12] = Content.Load<Texture2D>(".\\Images\\Menu\\Setting2Button");
            menuArt[13] = Content.Load<Texture2D>(".\\Images\\Menu\\Setting3Button");
            menuArt[14] = Content.Load<Texture2D>(".\\Images\\Menu\\EmptyButton");
            menuArt[15] = Content.Load<Texture2D>(".\\Images\\Menu\\BackGameButton");

            inplayArt = new Texture2D[10];
            inplayArt[0] = Content.Load<Texture2D>(".\\Images\\InPlay\\Bat");
            inplayArt[1] = Content.Load<Texture2D>(".\\Images\\InPlay\\Ball");
            inplayArt[2] = Content.Load<Texture2D>(".\\Images\\InPlay\\Block");
            inplayArt[3] = Content.Load<Texture2D>(".\\Images\\InPlay\\PowerUp");
            inplayArt[4] = Content.Load<Texture2D>(".\\Images\\InPlay\\InPlayBackground");
            inplayArt[5] = Content.Load<Texture2D>(".\\Images\\InPlay\\Border");
            inplayArt[6] = Content.Load<Texture2D>(".\\Images\\InPlay\\PlayingField");
            inplayArt[7] = Content.Load<Texture2D>(".\\Images\\InPlay\\InfoScreen");
            inplayArt[8] = Content.Load<Texture2D>(".\\Images\\InPlay\\PauseScreen");
            inplayArt[9] = Content.Load<Texture2D>(".\\Images\\InPlay\\BackTile");

            outroArt = new Texture2D[1];
            outroArt[0] = Content.Load<Texture2D>(".\\Images\\Outro\\OutroBackground");
            
            gameEffects = new SoundEffect[8];
            gameEffects[0] = Content.Load<SoundEffect>(".\\Audio\\Effects\\ButtonPress");
            gameEffects[1] = Content.Load<SoundEffect>(".\\Audio\\Effects\\GameOver");
            gameEffects[2] = Content.Load<SoundEffect>(".\\Audio\\Effects\\HitBlock");
            gameEffects[3] = Content.Load<SoundEffect>(".\\Audio\\Effects\\LoseLife");
            gameEffects[4] = Content.Load<SoundEffect>(".\\Audio\\Effects\\NewLevel");
            gameEffects[5] = Content.Load<SoundEffect>(".\\Audio\\Effects\\PowerUp");
            gameEffects[6] = Content.Load<SoundEffect>(".\\Audio\\Effects\\ServeBall");
            gameEffects[7] = Content.Load<SoundEffect>(".\\Audio\\Effects\\HitBat");

            gameMusic = new Song[8];
            gameMusic[0] = Content.Load<Song>(".\\Audio\\Music\\InPlayMusic01");
            gameMusic[1] = Content.Load<Song>(".\\Audio\\Music\\InPlayMusic02");
            gameMusic[2] = Content.Load<Song>(".\\Audio\\Music\\InPlayMusic03");
            gameMusic[3] = Content.Load<Song>(".\\Audio\\Music\\InPlayMusic04");
            gameMusic[4] = Content.Load<Song>(".\\Audio\\Music\\InPlayMusic05");
            gameMusic[5] = Content.Load<Song>(".\\Audio\\Music\\IntroMusic");
            gameMusic[6] = Content.Load<Song>(".\\Audio\\Music\\OutroMusic");
            gameMusic[7] = Content.Load<Song>(".\\Audio\\Music\\TitleTheme");

            MediaPlayer.IsRepeating = true;

            //Initialise Intro
            gameIntro = new Intro(introArt);
            gameMenu = new Menu(menuArt, menuFont);
            gameInPlay = new InPlay(inplayArt, scoreFont, pauseFont);
            gameOutro = new Outro(outroArt);

            #endregion
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            currMouse = Mouse.GetState();
            currKey = Keyboard.GetState();

            //**Remove this when polishing code
            if (currKey.IsKeyDown(Keys.P))
            {
                this.Exit();
            }

            #region GameState Loop
            switch (gameState)
            {
                case GameState.Intro:

                    gameIntro.UpdateMe(gameTime, ref gameState, currKey, oldKey);

                    break;

                case GameState.Menu:

                    //Put in some bool to check if reinit is needed
                    gameMenu.UpdateMe(currKey, oldKey, currMouse, oldMouse, ref gameState, ref gameInPlay);

                    break;

                case GameState.Inplay:

                    gameInPlay.UpdateMe(gameTime, ref gameState, currKey, oldKey);

                    break;

                case GameState.Outro:

                    gameOutro.UpdateMe(gameTime, ref gameState);

                    break;

                case GameState.Exit:

                    this.Exit();

                    break;
            }
            #endregion

            oldMouse = currMouse;
            oldKey = currKey;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            #region GameState Loop
            switch (gameState)
            {
                case GameState.Intro:

                    gameIntro.DrawMe(spriteBatch);

                    break;

                case GameState.Menu:

                    gameMenu.DrawMe(spriteBatch);

                    break;

                case GameState.Inplay:

                    gameInPlay.DrawMe(spriteBatch, gameTime);

                    break;

                case GameState.Outro:

                    gameOutro.DrawMe(spriteBatch);

                    break;

                case GameState.Exit:

                    this.Exit();

                    break;
            }
            #endregion

            //spriteBatch.DrawString(debugFont, 

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
