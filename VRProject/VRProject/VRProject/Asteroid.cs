using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VRProject
{
    struct Asteroid
    {
        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public bool isAlive;
        public Vector3 rotation;

        public void Update(float delta)
        {
            position +=  direction * speed * GameConstants.AsteroidSpeedAdjustment * delta;

            position += direction * speed * GameConstants.AsteroidSpeedAdjustment * delta;
            rotation.X += 0.01f;
            rotation.Y += 0.01f;
            rotation.Z += 0.01f;

            if (position.X > GameConstants.PlayfieldSizeX)
                position.X -= 2 * GameConstants.PlayfieldSizeX;
            if (position.X < -GameConstants.PlayfieldSizeX)
                position.X += 2 * GameConstants.PlayfieldSizeX;
            if (position.Y > GameConstants.PlayfieldSizeY)
                position.Y -= 2 * GameConstants.PlayfieldSizeY;
            if (position.Y < -GameConstants.PlayfieldSizeY)
                position.Y += 2 * GameConstants.PlayfieldSizeY;
        }
    }
}
