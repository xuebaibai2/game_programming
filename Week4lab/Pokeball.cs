using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Ass1
{
    class Pokeball : BasicModel
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;
        protected Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        MousePick mousePick;
        protected Vector3 initPosition;
        public Pokeball(Model model1, GraphicsDevice device, Camera camera) : base(model1)
        {
            this.model = model1;
            mousePick = new MousePick(device, camera);
            initPosition = new Vector3(0, 0, 0);
            translation = Matrix.CreateTranslation(initPosition);
        }

        public override void Update(GameTime gameTime)
        {
            Vector3? pickPosition = mousePick.GetCollisionTest();
            if (pickPosition.HasValue)
            {
                translation = Matrix.CreateTranslation(pickPosition.Value);
            }
            base.Update(gameTime);
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }
        public override Matrix Getworld()
        {
            return Matrix.CreateScale(0.2f) * rotation * translation;
        }
    }
}
