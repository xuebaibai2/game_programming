using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ass1
{
    public class Tank : BasicModel
    {
        private MousePick mousePick;
        private Camera tankCamera;
        public Vector3 position;
        private Matrix rotation = Matrix.Identity;
        private ModelBone turretBone;
        private ModelBone backWheelBone;
        private Vector3 currentVelocity;
        private Vector3 destinationPoint;
        private Vector3 tankDirection;

        private Matrix scale = Matrix.Identity;
        //private Matrix translation = Matrix.Identity;

        private MouseState prevMouseState;
        private double yawSpeed = -MathHelper.PiOver4 / 1000;
        private double turnedAngleTest;

        //update 9/4
        private double rotationSpeed = 0.005;
        private float currentMoveSpeed = 0;
        private float maxMoveSpeed = 0.5f;
        private float tankAcceleration = 0.01f;
        private float minStopSpeed = 0.1f;

        private double orintationAngle;
        private double tankAngle = 0;
        private double turretAngle = MathHelper.PiOver2;

        //update 9/3 15:21
        private Vector3 currentCameraDirection;
        private int cameraDistance = 200;
        private float currentPitch = 0;
        private float maxPitch = MathHelper.PiOver4/2;
        private float pitchSpeed = MathHelper.PiOver4 / 500;
        private float scaleRatio = 0.05f;

        //update 9/4 00:21
        private ModelBone cannonBone;
        private ModelBone lSteerEngineBone;
        private ModelBone rSteerEngineBone;
        private ModelBone leftBackWheelBone;
        private ModelBone rightBackWheelBone;
        private ModelBone leftFrontWheelBone;
        private ModelBone rightFrontWheelBone;
        private ModelBone hatchBone;
        private ModelBone tankBone;
        private float wheelRotationSpeed = 5f;
        Matrix rightBackWheelTransform;
        Matrix leftBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        Matrix hatchTransform;
        Matrix tankTransform;
        public float SteerRotationValue;

        //steven 9-11
        private const float boundary = 1000f;
        private float positiondiffx = 0;
        private float positiondiffz = 0;
        Vector3? pickPositionCur;


        public Tank(Model model, GraphicsDevice device, Camera camera) : base(model)
        {
            mousePick = new MousePick(device, camera);
            tankCamera = camera;
            position = Vector3.Zero;
            pickPositionCur = Vector3.Zero;
            tankDirection = new Vector3(0, 0, 1);
            tankDirection.Normalize();

            //update 9/3
            currentCameraDirection = tankCamera.cameraDirection;

            pickPositionCur = Vector3.Zero;
            turretBone = model.Bones["turret_geo"];

            //update 9/4 00:22
            cannonBone = model.Bones["canon_geo"];
            lSteerEngineBone = model.Bones["l_steer_geo"];
            rSteerEngineBone = model.Bones["r_steer_geo"];
            leftBackWheelBone = model.Bones["l_back_wheel_geo"];
            rightBackWheelBone = model.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = model.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = model.Bones["r_front_wheel_geo"];
            hatchBone = model.Bones["hatch_geo"];
            tankBone = model.Bones["tank_geo"];

            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            hatchTransform = hatchBone.Transform;
            tankTransform = tankBone.Transform;

            prevMouseState = Mouse.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            //leftFrontWheelBone.Transform *= Matrix.CreateRotationX(MathHelper.PiOver4 / 20);
            //rightFrontWheelBone.Transform *= Matrix.CreateRotationX(MathHelper.PiOver4 / 20);
            //leftBackWheelBone.Transform *= Matrix.CreateFromAxisAngle(new Vector3(1,0,0),MathHelper.PiOver4);
            //rightBackWheelBone.Transform *= Matrix.CreateRotationX(MathHelper.PiOver4 / 20);
            //update 9/4
            //leftBackWheelBone.Transform = Matrix.CreateRotationX(wheelRotationSpeed) * leftBackWheelTransform;
            //rightBackWheelBone.Transform = Matrix.CreateRotationX(wheelRotationSpeed) * rightBackWheelTransform;
            //hatchBone.Transform = Matrix.CreateRotationY(SteerRotationValue) * hatchTransform;
            //tankBone.Transform = Matrix.CreateRotationY(SteerRotationValue) * tankTransform;

            //create yaw
            turnedAngleTest = (Mouse.GetState().X - prevMouseState.X) * yawSpeed * gameTime.ElapsedGameTime.Milliseconds;
            tankAngle += turnedAngleTest;
            rotation = Matrix.CreateRotationY((float)tankAngle);
            tankDirection = Vector3.Transform(tankDirection, Matrix.CreateFromAxisAngle(Vector3.Up, (float)turnedAngleTest));

            currentVelocity = tankDirection * currentMoveSpeed;

            //update 9/4
            GetUserInput(gameTime);

            //update 9/3 15:07
            UpdateCamera();

            Vector3? pickPosition = mousePick.GetCollisionPosition();

            //Steven
            //Vector3 pickPosition;
            pickPositionCur = mousePick.GetCollisionPosition();
            float px = 0f, py = 0f, pz = 0f;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && pickPositionCur.HasValue == true)
            {

                px = pickPositionCur.Value.X;
                py = pickPositionCur.Value.Y;
                pz = pickPositionCur.Value.Z;
                if (pickPositionCur.Value.X > boundary)
                {
                    px = boundary;
                }

                if (pickPositionCur.Value.X < -boundary)
                {
                    px = -boundary;
                }

                if (pickPositionCur.Value.Z > boundary)
                {
                    pz = boundary;
                }

                if (pickPositionCur.Value.Z < -boundary)
                {
                    pz = -boundary;
                }
            }
            pickPosition = new Vector3(px, py, pz);

            double turnedAngle = rotationSpeed * gameTime.ElapsedGameTime.Milliseconds;

            LimitInBoundary();
            translation = Matrix.CreateTranslation(position);
            prevMouseState = Mouse.GetState();

            //turretBone.Transform *= Matrix.CreateRotationY(MathHelper.PiOver4 / 20);
            //     rightFrontWheelBone.ModelTransform *= Matrix.CreateRotationX(MathHelper.PiOver4 / 500);

            //   leftBackWheelBone.Transform *= Matrix.CreateRotationZ(MathHelper.PiOver4 / 500);
            //leftBackWheelBone.Transform = Matrix.CreateRotationX(MathHelper.PiOver4 / 20) * leftBackWheelTransform;
            //rightBackWheelBone.Transform = Matrix.CreateRotationX(MathHelper.PiOver4 / 20) * rightBackWheelTransform;
            //leftFrontWheelBone.Transform = Matrix.CreateRotationX(MathHelper.PiOver4 / 20) * leftFrontWheelTransform;
            //rightFrontWheelBone.Transform = Matrix.CreateRotationX(MathHelper.PiOver4 / 20) * rightFrontWheelTransform;

            turretBone.Transform *= Matrix.CreateRotationY(MathHelper.PiOver4 / 50);
            base.Update(gameTime);
        }

        //update 9/5
        private void GetUserInput(GameTime gameTime)
        {
            //update 9/5
            //forward accelerate
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (currentMoveSpeed < maxMoveSpeed)
                    currentMoveSpeed += tankAcceleration;
                else
                    currentMoveSpeed = maxMoveSpeed;
                currentVelocity = tankDirection * currentMoveSpeed;
                position += currentVelocity * gameTime.ElapsedGameTime.Milliseconds;
            }
            //backward accelerate
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (currentMoveSpeed < -maxMoveSpeed)
                    currentMoveSpeed = -maxMoveSpeed;
                else
                    currentMoveSpeed -= tankAcceleration;
                currentVelocity = tankDirection * currentMoveSpeed;
                position += currentVelocity * gameTime.ElapsedGameTime.Milliseconds;
            }
            //else if (Keyboard.GetState().IsKeyDown(Keys.A))
            //{
            //    if (currentMoveSpeed < maxMoveSpeed)
            //        currentMoveSpeed += tankAcceleration;
            //    else
            //        currentMoveSpeed = maxMoveSpeed;
            //    currentVelocity = Vector3.Cross(Vector3.Up, tankDirection) * currentMoveSpeed;
            //    position += currentVelocity * gameTime.ElapsedGameTime.Milliseconds;
            //}
            //else if (Keyboard.GetState().IsKeyDown(Keys.D))
            //{
            //    if (currentMoveSpeed < -maxMoveSpeed)
            //        currentMoveSpeed = -maxMoveSpeed;
            //    else
            //        currentMoveSpeed -= tankAcceleration;
            //    currentVelocity = Vector3.Cross(Vector3.Up, tankDirection) * currentMoveSpeed;
            //    position -= currentVelocity * gameTime.ElapsedGameTime.Milliseconds;
            //}
            
            //naturally slow down without accelerate and brake
            else
                NaturallySlowDown(gameTime.ElapsedGameTime.Milliseconds);
        }

        //update 9/4
        private void NaturallySlowDown(int elapsedFrameTime)
        {
            if (Math.Abs(currentMoveSpeed) < minStopSpeed)
            {
                currentMoveSpeed = 0;
                currentVelocity = Vector3.Zero;
            }
            else
            {
                // no matter currentMoveSpeed is positive or negative
                // -currentMoveSpeed/Math.Abs(currentMoveSpeed) return opposite 1 of it
                currentMoveSpeed -= currentMoveSpeed/Math.Abs(currentMoveSpeed) * tankAcceleration / 2;
                currentVelocity = tankDirection * currentMoveSpeed;
            }
            position += currentVelocity * elapsedFrameTime;
        }

        //update 9/3 15:03
        private void UpdateCamera()
        {
            Vector3 newCameraDirection = Vector3.Transform(currentCameraDirection,
                Matrix.CreateFromAxisAngle(Vector3.Up, (float)turnedAngleTest));
            float pitchAngle = pitchSpeed * (Mouse.GetState().Y - prevMouseState.Y);
            // limit the pitchangle
            if (Math.Abs(currentPitch + pitchAngle) < maxPitch)
            {
                newCameraDirection = Vector3.Transform(newCameraDirection, Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, newCameraDirection), pitchAngle));
                // record each pitch angle
                currentPitch += pitchAngle;
            }
            Vector3 newCameraPosition = position - newCameraDirection * cameraDistance;
            tankCamera.cameraDirection = newCameraDirection;
            tankCamera.cameraPosition = newCameraPosition;
            currentCameraDirection = newCameraDirection;
        }

        private void LimitInBoundary()
        {
            float minBoundary = boundary - 500 * scaleRatio;
            if (position.X > minBoundary)
                position.X = minBoundary;
            if (position.X < -minBoundary)
                position.X = -minBoundary;
            if (position.Z > minBoundary)
                position.Z = minBoundary;
            if (position.Z < -minBoundary)
                position.Z = -minBoundary;
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }

        public override Matrix Getworld()
        {
            return Matrix.CreateScale(scaleRatio) * rotation;
        }

        public override Vector3 GetTankDirection()
        {
            return tankDirection;
            //return velocity;
        }
        public override Vector3 GetTankPosition()
        {
            return translation.Translation;
        }

        //steven
        public override void setModelSpeed(float s)
        {
            //base.setModelSpeed(s);
            currentVelocity.X = (int)s;
            currentVelocity.Y = (int)s;
            currentVelocity.Z = (int)s;
        }

        public void rebounded(float x, float z)
        {
            positiondiffx = x;
            positiondiffz = z;
        }

        public float getBoundary()
        {
            return boundary;
        }

    }



}