using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
using Soundboard.Model;

namespace Soundboard.ViewModel
{
    public class KeyBind
    {
        public bool isPaused = false;
        public bool isRunning = true;
        public Thread TH;
        List<Sound> sounds;
        SoundViewModel soundVM;
        public KeyBind(SoundViewModel soundVM)
        {
            this.soundVM = soundVM;
            this.sounds = soundVM.Sounds;
            TH = new Thread(KeyboardListener);
            TH.SetApartmentState(ApartmentState.STA);
            TH.Start();
        }

        public void ThreadInterrupted()
        {
            TH.Interrupt();
        }

        void KeyboardListener()
        {
            try
            {
                while (isRunning)
                {
                    while (!isPaused)
                    {
                        Thread.Sleep(40);
                        foreach (Sound sound in sounds)
                        {
                            if (sound.KeyOne != Key.None && sound.KeyTwo != Key.None)
                            {
                                if ((Keyboard.GetKeyStates(sound.KeyOne) & KeyStates.Down) > 0 && (Keyboard.GetKeyStates(sound.KeyTwo) & KeyStates.Down) > 0)
                                {
                                    soundVM.Play(sound);
                                }
                            }
                            else if (sound.KeyOne != Key.None)
                            {
                                if ((Keyboard.GetKeyStates(sound.KeyOne) & KeyStates.Down) > 0)
                                {
                                    soundVM.Play(sound);
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadInterruptedException e)
            {
                System.Console.WriteLine(e);
            }
        }
    }
}
