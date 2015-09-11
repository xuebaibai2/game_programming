using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ass1
{
    class Cube : BasicModel
    {

        Matrix translation = Matrix.Identity;
        public Cube(Model model, Vector3 position)
            : base(model)
        {
            translation = Matrix.CreateTranslation(position);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }
        public override Matrix Getworld()
        {
            return Matrix.CreateScale(1) * translation;
        }
        public override Vector3 GetTankPosition()
        {
            return translation.Translation;
        }
    }
}
