using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ProtoDerp
{
    public class Sprite
    {
        public String fileName;
        public Texture2D index;
        public Vector2 origin;
        public Rectangle BBox;

        /**
         * Constructor
         * 
         * content:  ContentManager used to load content
         * fileName: The file name of the sprite to load, without the file extension.
         */
        public Sprite(ContentManager content, String fileName)
        {
            this.fileName = fileName;
            BBox = Rectangle.Empty;
            index = content.Load<Texture2D>(fileName);
        }

        /**
         * Constructor
         * 
         * content:  ContentManager used to load content
         * fileName: The file name of the sprite to load, without the file extension.
         * origin:   The origin of this sprite; A pivot point that the sprite rotates around.
         */
        public Sprite(ContentManager content, String fileName, Vector2 origin)
        {
            this.fileName = fileName;
            this.origin = origin;
            BBox = Rectangle.Empty;
            index = content.Load<Texture2D>(fileName);
        }

        /**
         * Constructor
         * 
         * content:  ContentManager used to load content
         * fileName: The file name of the sprite to load, without the file extension.
         * origin:   The origin of this sprite; A pivot point that the sprite rotates around.
         */
        public Sprite(ContentManager content, String fileName, Vector2 origin, Rectangle BBox)
        {
            this.fileName = fileName;
            this.origin = origin;
            this.BBox = BBox;
            index = content.Load<Texture2D>(fileName);
        }

    }
}
