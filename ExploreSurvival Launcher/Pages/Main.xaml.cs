﻿using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExploreSurvival_Launcher.Pages
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : System.Windows.Controls.Page
    {
        private IniFile config = new IniFile(Environment.CurrentDirectory + "/esl.ini");
        public Main()
        {
            InitializeComponent();
            AsyncRun();
        }
        private async void AsyncRun()
        {
            NEWS.Text = await GetNEWS();
        }

        private async Task<string> GetNEWS()
        {
            return await Task.Run(() =>
            {
                HttpWebRequest request = WebRequest.CreateHttp("http://www.exploresurvival.ml/news.txt");
                request.Method = "GET";
                request.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string news = sr.ReadToEnd();
                sr.Close();
                response.Close();
                return news;
            });
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

        private bool CheckJava()
        {
            if (config.exists("config", "JavaPath"))
            {
                if (!File.Exists(config.read("config", "JavaPath")))
                {
                    return false;
                }
            }
            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckJava())
            {
                // java.exe -Djava.library.path=natives -Xmx512M -jar game.jar <用户名> <sessionID> <uuid>
                if (config.exists("account", "userName") && bool.Parse(config.read("account", "offlineLogin")) && File.Exists("ExploreSurvival/game.jar"))
                {
                    try
                    {
                        Process.Start(config.read("config", "JavaPath"), "-Djava.library.path=ExploreSurvival/natives -Xmx" + config.read("config", "JvmMemery") + "M -jar ExploreSurvival/game.jar " + config.read("account", "userName"));
                        Dialog("游戏已启动", "已启动进程");
                    }
                    catch (Exception ex)
                    {
                        Dialog("无法启动", ex.ToString());
                    }
                }
                else if (!config.exists("account", "userName"))
                {
                    Dialog("无法启动", "未登录");
                }
                else if (!File.Exists("ExploreSurvival/game.jar"))
                {
                    Dialog("无法启动",  "你没有下载ExploreSurvival");
                }
                else
                {
                    Dialog("服务器离线", "无法连接到ExploreSurvival验证服务器");
                }
            }
            else
            {
                Dialog("注意", "ExploreSurival需要Java,你并没有安装Java\n如果你已经安装了,请前往设置界面配置Java");
            }
        }
    }
}