﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace K1_Parser_v1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = "";
            Uri iconUri = new Uri(Path.Combine(Directory.GetCurrentDirectory(), "Tapa File - Icon.png"), UriKind.RelativeOrAbsolute);
            ////Uri iconUri = new Uri(@"pack://application:,,,/Images/Tapa File - Icon.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
        }
    }
}
