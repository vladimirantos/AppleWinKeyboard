using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace AppleWinKeyboard.Core
{
    internal class KeyMapper
    {
        public Dictionary<Key, Action> ActionsMap { get; set; } = new Dictionary<Key, Action>();

        public KeyMapper()
        {
            ActionsMap.Add(new Key(Modifiers.Ctrl | Modifiers.Win, Keys.F10), VolumeControl.Mute);
            ActionsMap.Add(new Key(Modifiers.Ctrl | Modifiers.Win, Keys.F11), VolumeControl.VolumeDown);
            ActionsMap.Add(new Key(Modifiers.Ctrl | Modifiers.Win, Keys.F12), VolumeControl.VolumeUp);
            ActionsMap.Add(new Key(Modifiers.Ctrl | Modifiers.Win, Keys.F7), PlayControl.Previous);
            ActionsMap.Add(new Key(Modifiers.Ctrl | Modifiers.Win, Keys.F8), PlayControl.Pause);
            ActionsMap.Add(new Key(Modifiers.Ctrl | Modifiers.Win, Keys.F9), PlayControl.Next);
            //ActionsMap.Add(new Key(Modifiers.Ctrl | Modifiers.Win, Keys.F1), Brightness.BrightnessDown);
            //ActionsMap.Add(new Key(Modifiers.Ctrl | Modifiers.Win, Keys.F2), Brightness.BrightnessUp);

        }
    }

    internal struct Key
    {
        public Modifiers Modifier { get; set; }
        public Keys WinKey { get; set; }

        public Key(Modifiers modifier, Keys key)
        {
            Modifier = modifier;
            WinKey = key;
        }
    }

    internal enum SupportedActions
    {
        VolumeUp, VolumeDown
    }
}
