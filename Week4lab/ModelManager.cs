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
        
        List<BasicModel> modelsObstacle = new List<BasicModel>();
        List<BasicModel> modelsObstacleFirm = new List<BasicModel>();
        List<Cube> modelsObstacleBoundary = new List<Cube>();

        public List<BasicModel> npcModels = new List<BasicModel>();
        float shotMinZ = 5000;
        float shotMinX = 5000;
        Vector3 relativePosition = new Vector3(0.5f, 0, 0.5f);
        Vector3 collisionDistance;
        
        BasicModel tankModel;
        
        int shotDelay = 300;
        Random rndEnemyShotDelay;

        int enemyShotDelay = 5000;
        int shotCountdown = 0;
        int enemyShotCountdown = 0;
        Random rndPosition;
        int xSpawn;
        int zSpawn;
        Random rndEnemyAddShot;

        //steven 9-11
        const float reboundDistance = 10f;
        const float diff = 30f;
        const float modelDiff = 60f;
        Tank playerTank;

        public ModelManager(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            rndPosition = new Random();
            rndEnemyShotDelay = new Random();
            rndEnemyAddShot = new Random();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            playerTank = new Tank(
                Game.Content.Load<Model>(@"Tank/tank"),
                ((Game1)Game).GraphicsDevice,
                ((Game1)Game).camera);


           
            AddNPCTank(StaticValue.TOTAL_NPC_TANK, playerTank); // add enemy tanks
            

            Cube RockModel = new Cube(
                Game.Content.Load<Model>(@"Models/stone"),
                new Vector3(0, 0, 0));
            npcModels.Add(RockModel);

            //steven 9-11
            modelsObstacleFirm.Add(RockModel);
            putBoundary();

            models.Add(new Ground(
                Game.Content.Load<Model>(@"Ground/Ground")));
            //models.Add(new SkyBox(
            //    Game.Content.Load<Model>(@"Skybox/skybox")));
            //models.Add(new Pokeball(
            //    Game.Content.Load<Model>(@"Pokeball/Pokeball"),
            //    ((Game1)Game).GraphicsDevice,
            //    ((Game1)Game).camera));
            models.Add(playerTank);

            base.LoadContent();
        }

        //steven 9-11
        public void putBoundary()
        {
            int b = (int)playerTank.getBoundary();
            for (int x = -1 * b; x <= b; x += 30)
            {
                modelsObstacleBoundary.Add(new Cube(
                    Game.Content.Load<Model>(@"Models/stone"),
                    new Vector3(x, 0, b)));

                modelsObstacleBoundary.Add(new Cube(
                    Game.Content.Load<Model>(@"Models/stone"),
                    new Vector3(x, 0, -b)));

                modelsObstacleBoundary.Add(new Cube(
                    Game.Content.Load<Model>(@"Models/stone"),
                    new Vector3(b, 0, x)));

                modelsObstacleBoundary.Add(new Cube(
                    Game.Content.Load<Model>(@"Models/stone"),
                    new Vector3(-b, 0, x)));
            }

        }

        public override void Update(GameTime gameTime)
        {
            foreach (BasicModel model in models)
            {
                //steven
                Collide();

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
            
            foreach (BasicModel model in modelsObstacle)
            {
                model.Draw(((Game1)Game).device, ((Game1)Game).camera);
            }
            foreach (BasicModel model in modelsObstacleFirm)
            {
                model.Draw(((Game1)Game).device, ((Game1)Game).camera);
            }
            foreach (BasicModel model in modelsObstacleBoundary)
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
                enemyShotCountdown = rndEnemyShotDelay.Next(3000, 7000);
            }
            else
                enemyShotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
        }
        protected void AddNPCTank(int totalTankAmount, Tank playerTank)
        {
            for (int i = 0; i < totalTankAmount; i++)
            {
                xSpawn = rndPosition.Next(-1000, 1000);
                zSpawn = rndPosition.Next(-1000, 1000);
                if (i <= StaticValue.ENEMY_TANK_AMOUNT - 1)
                {
                    EnemyTank enemyTank = new EnemyTank(
                    Game.Content.Load<Model>(@"Tank/tank"),
                    ((Game1)Game).GraphicsDevice,
                    ((Game1)Game).camera,
                    new Vector3(xSpawn, 0, zSpawn),
                    i);
                    enemyTank.AddTarget(playerTank);
                    npcModels.Add(enemyTank);
                }
                else
                {
                    FleeTank fleeTank = new FleeTank(
                Game.Content.Load<Model>(@"Tank/tank"),
                ((Game1)Game).GraphicsDevice,
                ((Game1)Game).camera,
                new Vector3(xSpawn, 0, zSpawn),
                i);
                    fleeTank.AddTarget(playerTank);
                    npcModels.Add(fleeTank);
                }
            }
        }


        //9-11 steven
        //steven
        protected void Collide()
        {
            Vector3 tankPosition = playerTank.GetTankPosition();

            for (int i = modelsObstacle.Count - 1; i >= 0; i--)
            {
                Vector3 obstaclePosition = modelsObstacle[i].GetTankPosition();
                Rectangle tankRect = new Rectangle((int)tankPosition.X,
                    (int)tankPosition.Z, 100, 100);
                Rectangle obstacleRect = new Rectangle((int)obstaclePosition.X,
                    (int)obstaclePosition.Z, 100, 100);
                //Console.WriteLine((tankPosition.X - obstaclePosition.Z) + "----------");
                //Console.WriteLine((obstaclePosition.X - obstaclePosition.Z) + "----------");
                if (tankRect.Intersects(obstacleRect))
                {
                    modelsObstacle.Remove(modelsObstacle[i]);
                }
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
                    roundPlayer(obstaclePosition);
            }

            for (int i = modelsObstacleBoundary.Count - 1; i >= 0; i--)
            {
                Vector3 obstaclePosition = modelsObstacleBoundary[i].GetTankPosition();
                Rectangle tankRect = new Rectangle((int)tankPosition.X,
                    (int)tankPosition.Z, 100, 100);
                Rectangle obstacleRect = new Rectangle((int)obstaclePosition.X,
                    (int)obstaclePosition.Z, 100, 100);
                //Console.WriteLine((tankPosition.X - obstaclePosition.Z) + "----------");
                //Console.WriteLine((obstaclePosition.X - obstaclePosition.Z) + "----ooooo----------");
                if (tankRect.Intersects(obstacleRect))
                    stopPlayer();
            }
        }

        //steven
        public void roundPlayer(Vector3 position)
        {
            Vector3 curPos = playerTank.GetTankPosition();
            playerTank.setModelSpeed(0f);
            float tmpReboundDistancex = 0;
            float tmpReboundDistancey = 0;
            if (curPos.X.CompareTo(position.X - modelDiff) == 0)
            {
                tmpReboundDistancex = -reboundDistance;
            }
            else if (curPos.X.CompareTo(position.X + modelDiff) == 0)
            {
                tmpReboundDistancex = reboundDistance;
            }

            if (curPos.Z.CompareTo(position.Z - modelDiff) > 0)
            {
                tmpReboundDistancey = -reboundDistance;
            }
            else if (curPos.Z.CompareTo(position.Z + modelDiff) < 0)
            {
                tmpReboundDistancey = reboundDistance;
            }

            playerTank.rebounded(tmpReboundDistancex, tmpReboundDistancey);
        }

        public void stopPlayer()
        {
            //tankModel.WheelRotationValue = 0f;
            //playerTank.setModelSpeed(0f);
            float b = playerTank.getBoundary();
            float tmpReboundDistancex = 0;
            float tmpReboundDistancey = 0;
            Vector3 curPos = playerTank.GetTankPosition();
            if (curPos.X.CompareTo(b - diff) > 0)
            {
                tmpReboundDistancex = reboundDistance;
            }
            else if (curPos.X.CompareTo(-b + diff) < 0)
            {
                tmpReboundDistancex = -reboundDistance;
            }

            if (curPos.Z.CompareTo(b - diff) > 0)
            {
                tmpReboundDistancey = reboundDistance;
            }
            else if (curPos.Z.CompareTo(-b + diff) < 0)
            {
                tmpReboundDistancey = -reboundDistance;
            }

            playerTank.rebounded(tmpReboundDistancex, tmpReboundDistancey);
        }
        //public void stopPlayer()
        //{
        //    //tankModel.WheelRotationValue = 0f;
        //    tankModel.setModelSpeed(0f);
        //}
    }
}
