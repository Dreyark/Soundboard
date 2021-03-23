using Microsoft.Win32;
//using NAudio.Wave;
using Soundboard.Control;
using Soundboard.Model;
using Soundboard.View;
using Soundboard.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Soundboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SoundViewModel SoundVM;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        int OutputDeviceNr;
        Sound temporarySound;
        EditableTextBlock editableTextBlock;
        Sound s;
        KeyBind keyBind;
        ICollectionView Source { get; set; }
        public MainWindow()
        {
            SoundVM = new SoundViewModel(this);
            keyBind = new KeyBind(SoundVM);
            InitializeComponent();
            List<Sound> x = new List<Sound>();
            DataContext = SoundVM;
            Source = CollectionViewSource.GetDefaultView(SoundVM.Sounds);
            Source.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
            ListOfSounds.ItemsSource = Source;
            RefreshListOfSounds();
            ListOutputs.ItemsSource = SoundVM.devices;
            SizeChanged += new SizeChangedEventHandler(WindowResize);
        }

        private void WindowResize(object sender, EventArgs e)
        {
            HeaderName.Width = ListOfSounds.ActualWidth - 20;
        }

        private void miEdit_Click(object sender, EventArgs e)
        {
            if (editableTextBlock != null && temporarySound != null)
            {
                editableTextBlock.IsInEditMode = true;
            }
        }

        private void miDelete_Click(object sender, RoutedEventArgs e)
        {
            if (temporarySound != null)
            {
                SoundVM.OnSoundDelete(temporarySound);
                temporarySound = null;
                editableTextBlock = null;
                RefreshListOfSounds();
            }
        }

        private void miBind_Click(object sender, RoutedEventArgs e)
        {
            KeyBindWindow keyBindView = new KeyBindWindow(keyBind,SoundVM, temporarySound);
            keyBindView.Show();
            keyBind.isPaused = true;
        }

        public void RefreshListOfSounds()
        {
            Source.Refresh();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Sound sound = (Sound)(sender as ListViewItem).Content;
                if (sound != null)
                    SoundVM.Play(sound);
            }
        }



        private void TextBox_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            editableTextBlock = (EditableTextBlock)sender;
            editableTextBlock.routedEvents.Add(TextBox_LostFocus);
            temporarySound = SoundVM.Sounds.Find(x => x.Name == (string)editableTextBlock.Text);
        }


        public void StopAudio()
        {
            SoundVM.Stop();
        }


        private void ListOutputs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StopAudio();
            OutputDeviceNr = ListOutputs.SelectedIndex;
            SoundVM.SetOutput(OutputDeviceNr);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopAudio();
        }

        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (editableTextBlock != null)
            {
                string Name = editableTextBlock.Text;
                SoundVM.OnTitleChange(temporarySound.FilePath, Name);
                temporarySound = null;
                editableTextBlock = null;
                RefreshListOfSounds();
            }
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader currentHeader = e.OriginalSource as GridViewColumnHeader;
            if (currentHeader != null && currentHeader.Role != GridViewColumnHeaderRole.Padding)
            {
                using (this.Source.DeferRefresh())
                {
                    Func<SortDescription, bool> lamda = item => item.PropertyName.Equals("Name");
                    if (this.Source.SortDescriptions.Count(lamda) > 0)
                    {
                        SortDescription currentSortDescription = this.Source.SortDescriptions.First(lamda);
                        ListSortDirection sortDescription = currentSortDescription.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;


                        currentHeader.Column.HeaderTemplate = currentSortDescription.Direction == ListSortDirection.Ascending ?
                            this.Resources["HeaderTemplateArrowDown"] as DataTemplate : this.Resources["HeaderTemplateArrowUp"] as DataTemplate;

                        this.Source.SortDescriptions.Remove(currentSortDescription);
                        this.Source.SortDescriptions.Insert(0, new SortDescription("Name", sortDescription));
                    }
                    else
                        this.Source.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                }


            }


        }
        private void ListViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            //switch (e.Key)
            //{
            //    case Key.Left:
            //    case Key.Right:
            //    case Key.Up:
            //    case Key.Down:
            //    case Key.S:
            //    case Key.D0:
            //    case Key.D1:
            //    case Key.D2:
            //    case Key.D3:
            //    case Key.D4:
            //    case Key.D5:
            //    case Key.D6:
            //    case Key.D7:
            //    case Key.D8:
            //    case Key.D9:

            //        e.Handled = true;
            //        break;
            //    default:
            //        break;
            //}
            e.Handled = true;
            Regex key = new Regex(@"\w");
            Sound z = ListOfSounds.SelectedItem as Sound;
            if (e.Key == Key.Enter && z != null)
            {
                SoundVM.Play(z);
            }
            else if (key.IsMatch(e.Key.ToString()) && e.Key != Key.System)
            {
                var strKey = new KeyConverter().ConvertToString(e.Key);
                if (strKey.Length > 1)
                {
                    strKey = strKey.Replace("NumPad", "").Replace("D", "");
                }
                s = SoundVM.Sounds.FirstOrDefault(a => a.Name[0].ToString().ToUpper() == strKey.ToUpper());
                ListOfSounds.SelectedItem = s;
                ListOfSounds.ScrollIntoView(ListOfSounds.SelectedItem);

            }

        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Sound sound = (Sound)ListOfSounds.SelectedItem;
            if (sound != null)
                SoundVM.Play(sound);
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            keyBind.ThreadInterrupted();
        }
    }
}
