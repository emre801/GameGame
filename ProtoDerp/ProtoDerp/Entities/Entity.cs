using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/**
 * 
 * Any basic Game Entity that is manipulated in real time, and rendered to the screen.
 * 
 */
namespace ProtoDerp
{
    public abstract class Entity : IComparable<Entity>
    {
        public Sprite sprite = null;                    //Entity's sprite.
        public SpriteStripAnimationHandler anim = null; //Entity's animation.
        public Game game;
        protected readonly int entityId;
        protected readonly int entityType;
        private Dictionary<Entity, float> speedEffects = new Dictionary<Entity, float>();

        public bool IsVisible = true;      // If true, this entity's draw function will be called each draw
        public bool IsUpdateable = true;   // If true, this entity's update function will be called each update
        public bool updatesWhenPaused = false;  // If true, this entity updates even when the game is paused
        public bool isSelected = false; //If True the image is HighLighted 
        public int drawPriority = 0;       // Higher numbers get drawn later.  Therefore higher numbers are on top of smaller ones.  DONT CHANGE THIS AFTER CREATING THE ENTITY
        public Vector2 pos = Vector2.Zero; //Position in the room.
        public float scale = 1.0f;         //% scaling factor.
        public float angle = 0.0f;         //Angle in degrees that this object is rotated around its origin.
        public Color blend = Color.White;  //Color blending for this object's sprite.
        public float alpha = 1.0f;         //Transparency factor for the sprite (1.0 = Fully transclucent, 0.0 = Fully transparent)
        public Rectangle BBox;

        public int animDelay = 100;        //Delay in milliseconds between each frame in this Entity's animation.
        private int animTime = 0;

        

        public Vector2 Velocity = new Vector2();

        public float speedFactor = 1.0f;    // Speed factor for this object

        public bool dispose = false;       //If set to true, this object will be destroyed and garbage-collected ASAP.

        private static int entityCount = 0;
        private static int getNextEntityId()
        {
            return entityCount++;
        }

        protected Entity(Game game)
        {
            this.entityId = getNextEntityId();
            this.game = game;
        }

        //Copy constructor
        protected Entity(Game game, Entity e)
        {
            this.entityId = getNextEntityId();
            this.game = game;
            this.sprite = e.sprite;
            this.IsVisible = e.IsVisible;
            this.IsUpdateable = e.IsUpdateable;
            this.drawPriority = e.drawPriority;

            this.pos = e.pos;
            this.scale = e.scale;
            this.angle = e.angle;
            this.blend = e.blend;
            this.alpha = e.alpha;
            this.Velocity = e.Velocity;
            this.speedFactor = e.speedFactor;
            this.speedEffects = new Dictionary<Entity, float>(e.speedEffects);
        }

        public virtual void OnAddedToGame(GameTime time)
        {
        }

        public virtual void OnRemovedFromGame(GameTime time)
        {
        }
        public virtual void removeItself()
        {

        }
        public virtual void Update(GameTime gameTime, float worldFactor)
        {
            MoveSelf(gameTime, worldFactor);

            //Update Animation
            if (anim != null)
            {
                animTime += (int)(gameTime.ElapsedGameTime.Milliseconds * worldFactor);
                if (animTime > animDelay)
                {
                    anim.nextState();
                    animTime = 0;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            /*
            if (IsVisible == true && anim != null)
                anim.drawCurrentState(spriteBatch, this, new Vector2(0, 0), new Vector2(0, 0),null,new Rectangle(1,1,0,0),false);
            else if (IsVisible == true && sprite != null)
                spriteBatch.Draw(sprite.index, game.drawingTool.getDrawingCoords(pos), null, blend * alpha, MathHelper.ToRadians(angle), sprite.origin, game.drawingTool.gameToScreen(scale), SpriteEffects.None, 0);
            //spriteBatch.Draw(sprite.index, pos, null, blend * alpha, MathHelper.ToRadians(angle), sprite.origin, scale, SpriteEffects.None, 0);
        
             */
        }

        public virtual void MoveSelf(GameTime gameTime, float worldFactor)
        {
            var span = gameTime.ElapsedGameTime;

            pos += Velocity * (float)span.TotalMilliseconds * speedFactor * worldFactor;
        }

        public override String ToString()
        {
            return "Entity pos:" + pos;
        }

        int IComparable<Entity>.CompareTo(Entity other)
        {
            // Ensure drawing priority is respected.  Otherwise, sort by ID
            int priority = this.drawPriority - other.drawPriority;
            return priority == 0 ? this.entityId - other.entityId : priority;
        }

        public void pushSpeed(Entity source, float speed)
        {
            speedEffects[source] = speed;

            if (speedEffects.Count != 0)
            {
                speedFactor = speedEffects.Values.First<float>();
            }
            else
            {
                speedFactor = 1;
            }
        }

        public void popSpeed(Entity source)
        {
            speedEffects.Remove(source);

            if (speedEffects.Count != 0)
            {
                speedFactor = speedEffects.Values.First<float>();
            }
            else
            {
                speedFactor = 1;
            }
        }
    }
}
