using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ass1
{
    //updated code
    public enum TankType
    {
        DEFAULT,
        ENEMY_TANK,
        FLEE_TANK
    }
    public enum RocketType
    {
        DEFAULT,
        PLAYER_ROCKET,
        ENEMY_ROCKET
    }
    public class BasicModel
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;
        protected Matrix translation = Matrix.Identity;

        public BasicModel (Model model1)
        {
            this.model = model1;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GraphicsDevice device, Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    //effect.World = mesh.ParentBone.Transform * Getworld()* GetTranslation(camera);
                    effect.World = transforms[mesh.ParentBone.Index] * Getworld() * GetTranslation(camera);
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.TextureEnabled = true;
                    effect.Alpha = 1;
                }
                mesh.Draw();
            }
        }

        public virtual Matrix Getworld()
        {
            return world;
        }
        protected virtual Matrix GetTranslation(Camera camera)
        {
            return translation;
        }

        public virtual Vector3 GetTankDirection()
        {
            return Vector3.Zero;
        }
        public virtual Vector3 GetTankPosition()
        {
            return Vector3.Zero;
        }

        public bool CollidesWith(Model otherModel, Matrix otherWorld)
        {
            // Loop through each ModelMesh in both objects and compare    
            // all bounding spheres for collisions    
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                foreach (ModelMesh hisModelMeshes in otherModel.Meshes)
                {
                    if (myModelMeshes.BoundingSphere.Transform(Getworld())
                        .Intersects(hisModelMeshes.BoundingSphere.Transform(otherWorld)))
                        return true;
                }
            }
            return false;
        }

        public virtual TankType tankType()
        {
            return TankType.DEFAULT;
        }
        public virtual RocketType getRocketType()
        {
            return RocketType.DEFAULT;
        }
        public virtual void setModelSpeed(float s)
        {

        }
        public virtual Matrix GetScale()
        {
            return Matrix.Identity;
        }
    }

}
