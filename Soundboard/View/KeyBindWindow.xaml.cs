using Soundboard.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Soundboard.ViewModel;

namespace Soundboard.View
{
    /// <summary>
    /// Logika interakcji dla klasy KeyBind.xaml
    /// </summary>
    public partial class KeyBindWindow : Window
    {
        private Sound sound;
        private SoundViewModel soundVM;
        private KeyBind keybind;
        public KeyBindWindow(KeyBind keybind,SoundViewModel soundVM,Sound sound)
        {
            this.keybind = keybind;
            this.sound = sound;
            this.soundVM = soundVM;
            InitializeComponent();
            
        }
        bool keyup = true;
        bool KeybindSet = false;

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ButtonBind.Content = "Click buttons";
            //ButtonBind.Focusable = true;

            if(KeybindSet)
            {
                soundVM.KeyBindSet(sound);
                this.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            keybind.isPaused = false;
        }

        private void ButtonBind_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                case Key.S:
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                    e.Handled = true;
                    break;
                default:
                    break;
            }
            if (keyup)
            {
                sound.KeyOne = e.Key;
                sound.KeyTwo = Key.None;
                keyup = false;
                ButtonBind.Content = "Click to set: " + sound.KeyOne.ToString();
                KeybindSet = true;

            }
            else
            {
                sound.KeyTwo = e.Key;
                ButtonBind.Content = "Click to set: "+ sound.KeyOne.ToString()+" "+e.Key.ToString();
                KeybindSet = true;
            }
        }

        private void ButtonBind_KeyUp(object sender, KeyEventArgs e)
        {
            if(sound.KeyOne == e.Key)
            {
                keyup = true;
            }
        }
    }
}
