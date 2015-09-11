using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ass1
{
    class boxtexture : BasicModel
    {
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix scale = Matrix.CreateScale(5f);

        public boxtexture(Model model, GraphicsDevice device, Camera camera)
            : base(model)
        {
            translation.Translation = new Vector3(-500, 0, -500);
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }

        public override Matrix Getworld()
        {
            return scale * rotation * translation;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        //replaced by position
        public override Vector3 GetTankPosition()
        {
            return translation.Translation;
        }


        public override Matrix GetScale()
        {
            return Matrix.Identity;
        }
    }
}
