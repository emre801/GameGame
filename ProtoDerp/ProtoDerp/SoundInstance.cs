using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace ProtoDerp
{
    /*
     * A SoundEffectInstance representation with the added functionality to properly restart a sound.
     */
    public class SoundInstance
    {
        SoundEffectInstance instance;
        bool restart;

        public SoundInstance(SoundEffectInstance i)
        {
            instance = i;
            restart = false;
        }

        public bool PlayingOrRestarting()
        {
            return State == SoundState.Playing || restart;
        }

        public void Update()
        {
            if (restart)
            {
                instance.Play();
                restart = false;
            }
        }

        public void Play()
        {
            instance.Play();
        }

        public void Restart()
        {
            instance.Stop();
            restart = true;
        }

        public void Stop()
        {
            instance.Stop();
            restart = false;
        }

        public SoundState State
        {
            get { return instance.State; }
        }

        public float Volume
        {
            set { instance.Volume = value; }
            get { return instance.Volume; }
        }

        public bool IsLooped
        {
            set { instance.IsLooped = value; }
            get { return instance.IsLooped; }
        }
    }

}
