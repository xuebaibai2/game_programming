using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ass1
{
    class EnemyTank : BasicModel
    {
        private MousePick mousePick;
        private Vector3 position;
        private Vector3 targetPosition;
        private Matrix rotation = Matrix.Identity;
        private ModelBone turretBone;
        private ModelBone backWheelBone;

        //update 9/5
        private ModelBone leftBackWheelBone;
        private ModelBone rightBackWheelBone;

        private Vector3 orintation;
        private Vector3 currentVelocity;
        private Vector3 desiredVelocity;
        private Vector3 steeringForce;


        private double currentSpeed = 0;
        private float maxSpeed = 2f;
        private int mass = 10;

        //update 9/4
        private float minStopSpeed = 0.1f;

        float chaseMinDistance = 100;

        private double rotationSpeed = MathHelper.PiOver4/200;
        private double orintationAngle;
        private double tankAngle = MathHelper.PiOver2;
        private double turretAngle = MathHelper.PiOver2;
        private double acceleration = 0.002;

        private bool isMoving;

        private Tank targetTank;

        public EnemyTank(Model model, GraphicsDevice device, Camera camera, Vector3 position) : base(model)
        {
            mousePick = new MousePick(device, camera);
            //this.position = new Vector3(500, 0, 200);         //enemy tank's spawn position
            this.position = position;
            currentVelocity = Vector3.Normalize(new Vector3(0, 0, 1));  //enemy tank is facing vector3(0,0,1) when it spawn, this is initial velocity
            turretBone = model.Bones["turret_geo"];

            //update 9/5
            leftBackWheelBone = model.Bones["l_back_wheel_geo"];
            rightBackWheelBone = model.Bones["r_back_wheel_geo"];

            translation = Matrix.CreateTranslation(position);
        }

        public void AddTarget(Tank playerTank)
        {
            this.targetTank = playerTank;
        }
        public override void Update(GameTime gameTime)
        {
            int elapsedFrameTime = gameTime.ElapsedGameTime.Milliseconds;
            double turnedAngle = rotationSpeed * elapsedFrameTime;
            targetPosition = targetTank.position;
            orintation = targetPosition - position;
            desiredVelocity = Vector3.Normalize(orintation) * maxSpeed;
            orintationAngle = Math.Atan2(orintation.X, orintation.Z);

            //update 9/4
            if (currentVelocity.Length() > 1)
                currentVelocity.Normalize();

            currentVelocity *= (float)currentSpeed;

            //the enemy tank will chase player and keep 100 distance
            float distance = (targetPosition - position).Length();
            if (distance > chaseMinDistance)
            {
                isMoving = true;

                //update 9/3
                //tank will accelerate from 0 to max speed
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += acceleration * elapsedFrameTime;
                }
                else
                    currentSpeed = maxSpeed;

                //steering behavior of the enemy tank
                //update 9/5
                Steering(elapsedFrameTime);
                RotateTank(turnedAngle);
            }
            else
            {
                //smoothly slow down, acceleration here is also brake force
                if (Math.Abs(currentSpeed) < minStopSpeed)
                    currentSpeed = 0;
                else if (currentSpeed > 0)
                {
                    currentSpeed -= acceleration * elapsedFrameTime;
                }
                else
                    currentSpeed = 0;

                //steering behavior of the enemy tank
                //update 9/5
                Steering(elapsedFrameTime);
                RotateTank(turnedAngle);

                isMoving = false;
            }
            turretBone.Transform *= Matrix.CreateRotationY(MathHelper.PiOver4 / 20);
            //leftBackWheelBone.Transform *= Matrix.CreateRotationX(MathHelper.PiOver4 / 20);
            //rightBackWheelBone.Transform *= Matrix.CreateRotationX(MathHelper.PiOver4 / 20);

            base.Update(gameTime);
        }

        //update 9/5
        private void RotateTank(double turnedAngle)
        {
            //rotate the tank fram axis Y
            //prevent tank angle from increasing over pi
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
                //decide which direction the tank rotate, left or right
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
                //calculate steering force and use it for natural movement
                steeringForce = desiredVelocity - currentVelocity;
                steeringForce /= mass;

                if(steeringForce.Length() < 1)
                {
                    currentVelocity = desiredVelocity;
                }
                else
                {
                    currentVelocity += steeringForce * elapsedFrameTime;
                }
                
            }
            position += currentVelocity;
            translation = Matrix.CreateTranslation(position);
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }

        public override Matrix Getworld()
        {
            return Matrix.CreateScale(0.05f) * rotation;
        }
        
        public override Vector3 GetTankPosition()
        {
            return position;
        }
        public override Vector3 GetTankDirection()
        {
            //return orintation;
            Vector3 bulletOrintation = new Vector3(orintation.X, 0, orintation.Z);
            bulletOrintation.Normalize();
            return bulletOrintation / 10; //THis is miracle
        }
        public override TankType tankType()
        {
            return TankType.ENEMY_TANK;
        }
    }
}
