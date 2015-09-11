using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ass1
{
    public class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public List<BasicModel> models = new List<BasicModel>();
        public List<BasicModel> shots = new List<BasicModel>();

        //steven
        List<BasicModel> modelsObstacle = new List<BasicModel>();
        List<BasicModel> modelsObstacleFirm = new List<BasicModel>();

        //updated code
        public List<BasicModel> npcModels = new List<BasicModel>();
        float shotMinZ = 5000;
        float shotMinX = 5000;
        Vector3 relativePosition = new Vector3(0.5f, 0, 0.5f);
        Vector3 collisionDistance;

        //steven
        BasicModel tankModel;

        //updated code
        int shotDelay = 300;
        //9_11
        Random rndEnemyShotDelay;

        int enemyShotDelay = 5000;
        int shotCountdown = 0;
        int enemyShotCountdown = 0;
        Random rndPosition;
        int xSpawn;
        int zSpawn;
        //9_11
        Random rndEnemyAddShot;

        public ModelManager(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            rndPosition = new Random();
            //9_11
            rndEnemyShotDelay = new Random();
            rndEnemyAddShot = new Random();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Tank playerTank = new Tank(
                Game.Content.Load<Model>(@"Tank/tank"),
                ((Game1)Game).GraphicsDevice,
                ((Game1)Game).camera);


            FleeTank fleeTank = new FleeTank(
                Game.Content.Load<Model>(@"Tank/tank"),
                ((Game1)Game).GraphicsDevice,
                ((Game1)Game).camera);
            AddEnemyTanks(StaticValue.ENEMY_TANK_AMOUNT, playerTank); // add enemy tanks
            //enemyTank.AddTarget(playerTank);
            fleeTank.AddTarget(playerTank);
            npcModels.Add(fleeTank);

            npcModels.Add(new Cube(
                Game.Content.Load<Model>(@"Models/stone"),
                new Vector3(0, 0, 500)));

            models.Add(new Ground(
                Game.Content.Load<Model>(@"Ground/Ground")));
            //models.Add(new SkyBox(
            //    Game.Content.Load<Model>(@"Skybox/skybox")));
            //models.Add(new Pokeball(
            //    Game.Content.Load<Model>(@"Pokeball/Pokeball"),
            //    ((Game1)Game).GraphicsDevice,
            //    ((Game1)Game).camera));
            models.Add(playerTank);

            //npcModels.Add(enemyTank);
            
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (BasicModel model in models)
            {
                //steven
                //if (!Collide())
                //{
                //    Console.WriteLine("no collision~~~~");
                //}
                //else
                //{
                //    Console.WriteLine("collision!!!");
                //    stopPlayer();
                //}

                model.Update(gameTime);
            }
            foreach (BasicModel npc in npcModels)
            {
                npc.Update(gameTime);
            }
            foreach (BasicModel shot in shots)
            {
                shot.Update(gameTime);
            }
            UpdateShots(gameTime);

            //updated code
            FireShots(gameTime);
            EnemyFireShots(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (BasicModel model in models)
            {
                model.Draw(((Game1)Game).device, ((Game1)Game).camera);
            }

            // Loop through and draw each shot 
            foreach (BasicModel bm in shots)
            {
                bm.Draw(((Game1)Game).device,
                    ((Game1)Game).camera);
            }

            foreach (BasicModel rock in npcModels)
            {
                rock.Draw(((Game1)Game).device,
                   ((Game1)Game).camera);
            }

            //steven
            foreach (BasicModel model in modelsObstacle)
            {
                model.Draw(((Game1)Game).device, ((Game1)Game).camera);
            }
            foreach (BasicModel model in modelsObstacleFirm)
            {
                model.Draw(((Game1)Game).device, ((Game1)Game).camera);
            }

            base.Draw(gameTime);
        }

        public void AddShot(Vector3 position, Vector3 direction)
        {
            shots.Add(new Rocket
                (Game.Content.Load<Model>(@"Models\Rocket"),
                position,
                direction,
                ((Game1)Game).GraphicsDevice,
                ((Game1)Game).camera,
                RocketType.PLAYER_ROCKET));
        }
        //updated code
        public void AddEnemyShot(Vector3 position, Vector3 direction)
        {
            shots.Add(new Rocket
                (Game.Content.Load<Model>(@"Models\Rocket"),
                position,
                direction,
                ((Game1)Game).GraphicsDevice,
                ((Game1)Game).camera,
                RocketType.ENEMY_ROCKET));
        }

        protected void UpdateShots(GameTime gameTime)
        {
            // Loop through shots    
            for (int i = 0; i < shots.Count; ++i)
            {
                // Update each shot        
                shots[i].Update(gameTime);
                // If shot is out of bounds, remove it from game        
                if (shots[i].Getworld().Translation.Z > shotMinZ ||
                    shots[i].Getworld().Translation.Z < -shotMinZ ||
                    shots[i].Getworld().Translation.X > shotMinX ||
                    shots[i].Getworld().Translation.X < -shotMinX)
                {
                    shots.RemoveAt(i);
                    --i;
                }
                else
                {
                    // If shot is still in play, check for collisions            
                    for (int j = 0; j < npcModels.Count; ++j)
                    {
                        if (shots[i].getRocketType() == RocketType.PLAYER_ROCKET)
                        {
                            if (shots[i].CollidesWith(npcModels[j].model, npcModels[j].Getworld()
                            * Matrix.CreateTranslation(npcModels[j].GetTankPosition())))
                            {
                                //updated code
                                if (npcModels[j].tankType() == TankType.FLEE_TANK)
                                {
                                    ((Game1)Game).text.updateScore(100);
                                }
                                if (npcModels[j].tankType() == TankType.ENEMY_TANK)
                                {
                                    ((Game1)Game).text.updateScore(50);
                                }
                                // Collision! remove the ship and the shot.                    
                                npcModels.RemoveAt(j);
                                shots.RemoveAt(i);
                                --i;
                                break;
                            }
                        } 
                    }
                    //9_11
                    if (shots[i].getRocketType() == RocketType.ENEMY_ROCKET)
                    {
                        if (shots[i].CollidesWith(models[1].model, models[1].Getworld()
                            * Matrix.CreateTranslation(models[1].GetTankPosition())))
                        {
                            ((Game1)Game).text.updateLife(1, false);
                            shots.RemoveAt(i);
                            --i;
                            break;
                        }
                    }
                }
            }
        }


        protected void FireShots(GameTime gameTime)
        {
            if (shotCountdown <= 0)
            {
                // Did player press space bar or left mouse button?        
                if (Keyboard.GetState().IsKeyDown(Keys.T))
                {
                    // Add a shot to the model manager            
                    AddShot(models[1].GetTankPosition() + new Vector3(0, 10, 0),
                         models[1].GetTankDirection());
                    // Reset the shot countdown            
                    shotCountdown = shotDelay;
                }
            }
            else
                shotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
        }
        protected void EnemyFireShots(GameTime gameTime)
        {
            if (enemyShotCountdown <= 0)
            {
                for (int i = 0; i < npcModels.Count; i++)
                {
                    if (npcModels[i].tankType() == TankType.ENEMY_TANK && rndEnemyAddShot.Next(100) % 2 == 0)
                    {
                        AddEnemyShot(npcModels[i].GetTankPosition() + new Vector3(0, 10, 0),
                            npcModels[i].GetTankDirection());
                    }
                }
                //9_11
                enemyShotCountdown = rndEnemyShotDelay.Next(3000, 7000);
            }
            else
                enemyShotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
        }
        protected void AddEnemyTanks(int number, Tank playerTank)
        {
            for (int i = 0; i < number; i++)
            {
                xSpawn = rndPosition.Next(-1000, 1000);
                zSpawn = rndPosition.Next(-1000, 1000);

                EnemyTank enemyTank = new EnemyTank(
                    Game.Content.Load<Model>(@"Tank/tank"),
                    ((Game1)Game).GraphicsDevice,
                    ((Game1)Game).camera,
                    new Vector3(xSpawn, 0, zSpawn));

                enemyTank.AddTarget(playerTank);

                npcModels.Add(enemyTank);
            }
        }

        //steven
        protected bool Collide()
        {
            Vector3 tankPosition = tankModel.GetTankPosition();

            for (int i = modelsObstacle.Count - 1; i >= 0; i--)
            {
                Vector3 obstaclePosition = modelsObstacle[i].GetTankPosition();
                Rectangle tankRect = new Rectangle((int)tankPosition.X,
                    (int)tankPosition.Z, 100, 100);
                Rectangle obstacleRect = new Rectangle((int)obstaclePosition.X,
                    (int)obstaclePosition.Z, 100, 100);
                Console.WriteLine((tankPosition.X - obstaclePosition.Z) + "----------");
                Console.WriteLine((obstaclePosition.X - obstaclePosition.Z) + "----------");
                if (tankRect.Intersects(obstacleRect))
                {
                    modelsObstacle.Remove(modelsObstacle[i]);
                }

                //if (modelsObstacle[i].CollidesWith(tankModel.model, tankModel.Getworld()))
                //{
                //    modelsObstacle.Remove(modelsObstacle[i]);
                //}
            }

            for (int i = modelsObstacleFirm.Count - 1; i >= 0; i--)
            {
                Vector3 obstaclePosition = modelsObstacleFirm[i].GetTankPosition();
                Rectangle tankRect = new Rectangle((int)tankPosition.X,
                    (int)tankPosition.Z, 100, 100);
                Rectangle obstacleRect = new Rectangle((int)obstaclePosition.X,
                    (int)obstaclePosition.Z, 100, 100);
                //Console.WriteLine((tankModel.GetScale().Up.Z - tankModel.GetScale().Down.Z) + "Y--------Y");
                if (tankRect.Intersects(obstacleRect))
                    return true;
            }
            return false;
        }

        //steven
        public void stopPlayer()
        {
            //tankModel.WheelRotationValue = 0f;
            tankModel.setModelSpeed(0f);
        }
    }
}
