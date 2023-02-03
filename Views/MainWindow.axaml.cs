using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ValCord.Handlers;

namespace ValCord.Views
{
    public partial class MainWindow : Window
    {
        private Carousel c;
        private List<String> tabList = new List<string>();
        public MainWindow()
        {
            Logs.Initialize();
            ProcessHandler.Initialize();
            InitializeComponent();
            var dockPanelTitleBar = this.Find<Panel>("TitleBar");
            dockPanelTitleBar.PointerPressed += (s, e)=>
            {
                this.BeginMoveDrag(e);
            };
            
            c = this.FindControl<Carousel>("TabCaro");

            StackPanel tabs = this.FindControl<StackPanel>("TopBarButtons");

            foreach (var tabsChild in tabs.Children)
            {
                tabList.Add(tabsChild.Name);
            }

           
        }

        private void OnButtonClick(object? sender, RoutedEventArgs e)
        {
            String senderName = (sender as Button).Name;
            
            c.SelectedIndex = tabList.IndexOf(senderName);
            //c.Next();
            Console.WriteLine(c.SelectedItem);
        }




    }
}