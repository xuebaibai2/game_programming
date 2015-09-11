using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ass1
{
    class Box : BasicModel
    {
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix scale = Matrix.CreateScale(3f);

        public Box(Model model, GraphicsDevice device, Camera camera)
            : base(model)
        {
            translation.Translation = new Vector3(1000, 0, 500);
        }

        //replaced by position
        public override Vector3 GetTankPosition()
        {
            return translation.Translation;
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }

        public override Matrix Getworld()
        {
            //return base.GetWorld();
            return scale * rotation * translation;
        }

        public override Matrix GetScale()
        {
            return scale;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
