using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ass1
{
    public class Text : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private float currentTime;
        private float countDuration = 1f;//every 1s
        private int maxTime = 50;
        private int minTime = 0;
        public int Score { get; private set; } // display score
        public int Time { get; private set; } //display time
        public int Life { get; private set; } //display life

        private SpriteFont font;
        private string top_left_text, top_middle_text, top_right_text;
        private Vector2 top_left_text_pos, top_middle_text_pos, top_right_text_pos;

        //9-4 updated code
        private string game_over;
        private Vector2 game_over_pos;
        private Texture2D gameOverScreen;
        private bool isGameOver = false;
        private double quitTime = 4;

        enum GameState
        {
            MainMenu,
            Options,
            Playing,
        }
        GameState CurrentGameState = GameState.MainMenu;


        Button btnRePlay;

        public Text(Game game) : base(game)
        {

        }
        public override void Initialize()
        {
            Time = 60;
            Life = 3;

            top_left_text = "Score: " + Score;
            top_middle_text = "Time: " + Time;
            top_right_text = "Life: " + Life;



            top_left_text_pos = new Vector2(10, 10);
            top_middle_text_pos = new Vector2(Game.Window.ClientBounds.Size.X / 2.5f, 10);
            top_right_text_pos = new Vector2(Game.Window.ClientBounds.Size.X / 1.2f, 10);

            //9-4 updated code
            game_over = "Game Over";
            game_over_pos = new Vector2(Game.Window.ClientBounds.Size.X / 2,
                Game.Window.ClientBounds.Size.Y / 2);

            base.Initialize();
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            ((Game1)Game).spriteBatch.Begin();
            ((Game1)Game).spriteBatch.DrawString(font, top_left_text, top_left_text_pos, Color.Black);
            ((Game1)Game).spriteBatch.DrawString(font, top_middle_text, top_middle_text_pos, Color.Yellow);
            ((Game1)Game).spriteBatch.DrawString(font, top_right_text, top_right_text_pos, Color.Red);
            if (isGameOver)
            {
                ((Game1)Game).spriteBatch.Draw(gameOverScreen, new Rectangle(0, 0, 1280, 720), Color.White);
            }
            ((Game1)Game).spriteBatch.End();
        }
        protected override void LoadContent()
        {
            font = Game.Content.Load<SpriteFont>(@"Fonts/Arial");

            btnRePlay = new Button(Game.Content.Load<Texture2D>(@"Image/menu_button"), ((Game1)Game).graphics.GraphicsDevice);
            btnRePlay.setPosition(new Vector2(350, 300));
            gameOverScreen = Game.Content.Load<Texture2D>(@"Image/gameover");

            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            countScore();
            countTime(gameTime);
            countLife();

            //Restart game
            if (this.Life < 0)
            {
                isGameOver = true;
                quitTime -= gameTime.ElapsedGameTime.TotalSeconds;
                if (quitTime < 1)
                    ((Game1)Game).Exit();
            }
            base.Update(gameTime);
        }
        public void updateScore(int score)
        {
            this.Score += score;
        }
        public void updateTime(int time, bool addTime)
        {
            if (addTime)
            {
                this.Time += time;
            }
            else
            {
                this.Time -= time;
            }
        }
        public void updateLife(int life, bool addLife)
        {
            if (addLife)
            {
                this.Life += life;
            }
            else
            {
                this.Life -= life;
            }
        }
        private void countScore()
        {
            top_left_text = "Score: " + Score;
        }
        private void countTime(GameTime gameTime)
        {
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //CountUp
            //if (currentTime >= countDuration)
            //{
            //    time++;
            //    currentTime -= countDuration; 
            //}
            //if (time >= maxTime)
            //{
            //    //Event happen
            //    time = 0;     
            //}

            //Countdown
            if (currentTime >= countDuration)
            {
                Time--;
                currentTime -= countDuration;
            }
            if (Time <= minTime)
            {
                //Event happen
                Time = 4;
            }
            top_middle_text = "Time: " + Time;
        }

        private void countLife()
        {
            top_right_text = "Life: " + Life;
        }
    }
}
