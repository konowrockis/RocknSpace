﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;

namespace RocknSpace
{
    public class Profiles : INotifyPropertyChanged
    {
        private ObservableCollection<Profile> list;
        private Profile current;

        public event PropertyChangedEventHandler PropertyChanged;
        public static event EventHandler CurrentChanged;

        public ObservableCollection<Profile> List
        {
            get { return list; }
            set { list = value; propertyChanged("List"); }
        }

        public Profile CurrentProfile
        {
            get { return current; }
            set
            {
                current = value; propertyChanged("Current");
                CurrentChanged?.Invoke(Current, EventArgs.Empty);
            }
        }

        public static Profiles Instance { get; private set; }
        public static Profile Current
        {
            get { return Instance.CurrentProfile; }
            set { Instance.CurrentProfile = value; if (Current.State != null) Current.State.OnLoad(); }
        }

        static Profiles()
        {
            Instance = new Profiles();
            Load();
        }

        private Profiles()
        {
            List = new ObservableCollection<Profile>();
        }

        private void propertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static Profile Create(string name)
        {
            Profile p = new Profile(name);
            Instance.List.Add(p);
            return p;
        }

        public static Profile GetByName(string name)
        {
            return Instance.List.FirstOrDefault(p => p.Name == name);
        }

        public static void Load()
        {
            if (!File.Exists(Settings.ProfilesPath)) return;

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Profile>));
            TextReader reader = new StreamReader(Settings.ProfilesPath);
            Instance.List = (ObservableCollection<Profile>)serializer.Deserialize(reader);
            reader.Close();
        }

        public static void Save()
        {
            foreach (Profile profile in Instance.List)
            {
                if (profile.State != null)
                    profile.State.OnSave();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Profile>));
            TextWriter writer = new StreamWriter(Settings.ProfilesPath);
            serializer.Serialize(writer, Instance.List);
            writer.Close();
        }
    }

    [Serializable]
    public class Profile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private Key keyup, keyleft, keyright, keystop;
        private float sound, music;
        private bool blur, fullscreen;
        private GameState state;

        public string Name
        {
            get { return name; }
            set { name = value; propertyChanged("Name"); }
        }

        public Key KeyUp
        {
            get { return keyup; }
            set { keyup = value; propertyChanged("KeyUp"); }
        }
        public Key KeyLeft
        {
            get { return keyleft; }
            set { keyleft = value; propertyChanged("Key"); }
        }
        public Key KeyRight
        {
            get { return keyright; }
            set { keyright = value; propertyChanged("KeyRight"); }
        }
        public Key KeyStop
        {
            get { return keystop; }
            set { keystop = value; propertyChanged("KeyStop"); }
        }

        public float Sound
        {
            get { return sound; }
            set { sound = value; propertyChanged("Sound"); }
        }
        public float Music
        {
            get { return music; }
            set { music = value; propertyChanged("Music"); }
        }

        public bool Blur
        {
            get { return blur; }
            set { blur = value; propertyChanged("Blur"); }
        }
        public bool Fullscreen
        {
            get { return fullscreen; }
            set { fullscreen = value; propertyChanged("Fullscreen"); }
        }

        public GameState State
        {
            get { return state; }
            set { state = value; propertyChanged("State"); }
        }

        public Profile() : this(string.Empty) { }
        public Profile(string name)
        {
            this.Name = name;
            KeyUp = Key.W;
            KeyLeft = Key.A;
            KeyRight = Key.D;
            KeyStop = Key.Space;
            
            Sound = 1.0f;
            Music = 1.0f;
            Blur = true;
            Fullscreen = false;
        }

        private void propertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
