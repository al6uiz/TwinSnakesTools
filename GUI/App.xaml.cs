using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace TwinSnakesTools.GUI
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Dark);
            
            StyleManager.IsEnabled = true;
            StyleManager.ApplicationTheme = new VisualStudio2013Theme();

        }
    }
}
