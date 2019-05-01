using AppleWinKeyboard.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

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
            WindowState = WindowState.Minimized;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _hotkeyRegistrator = new HotkeyRegistrator(this);
            foreach(KeyValuePair<Key, Action> kv in _keyMapper.ActionsMap)
                _hotkeyRegistrator.Add(kv.Key.Modifier, kv.Key.WinKey, kv.Value);
            _hotkeyRegistrator.Register();
        }

        private void OpenWindowAction()
        {
        }
    }
}
