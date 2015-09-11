using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ass1
{
    class SkyBox : BasicModel
    {
        public SkyBox(Model model)
            : base(model)
        {

        }
        public override void Update(GameTime gameTime)
        {
            //move the skybox with camera

            base.Update(gameTime);
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            device.SamplerStates[0] = SamplerState.LinearClamp;
            base.Draw(device, camera);
        }

        public override Matrix Getworld()
        {
            return Matrix.CreateScale(2000f);
        }
        //protected override Matrix GetTranslation(Camera camera)
        //{
        //    float positionX = camera.cameraPosition.X;
        //    float positionZ = camera.cameraPosition.Z;
        //    return Matrix.CreateTranslation(positionX, 0, positionZ);
        //}
    }
}
