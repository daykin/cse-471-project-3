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

namespace Game1
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D player1;
        Texture2D player2;
        Texture2D p1Eye;
        Texture2D p2Eye;

        float p1EyeRotation = 0;
        float p2EyeRotation = 0;
        Texture2D ball;
        Texture2D net;
        Vector2 netPosition;
        Vector2 spritePosition1;
        Vector2 spritePosition2;
        Vector2 p1EyePosition;
        Vector2 p2EyePosition;
        Vector2 spriteSpeed1 = new Vector2(0f,0f);
        Vector2 spriteSpeed2 = new Vector2(0f,0f);
        //Vector2 spriteAcceleration1 = new Vector2(0f, 0f);
        Vector2 ballPosition;
        Texture2D background;
        Rectangle mainFrame;
        float ballAcceleration= 2.0f;

        Vector2 ballVelocity = new Vector2(0f, 0f);
        Vector2 maxBallVelocity = new Vector2(300f, 500f);
        Vector2 maxBallReboundVelocity = new Vector2(190f, 190f);
        //double minReboundVelocity = 200;
        Vector2 backgroundPosition;
        int sprite1WidthMinusHeight;
        int sprite2WidthMinusHeight;
        int sprite1Height;
        int sprite1Width;
        int sprite2Height;
        int sprite2Width;
        Vector2 player1Center;
        Vector2 player2Center;
        Vector2 ballCenter;
        int player1Radius = 58;
        int player2Radius = 50;
        KeyboardState oldState;
        int MaxX;
        int MinX;
        int MaxY;
        int MinY;
        float playerAcceleration = 10f;
        SoundEffect soundEffect;
        SoundEffect xp;
        private int ballRadius = 15;
        private Vector2 p2eyeOrigin;
        private Vector2 p1eyeOrigin;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            oldState = Keyboard.GetState();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures

            spriteBatch = new SpriteBatch(GraphicsDevice);

            player1 = Content.Load<Texture2D>("sparty");
            player2 = Content.Load<Texture2D>("garbage");
            p1Eye = Content.Load<Texture2D>("eyeball");
            p2Eye = Content.Load<Texture2D>("eyeball2");
            net = Content.Load<Texture2D>("net");
            ball = Content.Load<Texture2D>("ball");
            sprite1WidthMinusHeight = player1.Width - player1.Height;
            sprite2WidthMinusHeight = player2.Width - player2.Height;
            soundEffect = Content.Load<SoundEffect>("chimes");
            xp = Content.Load<SoundEffect>("xp");
            background = Content.Load<Texture2D>("eb");

            MaxX = graphics.GraphicsDevice.Viewport.Width;
            MinX = 0;
            MaxY = graphics.GraphicsDevice.Viewport.Height;
            MinY = 0;
            spritePosition1.X = graphics.GraphicsDevice.Viewport.Width/4 -player1.Width/2;
            spritePosition1.Y = graphics.GraphicsDevice.Viewport.Height-player1.Height;

            netPosition.X = graphics.GraphicsDevice.Viewport.Width / 2 - net.Width / 2;
            netPosition.Y = graphics.GraphicsDevice.Viewport.Height-net.Height;

            backgroundPosition.X = graphics.GraphicsDevice.Viewport.Width / 2;
            backgroundPosition.Y = graphics.GraphicsDevice.Viewport.Height / 2;

            spritePosition2.X = graphics.GraphicsDevice.Viewport.Width*3/4 - player2.Width/2;
            spritePosition2.Y = graphics.GraphicsDevice.Viewport.Height - player2.Height;

            sprite1Height = player1.Bounds.Height;
            sprite1Width = player1.Bounds.Width;

            sprite2Height = player2.Bounds.Height;
            sprite2Width = player2.Bounds.Width;

            p1eyeOrigin = new Vector2(p1Eye.Width / 2, p1Eye.Height / 2);
            p2eyeOrigin = new Vector2(p2Eye.Width / 2, p2Eye.Height / 2);

            ballPosition.X = graphics.GraphicsDevice.Viewport.Width/2;
            ballPosition.Y = 100;
            ballVelocity.X = 20;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allow the game to exit

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Move the sprite around

            UpdateSprite(gameTime, ref spritePosition1, ref spriteSpeed1);
            UpdateSprite(gameTime, ref spritePosition2, ref spriteSpeed2);

            base.Update(gameTime);
        }

        void UpdateSprite(GameTime gameTime, ref Vector2 spritePosition, ref Vector2 spriteSpeed)
        {

            // Move the sprite by speed, scaled by elapsed time 

            //spritePosition += spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            applyGravity();
            captureInput();
            spritePosition.X += spriteSpeed.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            spritePosition.Y += spriteSpeed.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            updateBall((float)gameTime.ElapsedGameTime.TotalSeconds);
            p1EyePosition.X = spritePosition1.X + 88;
            p1EyePosition.Y = spritePosition1.Y + 54;
            if (p1EyePosition.X < ballPosition.X)
            {
                p1EyeRotation = Convert.ToSingle(Math.Atan((ballPosition.Y - p1EyePosition.Y) / (ballPosition.X - p1EyePosition.X)));
            }
            else
            {
                p1EyeRotation = Convert.ToSingle(Math.PI + Math.Atan((ballPosition.Y - p1EyePosition.Y) / (ballPosition.X - p1EyePosition.X)));
            }
            p2EyePosition.X = spritePosition2.X + 24;
            p2EyePosition.Y = spritePosition2.Y + 25;
            if (p2EyePosition.X > ballPosition.X)
            {
                p2EyeRotation = Convert.ToSingle(Math.Atan((ballPosition.Y - p2EyePosition.Y) / (ballPosition.X - p2EyePosition.X)));
            }
            else
            {
                p2EyeRotation = Convert.ToSingle(Math.PI+Math.Atan((ballPosition.Y - p2EyePosition.Y) / (ballPosition.X - p2EyePosition.X)));
            }
            // Check for bounce 
            updateBall((float)gameTime.ElapsedGameTime.TotalSeconds);

            
        }

        void captureInput()
        {
            KeyboardState current = Keyboard.GetState();
            if (current.IsKeyDown(Keys.D) &&current.IsKeyUp(Keys.A) &&spritePosition1.X+player1.Width<netPosition.X)
            {
                spriteSpeed1.X = 300;
            }
            
            else if (current.IsKeyDown(Keys.A) && current.IsKeyUp(Keys.D)&&spritePosition1.X>MinX-16){
                spriteSpeed1.X = -300;
            }
            else
            {
                spriteSpeed1.X = 0;
            }

            if (current.IsKeyDown(Keys.Right) && current.IsKeyUp(Keys.Left)&&spritePosition2.X<MaxX-100)
            {
                spriteSpeed2.X = 300;
            }

            else if (current.IsKeyDown(Keys.Left) && current.IsKeyUp(Keys.Right)&&spritePosition2.X>netPosition.X+net.Width){
                spriteSpeed2.X = -300;
            }
            else
            {
                spriteSpeed2.X = 0;
            }

            if (current.IsKeyDown(Keys.W) &&spritePosition1.Y== graphics.GraphicsDevice.Viewport.Height - player1.Height)
            {
                spritePosition1.Y -= 1;
                spriteSpeed1.Y = -450;
            }

            if (current.IsKeyDown(Keys.Up) && spritePosition2.Y == graphics.GraphicsDevice.Viewport.Height - player2.Height)
            {
                spritePosition2.Y -= 1;
                spriteSpeed2.Y = -450;
            }


        }
       

        void applyGravity()
        { 
            if ((ballCenter.Y < MaxY - ballRadius) && ballVelocity.Y < maxBallVelocity.Y)
            {
                ballVelocity.Y += ballAcceleration;
            }

            if (spritePosition1.Y < graphics.GraphicsDevice.Viewport.Height - player1.Height)
                {
                    spriteSpeed1.Y += playerAcceleration;
                }
            else
            {
                spritePosition1.Y = graphics.GraphicsDevice.Viewport.Height - player1.Height;
                spriteSpeed1.Y = 0;
            }

            if (spritePosition2.Y < graphics.GraphicsDevice.Viewport.Height - player2.Height)
            {
                spriteSpeed2.Y += playerAcceleration;
            }
            else
            {
                spritePosition2.Y = graphics.GraphicsDevice.Viewport.Height - player2.Height;
                spriteSpeed2.Y = 0;
            }

        }

        void playerRebound()
        {
            // when ball hits player....
            // distance from ball center to player center is less than the sum of their radii

            ballCenter = new Vector2(ballPosition.X + ball.Width/2, ballPosition.Y + ball.Height / 2);
            player1Center = new Vector2(spritePosition1.X + player1.Width / 2, spritePosition1.Y + player1.Height);// dimensions are 115 x 84
            player2Center = new Vector2(spritePosition2.X + player2.Width / 2, spritePosition2.Y + player2.Height);// dimensions are 100 x 50

            if (Vector2.Distance(ballCenter, player2Center) <= (ballRadius + player2Radius) ) // radius = 50
            {
                Bounce(ref ballVelocity, ref ballPosition, ref spriteSpeed2, ref spritePosition2, ballCenter, player2Center);
            }
            else if (Vector2.Distance(ballCenter, player1Center) <= (ballRadius + player1Radius)) // radius = 84
            {
                Bounce(ref ballVelocity, ref ballPosition, ref spriteSpeed1, ref spritePosition1, ballCenter, player1Center);
            }
        }

        void Bounce(ref Vector2 ballVelocity, ref Vector2 ballPosition, ref Vector2 spriteVelocity, ref Vector2 spritePosition, Vector2 ballCenter, Vector2 spriteCenter)
        {
            double xDist = spriteCenter.X - ballCenter.X;
            double yDist = spriteCenter.Y - ballCenter.Y;
            double distSquared = xDist * xDist + yDist * yDist;
            float xVelocity = ballVelocity.X - spriteVelocity.X;
            float yVelocity = ballVelocity.Y - spriteVelocity.Y;
            double dotProduct = xDist * xVelocity + yDist * yVelocity;
            //Neat vector maths, used for checking if the objects moves towards one another.
            if (dotProduct > 0)
            {
                double collisionScale = dotProduct / distSquared;
                double xCollision = xDist * collisionScale;
                double yCollision = yDist * collisionScale;
                //The Collision vector is the speed difference projected on the Dist vector,
                //thus it is the component of the speed difference needed for the collision.
                ballVelocity.X -= (float)(2 * xCollision);
                ballVelocity.Y -= (float)(2 * yCollision);
            }



            // check if velocity is too high
            if (ballVelocity.X > 0)
            {
                if (ballVelocity.X > maxBallReboundVelocity.X)
                {
                    ballVelocity.X = maxBallReboundVelocity.X;
                }
            }
            else
            {
                if (ballVelocity.X * -1 > maxBallReboundVelocity.X)
                {
                    ballVelocity.X = maxBallReboundVelocity.X * -1;
                }
            }

            if (ballVelocity.Y > 0)
            {
                if (ballVelocity.Y > maxBallReboundVelocity.Y)
                {
                    ballVelocity.Y = maxBallReboundVelocity.Y;
                }
            }
            else
            {
                if (ballVelocity.Y * -1 > maxBallReboundVelocity.Y)
                {
                    ballVelocity.Y = maxBallReboundVelocity.Y * -1;
                }
            }

            
        }

        void checkForBallCollision(bool x)
        {
            if (x)
            {

                if (MaxX - ballPosition.X < 2 * ballRadius)
                {
                    ballPosition.X = MaxX - 2 * ballRadius;
                    ballVelocity.X *= -1;
                }

                else if (ballPosition.X < 0)
                {
                    ballPosition.X = MinX;
                    ballVelocity.X *= -1;
                }

                else if (ballPosition.Y > MaxY - net.Height && ((ballPosition.X + 2 * ballRadius) > netPosition.X && ballPosition.X<netPosition.X+net.Width)) // ball hit top of net
                {
                    if (ballVelocity.X < 0)
                    {
                        ballVelocity.X *= -1;
                        ballPosition.X = netPosition.X+net.Width+1;// teleport out of collision territory

                    }
                    else
                    {
                        ballVelocity.X *= -1;
                        ballPosition.X -= (((ballPosition.X + 30) - netPosition.X)); //teleport out of collision territory
                    }
                    
                }
                else
                {
                    if (ballPosition.Y > MaxY - ballRadius) // ball hit ground
                    {
                        ballPosition.Y = MaxY - ballRadius;
                        ballVelocity.Y *= -1;
                    }
                    else if (ballPosition.Y+30 > MaxY - net.Height && ((ballPosition.X + 2 * ballRadius) > netPosition.X && ballPosition.X < netPosition.X + net.Width))
                    {
                        ballPosition.Y = MaxY - net.Height - 30;
                        ballVelocity.Y *= -1;
                    }
                }
            }
        }

        void updateBall(float dt)
        {
            
            ballPosition.Y += dt * ballVelocity.Y;
            //checkForBallCollision(false);
            ballPosition.X += dt * ballVelocity.X;
            checkForBallCollision(true);
            playerRebound();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            

            // Draw the sprite

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(background, mainFrame, Color.White);
            
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(player1, spritePosition1, null, Color.White, 0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(p1Eye, p1EyePosition, null, Color.White, p1EyeRotation, new Vector2(p1Eye.Width/2, p1Eye.Height/2), 1, SpriteEffects.None, 0.05f);
            
            spriteBatch.Draw(player2, spritePosition2, null, Color.White, 0f, new Vector2(0, 0), 1, SpriteEffects.None, 0.1f);
            
            spriteBatch.Draw(net, netPosition, null, Color.White);
            spriteBatch.Draw(p2Eye, p2EyePosition, null, Color.White, p2EyeRotation, p2eyeOrigin, 1, SpriteEffects.None, 0.05f);
            spriteBatch.Draw(ball, ballPosition, null, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
