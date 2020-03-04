using AppleWinKeyboard.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace AppleWinKeyboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HotkeyRegistrator _hotkeyRegistrator;
        private KeyMapper _keyMapper;

        public MainWindow()
        {
            InitializeComponent();
            _keyMapper = new KeyMapper();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _hotkeyRegistrator = new HotkeyRegistrator(this);
            foreach(KeyValuePair<Key, Action> kv in _keyMapper.ActionsMap)
                _hotkeyRegistrator.Add(kv.Key.Modifier, kv.Key.WinKey, kv.Value);
            _hotkeyRegistrator.Register();
            Visibility = Visibility.Hidden;
        }

        private void OpenWindowAction()
        {
        }
    }
}
