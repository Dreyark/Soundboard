using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Soundboard.Model;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using NAudio.Wave;
using System.Collections;

namespace Soundboard.ViewModel
{
    public class SoundViewModel
    {
        public List<String> devices;
        WaveOutEvent OutputDevice;
        private ICommand _AddSoundButton;
        private MainWindow mainWindow;
        SoundList soundList;
        public List<Sound> Sounds;
        public SoundViewModel(MainWindow mainWindow)
        {
            soundList = new SoundList();
            this.mainWindow = mainWindow;
            Sounds = soundList.MakeSoundList();

            devices = new List<String>();
            for (int n = 0; n < WaveOut.DeviceCount; n++)
            {
                devices.Add(WaveOut.GetCapabilities(n).ProductName.ToString());
            }
        }

        public void KeyBindSet(Sound sound)
        {
            Sound Old = Sounds.Find(s => s.FilePath == sound.FilePath);
            Old = sound;
            soundList.Serialize(Sounds);
        }

        public SoundViewModel()
        {
            devices = new List<String>();
            for (int n = 0; n < WaveOut.DeviceCount; n++)
            {
                devices.Add(WaveOut.GetCapabilities(n).ProductName.ToString());
            }
        }

        public void OnTitleChange(string path,string newTitle)
        {
            var obj = Sounds.Find(x => x.FilePath == path);
            obj.Name = newTitle;
            soundList.Serialize(Sounds);
        }

        public void OnSoundDelete(Sound sound)
        {
            Sounds.Remove(sound);
            soundList.Serialize(Sounds);
        }

        public void Play(Sound sound)
        {
            if (sound != null)
            {
                MediaFoundationReader audioFile = new MediaFoundationReader(sound.FilePath);
                if (OutputDevice != null)
                {
                    OutputDevice.Stop();
                    OutputDevice.Init(audioFile);
                    OutputDevice.Play();
                }
            }
        }
        
        public void Stop()
        {
            if (OutputDevice != null)
                OutputDevice.Stop();
        }

        public void SetOutput(int i)
        {
            OutputDevice = new WaveOutEvent() { DeviceNumber = i };
        }

        public ICommand AddSoundButton
        {
            get
            {
                if (_AddSoundButton == null)
                {
                    _AddSoundButton = new RelayCommand(() =>
                    {
                        var fileDialog = new OpenFileDialog();
                        fileDialog.Multiselect = true;
                        if (fileDialog.ShowDialog() == true)
                        {
                            for (int i = 0; i < fileDialog.FileNames.Length; i++)
                            {

                                if (fileDialog.SafeFileNames[i].ToLower().EndsWith(".mp3"))
                                {
                                    var sound = new Sound();

                                    sound.Name = fileDialog.SafeFileNames[i].Substring(0, fileDialog.SafeFileNames[i].Length - 4);
                                    sound.FilePath = fileDialog.FileNames[i];
                                    sound.KeyOne = Key.None;
                                    sound.KeyTwo = Key.None;
                                    if (!Sounds.Exists(s => s.FilePath == sound.FilePath))
                                    {
                                        Sounds.Add(sound);
                                        soundList.Serialize(Sounds);
                                    }
                                }
                            }
                        }
                        mainWindow.RefreshListOfSounds();
                    });
                }
                return _AddSoundButton;
            }
        }
    }
}
