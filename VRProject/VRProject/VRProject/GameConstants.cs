using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRProject
{
    class GameConstants
    {
        //camera constants
        public const float CameraHeight = 15000.0f;
        public const float PlayfieldSizeX = 27000;
        public const float PlayfieldSizeY = 16000;
        //asteroid constants
        public const int NumAsteroids = 15;

        public const float AsteroidMinSpeed = 100.0f;
        public const float AsteroidMaxSpeed = 300.0f;
        public const float AsteroidSpeedAdjustment = 5.0f;

        public const float AsteroidBoundingSphereScale = 0.95f;  //95% size
        public const float ShipBoundingSphereScale = 0.5f;  //50% size

        public const int NumBullets = 30;
        public const float BulletSpeedAdjustment = 200.0f;

        public const int DeathPenalty = 100;
        public const int WarpPenalty = 50;
        public const int KillBonus = 25;
    }
}