using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ass1
{
    public class Rocket : BasicModel
    {
        public Vector3 bulletPosition;
        public Matrix translation = Matrix.Identity;
        public Matrix orientation = Matrix.Identity;

        public bool bulletShot = false;
        public Vector3 direction;
        public Vector3 velocity;
        private float newOrientation;
        private Vector3 lastPoint;
        private RocketType rocketType;

        public Rocket(Model model, Vector3 position, Vector3 direction, GraphicsDevice device, Camera camera, RocketType rocketType)
            : base(model)
        {
            this.direction = direction;
            this.rocketType = rocketType;
            
            newOrientation = (float)(Math.Atan2(direction.X, direction.Z));
            translation = Matrix.CreateTranslation(position);
            lastPoint = translation.Translation;
            translation = Matrix.CreateRotationY(newOrientation);
            translation.Translation = lastPoint;

        }
        public override void Update(GameTime gameTime)
        {
            if (direction == Vector3.Zero)
            {
                direction = new Vector3(0, 0, 1);
            }
            velocity = StaticValue.BULLET_SPEED * direction;
            translation.Translation += velocity * gameTime.ElapsedGameTime.Milliseconds;
            //Console.WriteLine("Translation: {0}", translation.Translation);
            base.Update(gameTime);
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }
        public override Matrix Getworld()
        {
            return Matrix.CreateScale(0.5f) * translation;
        }

        public override RocketType getRocketType()
        {
            return this.rocketType;
        }
    }
}
