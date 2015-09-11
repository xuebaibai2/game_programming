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

        private ModelBone leftBackWheelBone;
        private ModelBone rightBackWheelBone;

        private Vector3 orintation;
        private Vector3 currentVelocity;
        private Vector3 desiredVelocity;
        private Vector3 steeringForce;

        private double currentSpeed = 0;
        private float maxSpeed = 2f;
        private int mass = 10;

        private float minStopSpeed = 0.1f;

        float chaseMinDistance = 200;
        float chaseMaxDistance = 500;
        float boundary = 1000f;

        private double rotationSpeed = MathHelper.PiOver4/200;
        private double orintationAngle;
        private double tankAngle = MathHelper.PiOver2;
        private double turretAngle = MathHelper.PiOver2;
        private double acceleration = 0.002;
        private float scaleRatio = 0.05f;

        private bool isMoving;
        //9-11
        private bool isPatrolling;
        private Tank targetTank;

        public EnemyTank(Model model, GraphicsDevice device, Camera camera, Vector3 position,int tankID) : base(model)
        {
            npcTankID = tankID;
            mousePick = new MousePick(device, camera);
            //this.position = new Vector3(500, 0, 200);         //enemy tank's spawn position
            this.position = position;
            currentVelocity = Vector3.Normalize(new Vector3(0, 0, 1));  //enemy tank is facing vector3(0,0,1) when it spawn, this is initial velocity
            turretBone = model.Bones["turret_geo"];

            leftBackWheelBone = model.Bones["l_back_wheel_geo"];
            rightBackWheelBone = model.Bones["r_back_wheel_geo"];

            //9-11 patrol
            RandomPatrolPoint();

            translation = Matrix.CreateTranslation(position);
        }

        public void AddTarget(Tank playerTank)
        {
            this.targetTank = playerTank;
        }

        //9-11 patrol
        public void RandomPatrolPoint()
        {
            Random ran = new Random();
            int x = ran.Next(-1000, 1000);
            int z = ran.Next(-1000, 1000);
            targetPosition = new Vector3(x, 0, z);
        }

        public override void Update(GameTime gameTime)
        {
            int elapsedFrameTime = gameTime.ElapsedGameTime.Milliseconds;

            //9-11 patrol
            float distance = (targetTank.position - position).Length();
            if (distance > chaseMaxDistance)
            {
                isPatrolling = true;
            }
            else
            {
                isPatrolling = false;
                targetPosition = targetTank.position;
            }

            //9-11 patrol
            if(currentSpeed == 0 && isPatrolling)
            {
                RandomPatrolPoint();
            }
            MovingToTarget(elapsedFrameTime);

            //leftBackWheelBone.Transform *= Matrix.CreateRotationX(MathHelper.PiOver4 / 20);
            //rightBackWheelBone.Transform *= Matrix.CreateRotationX(MathHelper.PiOver4 / 20);

            base.Update(gameTime);
        }

        //9-11 patrol
        private void MovingToTarget(int elapsedFrameTime)
        {
            double turnedAngle = rotationSpeed * elapsedFrameTime;
            orintation = targetPosition - position;
            desiredVelocity = Vector3.Normalize(orintation) * maxSpeed;
            orintationAngle = Math.Atan2(orintation.X, orintation.Z);

            if (currentVelocity.Length() > 1)
                currentVelocity.Normalize();

            currentVelocity *= (float)currentSpeed;

            //the enemy tank will chase player and keep 100 distance

            if ((targetPosition - position).Length() > chaseMinDistance)
            {
                isMoving = true;

                //tank will accelerate from 0 to max speed
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += acceleration * elapsedFrameTime;
                }
                else
                    currentSpeed = maxSpeed;

                //steering behavior of the enemy tank
                
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

                isMoving = false;
            }
            Steering(elapsedFrameTime);
            RotateTank(turnedAngle);
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
            LimitInBoundary();
            translation = Matrix.CreateTranslation(position);
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }

        public override Matrix Getworld()
        {
            return Matrix.CreateScale(scaleRatio) * rotation;
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
