using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VRProject
{
    class Ship
    {
        public Model model;
        public Matrix[] transforms;

        //position of model in the world space
        public Vector3 worldPosition = Vector3.Zero;

        //velocity of the ship model, updated in each frame during update
        public Vector3 shipVelocity = Vector3.Zero;

        public bool isAlive = true;

        public Matrix rotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);

        private float rotation;
        public float Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                float newValue = value;
                if (newValue >= MathHelper.TwoPi)
                {
                    newValue -= MathHelper.TwoPi;
                }

                while (newValue < 0)
                {
                    newValue += MathHelper.TwoPi;
                }

                if (rotation != newValue)
                {
                    rotation = newValue;
                    rotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationZ(rotation);
                }
            }
        }

        public void update(KeyboardState currentKeyState, float fwdSpeed, float rotSpeed)
        {
            if (currentKeyState.IsKeyDown(Keys.A) || rotSpeed > 0.12)
                Rotation += 0.05f;
            else if (currentKeyState.IsKeyDown(Keys.D) || rotSpeed < -0.12)
                Rotation -= 0.05f;
            //Console.WriteLine(speed);
            if (currentKeyState.IsKeyDown(Keys.W) || fwdSpeed < -0.11f)
            {
                
                shipVelocity += rotationMatrix.Forward * 5.0f;
                if (worldPosition.X >= 27000)
                    worldPosition.X = -27000;

                else if (worldPosition.Y>= 16000)
                    worldPosition.Y = -16000;

                else if (worldPosition.X <= -27000)
                    worldPosition.X = 27000;

                else if (worldPosition.Y <= -16000)
                    worldPosition.Y = 16000;
            }
        }
    }
}
