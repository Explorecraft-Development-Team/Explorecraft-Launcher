﻿using ModernWpf.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ExploreSurvival_Launcher.Pages
{
    /// <summary>
    /// Account.xaml 的交互逻辑
    /// </summary>
    public partial class Account : System.Windows.Controls.Page
    {
        private IniFile config = new IniFile(Environment.CurrentDirectory + "/esl.ini");
        public Account()
        {
            InitializeComponent();
            if (config.exists("account", "userName"))
            {
                HideLogin();
                Welcome.Content += config.read("account", "userName");
            }
            else
            {
                HideLoginAfter();
            }
        }

        private void Dialog(string Title, string Content)
        {
            new ContentDialog
            {
                Title = Title,
                Content = Content,
                CloseButtonText = "OK"
            }.ShowAsync();
        }
        private void HideLogin()
        {
            userName.Visibility = Visibility.Hidden;
            userNameL.Visibility = Visibility.Hidden;
            userPass.Visibility = Visibility.Hidden;
            userPassL.Visibility = Visibility.Hidden;
            OfflineLogin.Visibility = Visibility.Hidden;
            login.Visibility = Visibility.Hidden;
        }

        private void ShowLogin()
        {
            userName.Visibility = Visibility.Visible;
            userNameL.Visibility = Visibility.Visible;
            userPass.Visibility = Visibility.Visible;
            userPassL.Visibility = Visibility.Visible;
            OfflineLogin.Visibility = Visibility.Visible;
            login.Visibility = Visibility.Visible;
        }

        private void HideLoginAfter()
        {
            Logout.Visibility = Visibility.Hidden;
            Welcome.Visibility = Visibility.Hidden;
        }
        private void ShowLoginAfter()
        {
            Logout.Visibility = Visibility.Visible;
            Welcome.Visibility = Visibility.Visible;
        }


        private void login_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)OfflineLogin.IsChecked)
            {
                config.write("account", "userName", userName.Text);
                config.write("account", "offlineLogin", "true");
                HideLogin();
                Restart("登录成功");
            }
            else
            {
                Dialog("服务器离线", "无法连接到ExploreSurvival验证服务器");
            }
        }

        private async void Restart(string Title)
        {
            await new ContentDialog
            {
                Title = Title,
                Content = "启动器需要重新启动",
                CloseButtonText = "OK"
            }.ShowAsync();
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }

        private void OfflineLogin_Checked(object sender, RoutedEventArgs e)
        {
            userPass.IsEnabled = false;
            login.IsEnabled = userName.Text != "";
        }

        private void OfflineLogin_Unchecked(object sender, RoutedEventArgs e)
        {
            userPass.IsEnabled = true;
            login.IsEnabled = userPass.Password != "";
        }

        private void userName_TextChanged(object sender, TextChangedEventArgs e)
        {
            login.IsEnabled = (userName.Text != "" && userPass.Password != "") || ((bool)OfflineLogin.IsChecked && userName.Text != "");
        }

        private void userPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            login.IsEnabled = userName.Text != "" && userPass.Password != "";
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            config.write("account", "userName", "");
            HideLoginAfter();
            ShowLogin();
            Restart("注销成功");
        }
    }
}
