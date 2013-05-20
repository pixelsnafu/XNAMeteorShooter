using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VRProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        KeyboardState lastState = Keyboard.GetState();
        SpriteBatch spriteBatch;
        //Model ship;
        float aspectRatio;
        Vector3 shipPosition = Vector3.Zero;

        Vector3 cameraPosition = new Vector3(0.0f, 0.0f, GameConstants.CameraHeight);
        Vector3 shipVelocity = Vector3.Zero;

        SoundEffect soundEngine;
        SoundEffectInstance soundEngineInstance;
        SoundEffect soundHyperspaceActivation;
        SoundEffect soundExplosion2;
        SoundEffect soundExplosion3;
        SoundEffect soundWeaponsFire;

        Matrix projectionMatrix;
        Matrix viewMatrix;

        Ship ship = new Ship();

        Model asteroidModel;
        Matrix[] asteroidTransforms;
        Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];
        Random random = new Random();

        Model bulletModel;
        Matrix[] bulletTransforms;
        Bullet[] bulletList = new Bullet[GameConstants.NumBullets];

        Texture2D stars;
        Texture2D explosion;

        //explosion variables
        float timer = 0;
        float interval = 40.0f;
        int row = 5, column = 5;
        int height, width, frameHeight, frameWidth;
        bool isExplosion;
        float explosionScale = (float)1.5;

        SpriteFont kootenay;
        int score;
        Vector2 scorePosition = new Vector2(100, 50);


        //faceshift variables
        Server server;
        float quatx = 0.0f;
        float quaty = 0.0f;
        float quatz = 0.0f;
        float quatw = 0.0f;

        float[] coeffs;
        bool isFired = false;
        bool isReset = false;
        float positionX = 0, positionY = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1366;
            //graphics.IsFullScreen = true;
            //server that listens for faceshift data
            server = new Server(33433, ReceiveFaceShift);
            server.Start();

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
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90),
                GraphicsDevice.DisplayMode.AspectRatio,
                GameConstants.CameraHeight - 2000.0f, GameConstants.CameraHeight + 2000.0f);

            viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            ResetAsteroids();
            base.Initialize();
        }

        private Matrix[] SetupEffectDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effect.DirectionalLight0.Direction = new Vector3(1, 1, 1);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);
                    effect.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
                    //effect.EmissiveColor = new Vector3(1, 1, 1);
                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                }
            }
            return absoluteTransforms;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ship.model = Content.Load<Model>("Models\\p1_wedge");
            ship.transforms = SetupEffectDefaults(ship.model);
            asteroidModel = Content.Load<Model>("Models\\asteroid2");
            asteroidTransforms = SetupEffectDefaults(asteroidModel);
            bulletModel = Content.Load<Model>("Models\\pea_proj");
            bulletTransforms = SetupEffectDefaults(bulletModel);
            soundEngine = Content.Load<SoundEffect>("Audio\\Waves\\engine_3");
            stars = Content.Load<Texture2D>("Textures\\B1_stars");
            explosion = Content.Load<Texture2D>("Sprites\\explosion");
            height = explosion.Height;
            width = explosion.Width;
            frameHeight = height / 5;
            frameWidth = width / 5;
            kootenay = Content.Load<SpriteFont>("Fonts\\Lucida Console");
            soundEngineInstance = soundEngine.CreateInstance();
            soundHyperspaceActivation = Content.Load<SoundEffect>("Audio\\Waves\\hyperspace_activate");
            soundExplosion2 = Content.Load<SoundEffect>("Audio/Waves/explosion2");
            soundExplosion3 = Content.Load<SoundEffect>("Audio/Waves/explosion3");
            soundWeaponsFire = Content.Load<SoundEffect>("Audio/Waves/tx0_fire1");

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            // TODO: use this.Content to load your game content here
        }

        private void ResetAsteroids()
        {
            float xStart, yStart;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                asteroidList[i].isAlive = true;
                if (random.Next(2) == 0)
                {
                    xStart = (float)(random.Next((int)GameConstants.PlayfieldSizeX) - (float)GameConstants.PlayfieldSizeX);
                }
                else
                {
                    xStart = (float)random.Next((int)GameConstants.PlayfieldSizeX);//(float)GameConstants.PlayfieldSizeX;
                }
                yStart = (float)random.NextDouble() * 2 * GameConstants.PlayfieldSizeY;
                asteroidList[i].position = new Vector3(xStart, yStart, 0.0f);
                double angle = random.NextDouble() * 2 * Math.PI;
                asteroidList[i].direction.X = -(float)Math.Sin(angle);
                asteroidList[i].direction.Y = -(float)Math.Cos(angle);
                asteroidList[i].speed = GameConstants.AsteroidMinSpeed +
                    (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed;
                asteroidList[i].rotation = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            }
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
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();



            // TODO: Add your update logic here
            /*shipRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds *
                MathHelper.ToRadians(0.1f);*/


            updateInput();

            ship.worldPosition += ship.shipVelocity;
            ship.shipVelocity *= 0.97f;

            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                asteroidList[i].Update(timeDelta);
            }

            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                if (bulletList[i].isAlive)
                {
                    bulletList[i].Update(timeDelta);
                }
            }

            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (asteroidList[i].isAlive)
                {
                    BoundingSphere b = new BoundingSphere(
                                    asteroidList[i].position,
                                    asteroidModel.Meshes[0].BoundingSphere.Radius *
                                    GameConstants.AsteroidBoundingSphereScale);
                    for (int j = 0; j < GameConstants.NumAsteroids; j++)
                    {
                        if (asteroidList[j].isAlive)
                        {
                            BoundingSphere aSphere = new BoundingSphere(
                                                    asteroidList[j].position,
                                                    asteroidModel.Meshes[0].BoundingSphere.Radius *
                                                    GameConstants.AsteroidBoundingSphereScale);
                            //asteroid to asteroid collision logic
                            //if (!asteroidList[i].Equals(asteroidList[j]) && b.Intersects(aSphere))
                            //{
                            //    float deltaRadius = b.Radius + aSphere.Radius;

                            //}

                        }
                    }
                }
            }

            //Console.WriteLine(asteroidList[0].speed);
            if (ship.isAlive)
            {
                BoundingSphere shipSphere = new BoundingSphere(
                        ship.worldPosition,
                        ship.model.Meshes[0].BoundingSphere.Radius *
                        GameConstants.ShipBoundingSphereScale);

                isReset = true;

                for (int i = 0; i < asteroidList.Length; i++)
                {
                    if (asteroidList[i].isAlive)
                    {
                        BoundingSphere b = new BoundingSphere(
                                        asteroidList[i].position,
                                        asteroidModel.Meshes[0].BoundingSphere.Radius *
                                        50 * GameConstants.AsteroidBoundingSphereScale);

                        for (int j = 0; j < GameConstants.NumBullets; j++)
                        {
                            if (bulletList[j].isAlive)
                            {
                                BoundingSphere bulletSphere = new BoundingSphere(bulletList[j].position,
                                    bulletModel.Meshes[0].BoundingSphere.Radius);
                                if (b.Intersects(bulletSphere))
                                {
                                    soundExplosion2.Play();

                                    //calculate 2d explosion coordinates
                                    Vector4 a = new Vector4(bulletList[j].position, 1);
                                    a = Vector4.Transform(bulletList[j].position, viewMatrix);
                                    a = Vector4.Transform(a, projectionMatrix);
                                    positionX = a.X / a.W * 1366 / 2 +1366 / 2;
                                    positionY = -a.Y / a.W * 768 / 2 +768 / 2;
                                    isExplosion = true;
                                    row = 0;
                                    column = 0;
                                    explosionScale = 1.5f;

                                    asteroidList[i].isAlive = false;
                                    bulletList[j].isAlive = false;
                                    score += GameConstants.KillBonus;
                                    
                                    break;
                                }
                            }
                        }

                        if (b.Intersects(shipSphere))
                        {
                            soundEngineInstance.Pause();
                            soundExplosion3.Play();

                            //explosion calculation
                            Vector4 a = new Vector4(asteroidList[i].position, 1);
                            a = Vector4.Transform(asteroidList[i].position, viewMatrix);
                            a = Vector4.Transform(a, projectionMatrix);
                            positionX = a.X / a.W * 1366 / 2 + 1366 / 2;
                            positionY = -a.Y / a.W * 768 / 2 + 768 / 2;
                            isExplosion = true;
                            row = 0;
                            column = 0;
                            explosionScale = 2.5f;

                            ship.isAlive = false;
                            isReset = false;
                            asteroidList[i].isAlive = false;
                            score -= GameConstants.DeathPenalty;
                            break;
                        }
                    }
                }
            }
            int count = 0;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (asteroidList[i].isAlive)
                    break;
                else
                    count++;
            }

            if (count == GameConstants.NumAsteroids)
                ResetAsteroids();

            if (isExplosion)
            {
                explodeUpdate(gameTime); 
            }

            base.Update(gameTime);
        }

        private void explodeUpdate(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                timer = 0;
                if (row < 5)
                {
                    if (column < 5)
                    {
                        column++;
                    }
                    else
                    {
                        row++;
                        column = 0;
                    }
                }
                else
                {
                    isExplosion = false;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
            spriteBatch.Draw(stars, new Rectangle(0, 0, 1366, 768), Color.White);

            spriteBatch.Draw(explosion, new Vector2((int)positionX, (int)positionY),
                    new Rectangle(column * frameHeight, row * frameWidth, frameHeight, frameWidth),
                    Color.White, 0, new Vector2((int)frameHeight/2, (int)frameWidth/2), explosionScale, SpriteEffects.None, 0);

            spriteBatch.DrawString(kootenay, "Score : " + score, scorePosition, Color.LightBlue);
            spriteBatch.End();
            if (ship.isAlive)
            {
                Matrix shipTransformMatrix = ship.rotationMatrix *
                        Matrix.CreateTranslation(ship.worldPosition);

                drawModel(ship.model, shipTransformMatrix, ship.transforms);
            }

            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (asteroidList[i].isAlive)
                {
                    Matrix asteroidTransform = asteroidTransform = Matrix.CreateScale(20.0f) *
                                Matrix.CreateRotationZ(asteroidList[i].rotation.Z) * Matrix.CreateRotationY(asteroidList[i].rotation.Y) *
                                Matrix.CreateTranslation(asteroidList[i].position);
                    drawModel(asteroidModel, asteroidTransform, asteroidTransforms);
                }
            }

            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                if (bulletList[i].isAlive)
                {
                    Matrix bulletTransform = Matrix.CreateTranslation(bulletList[i].position);
                    drawModel(bulletModel, bulletTransform, bulletTransforms);
                }
            }

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public static void drawModel(Model model, Matrix modelTransform,
            Matrix[] absoluteBoneTransforms)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                }

                mesh.Draw();
            }
        }

        protected void updateInput()
        {
            KeyboardState currentKeyState = Keyboard.GetState();
            ship.update(currentKeyState, quatx, quaty);
            //Vector3 shipVelocityAdd = Vector3.Zero;
            //if (currentKeyState.IsKeyDown(Keys.A))
            //    shipRotation += 0.10f;
            //else if (currentKeyState.IsKeyDown(Keys.D))
            //    shipRotation -= 0.10f;

            //shipVelocityAdd.X = -(float)Math.Sin(shipRotation);
            //shipVelocityAdd.Z = -(float)Math.Cos(shipRotation);

            //if (currentKeyState.IsKeyDown(Keys.W))
            //{
            //    shipVelocityAdd *= 5;
            //}


            //shipVelocity += shipVelocityAdd;
            if (currentKeyState.IsKeyDown(Keys.W) || quatx < -0.15 && ship.isAlive)
            {
                if (soundEngineInstance.State == SoundState.Stopped)
                {
                    soundEngineInstance.Volume = 0.75f;
                    soundEngineInstance.IsLooped = true;
                    soundEngineInstance.Play();
                }
                else
                {
                    soundEngineInstance.Resume();
                }
            }
            else
            {
                soundEngineInstance.Pause();
            }

            if (ship.isAlive && (currentKeyState.IsKeyDown(Keys.Space) && lastState.IsKeyUp(Keys.Space)) || coeffs[45] > 0.55)
            {
                for (int i = 0; i < GameConstants.NumBullets; i++)
                {
                    if (!bulletList[i].isAlive && isFired == false)
                    {
                        bulletList[i].direction = ship.rotationMatrix.Forward;
                        bulletList[i].speed = GameConstants.BulletSpeedAdjustment;
                        bulletList[i].position = ship.worldPosition + (200 * bulletList[i].direction);
                        bulletList[i].isAlive = true;
                        soundWeaponsFire.Play();
                        isFired = true;
                        break;
                    }
                }
            }
            else
            {
                isFired = false;
            }


            if (currentKeyState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter) || quatx > 0.20)
            {

                if (!isReset)
                {
                    isReset = true;
                    ship.worldPosition = Vector3.Zero;
                    ship.shipVelocity = Vector3.Zero;
                    ship.Rotation = 0.0f;
                    ship.isAlive = true;
                    score -= GameConstants.WarpPenalty;
                    soundHyperspaceActivation.Play();
                }
            }

            lastState = currentKeyState;
        }

        /**
         * Reads data from a packet sent from FaceShift
         */
        public void ReceiveFaceShift(byte[] data)
        {
            try
            {
                Packet received = new Packet(data);
                uint blockID = received.NextUInt16();
                uint versionNumber = received.NextUInt16();
                uint blockSize = received.NextUInt32();

                if (blockID == 33433)
                {
                    uint numBlock = received.NextUInt16();

                    for (int i = 0; i < numBlock; i++)
                    {
                        uint newBlockId = received.NextUInt16();
                        uint newVersion = received.NextUInt16();
                        uint newBlockSize = received.NextUInt32();

                        if (newBlockId == 101)
                        {
                            double timeStamp = received.NextDouble();
                            bool flag = received.NextBool();

                            if (flag)
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("Tracking at ");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Not Tracking at ");
                            }
                        }
                        else if (newBlockId == 102)
                        {
                            float xRot = received.NextFloat();
                            float yRot = received.NextFloat();
                            float zRot = received.NextFloat();
                            float wRot = received.NextFloat();
                            float xPose = received.NextFloat();
                            float yPose = received.NextFloat();
                            float zPose = received.NextFloat();

                            quatx = xRot;

                            quaty = yRot;
                            quatz = zRot;
                            quatw = wRot;
                            Console.WriteLine(quatx);
                        }
                        else if (newBlockId == 103)
                        {
                            uint numCoeffs = received.NextUInt32();
                            coeffs = new float[numCoeffs];
                            for (int j = 0; j < numCoeffs; j++)
                            {
                                coeffs[j] = received.NextFloat();
                            }
                            //Console.WriteLine(coeffs[41]);
                            //leftblink = coeffs[0];
                            //Console.WriteLine(coeffs[0]);
                        }
                        else if (newBlockId == 104)
                        {
                            float lefttheta = received.NextFloat();
                            float leftphi = received.NextFloat();
                            float righttheta = received.NextFloat();
                            float rightphi = received.NextFloat();

                            //Console.WriteLine(leftphi);
                        }
                        else
                        {
                            received.Skip(System.Convert.ToInt32(newBlockSize));
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
            }
        }
    }


}
