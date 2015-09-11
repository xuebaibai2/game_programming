using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ass1
{
    class FleeTank :BasicModel
    {
        private MousePick mousePick;
        private Vector3 position;
        private Vector3 targetPosition;
        private Matrix rotation = Matrix.Identity;
        private ModelBone turretBone;
        private ModelBone backWheelBone;
        private Vector3 orintation;
        private Vector3 currentVelocity;
        private Vector3 desiredVelocity;
        private Vector3 steeringForce;


        private double currentSpeed = 0;
        private float maxSpeed = 3f;

        //update 9/5
        private float minStopSpeed = 0.1f;

        //update
        private int mass = 10;

        private float fleeDistance = 800;
        private float boundary = 1000f;

        private double rotationSpeed = MathHelper.PiOver4 / 200;
        private double orintationAngle;
        private double tankAngle = 0;//MathHelper.PiOver2;
        private double turretAngle = MathHelper.PiOver2;
        private double acceleration = 0.005;
        private float scaleRatio = 0.05f;

        private bool isMoving;

        private Tank targetTank;

        public FleeTank(Model model, GraphicsDevice device, Camera camera, Vector3 position, int tankID) : base(model)
        {
            mousePick = new MousePick(device, camera);
            npcTankID = tankID;
            //position = new Vector3(100, 0, -100);         //enemy tank's spawn position
            this.position = position;
            currentVelocity = Vector3.Normalize(new Vector3(0, 0, 1));  //enemy tank is facing vector3(0,0,1) when it spawn, this is initial velocity
            turretBone = model.Bones["turret_geo"];
            backWheelBone = model.Bones["r_back_wheel_geo"];
            translation = Matrix.CreateTranslation(position);
        }

        public void AddTarget(Tank playerTank)
        {
            targetTank = playerTank;
        }
        public override void Update(GameTime gameTime)
        {
            int elapsedFrameTime = gameTime.ElapsedGameTime.Milliseconds;
            double turnedAngle = rotationSpeed * elapsedFrameTime;

            targetPosition = targetTank.position;
            orintation = position - targetPosition;   //opposite direction of the target
            desiredVelocity = Vector3.Normalize(orintation) * maxSpeed;

            orintationAngle = Math.Atan2(orintation.X, orintation.Z);
            
            //update 9/5
            if(currentVelocity.Length()>1)
                currentVelocity.Normalize();

            currentVelocity *= (float)currentSpeed;
            float distance = (targetPosition - position).Length();

            //Flee tank will flee opposite way of player if player is in flee distance range
            if (distance < fleeDistance)
            {
                isMoving = true;

                //tank will accelerate from 0 to max speed
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += acceleration * elapsedFrameTime;
                }
                else
                    currentSpeed = maxSpeed;
                currentVelocity *= (float)currentSpeed;
                //steering behavior of the enemy tank
                Steering(elapsedFrameTime);

                //update 9/5
                RotateTank(turnedAngle);

            }
            else
            {
                //smoothly slow down, acceleration here is also brake force

                //update 9/5
                if (Math.Abs(currentSpeed) < minStopSpeed)
                    currentSpeed = 0;
                if (currentSpeed > 0)
                {
                    currentSpeed -= acceleration * elapsedFrameTime;
                }
                else
                    currentSpeed = 0;

                //update 9/5
                //steering behavior of the enemy tank
                Steering(elapsedFrameTime);
                RotateTank(turnedAngle);

                isMoving = false;
            }
            turretBone.Transform *= Matrix.CreateRotationY(MathHelper.PiOver4 / 20);

            base.Update(gameTime);
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

        //update 9/5
        private void RotateTank(double turnedAngle)
        {
            //rotate the tank fram axis Y
            if (tankAngle > MathHelper.Pi || tankAngle < -MathHelper.Pi)
            {
                tankAngle = orintationAngle;
            }
            double angleDifference = tankAngle - orintationAngle;
            if (Math.Abs(angleDifference) < MathHelper.PiOver4 / 10)
            {
                rotation = Matrix.CreateRotationY((float)orintationAngle);
            }
            else
            {
                if (tankAngle > 0)
                {
                    if (angleDifference > 0 && angleDifference < MathHelper.Pi)
                        tankAngle -= turnedAngle;
                    else
                        tankAngle += turnedAngle;
                }
                else
                {
                    if (angleDifference > -MathHelper.Pi && angleDifference < 0)
                        tankAngle += turnedAngle;
                    else
                        tankAngle -= turnedAngle;
                }
                rotation = Matrix.CreateRotationY((float)tankAngle);
            }
        }

        private void Steering(int elapsedFrameTime)
        {
            if (isMoving)
            {
                steeringForce = desiredVelocity - currentVelocity;
                steeringForce /= mass;

                if (steeringForce.Length() < 1)
                {
                    currentVelocity = desiredVelocity;
                }
                else
                {
                    currentVelocity += steeringForce * elapsedFrameTime;
                }
                  
            }
            
            //update 9/5
            position += currentVelocity;
            LimitInBoundary();
            translation = Matrix.CreateTranslation(position);
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }

        public override Matrix Getworld()
        {
            return Matrix.CreateScale(scaleRatio) * rotation; //* translation;
        }
        public override Vector3 GetTankPosition()
        {
            return position;
        }
        public override Vector3 GetTankDirection()
        {
            return orintation;
        }
        protected virtual Matrix GetTranslation(Camera camera)
        {
            return translation;
        }
        public override TankType tankType()
        {
            return TankType.FLEE_TANK;
        }
    }
}
