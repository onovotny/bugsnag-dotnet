﻿using Bugsnag;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BugsnagDemoWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var bugsnag = new Client("9134c4469d16f30f025a1e98f45b3ddb");

            bugsnag.Config.AppVersion = "6.7.8";
            bugsnag.Config.ReleaseStage = "Beta WPF";
            bugsnag.Config.SetUser("2222", "cccc@dddd.com", "CCcc Dddd");
            bugsnag.Config.FilePrefix = new List<string> { @"e:\GitHub\Bugsnag-NET\" };

            base.OnStartup(e);

        }
    }
}