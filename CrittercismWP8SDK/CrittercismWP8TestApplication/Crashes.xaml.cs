﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CrittercismWP8TestApplication {
    public partial class Crashes : PhoneApplicationPage {
        public Crashes() {
            InitializeComponent();
        }

        private void backButtonClicked(object sender, RoutedEventArgs e) {
            NavigationService.Navigate(new Uri("/Customers.xaml", UriKind.Relative));
        }

        private void nextButtonClicked(object sender, RoutedEventArgs e) {
            NavigationService.Navigate(new Uri("/CrashSim.xaml", UriKind.Relative));
        }
    }
}