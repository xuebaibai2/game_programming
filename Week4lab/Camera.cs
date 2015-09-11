using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Ass1
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        public Vector3 cameraPosition { get; set; }
        public Vector3 cameraDirection;
        Vector3 cameraUp;
        float moveSpeed = 3f;
        MouseState prevMouseState;
        bool jumpingUp;
        bool jumpingDown;
        float jumpPeak = 100f;
        float jumpSpeed = 5f;
        Vector3 oringinPosition;
        float totalPitch = MathHelper.PiOver2;
        float currentPitch = 0;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // Build camera view matrix 
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height,
                1, 4000);
            cameraPosition = pos;
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }

        public override void Initialize()
        { 
            Mouse.SetPosition(Game.Window.ClientBounds.Width /2, Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //// Move forward/backward
            //if (Keyboard.GetState().IsKeyDown(Keys.W))
            //    cameraPosition += new Vector3(cameraDirection.X, 0, cameraDirection.Z) * moveSpeed;
            //if (Keyboard.GetState().IsKeyDown(Keys.S))
            //    cameraPosition -= new Vector3(cameraDirection.X, 0, cameraDirection.Z) * moveSpeed;
            // Move side to side 
            //if (Keyboard.GetState().IsKeyDown(Keys.A))
            //    cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * moveSpeed;
            //if (Keyboard.GetState().IsKeyDown(Keys.D))
            //    cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * moveSpeed;
            // camera jumping
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (!jumpingUp && !jumpingDown)  //do not allow multiple jumps within one jump process
                {
                    jumpingUp = true;
                    oringinPosition = cameraPosition;
                }
            }

            //camera yaw
            //cameraDirection = Vector3.Transform(cameraDirection,    
            //    Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 / 50) * (Mouse.GetState().X - prevMouseState.X)));
            //camera pitch
            //float pitchAngle = (MathHelper.PiOver4 / 50) * (Mouse.GetState().Y - prevMouseState.Y);
            //// limit the pitchangle
            //if (Math.Abs(currentPitch + pitchAngle) < totalPitch)
            //{
            //    cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), pitchAngle));
            //    // record each pitch angle
            //    currentPitch += pitchAngle;
            //}
            //Reset prevMouseState 
            prevMouseState = Mouse.GetState();
            
            //camera jump
            if (jumpingUp)
            {
                if(cameraPosition.Y < jumpPeak)
                {
                    cameraPosition += new Vector3(0, jumpSpeed, 0);
                }
                else
                {
                    jumpingUp = false;
                    jumpingDown = true;
                }
            }
            if (jumpingDown)
            {
                if(cameraPosition.Y > oringinPosition.Y)
                {
                    cameraPosition -= new Vector3(0, jumpSpeed, 0);
                }
                else
                {
                    cameraPosition = oringinPosition;
                    jumpingDown = false;
                }
            }

            // Recreate the camera view matrix 
            CreateLookAt(); 

            base.Update(gameTime);
        }
    }
}
