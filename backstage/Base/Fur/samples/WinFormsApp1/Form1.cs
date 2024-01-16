using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Web.WebView2.Core;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    public Form1(IServer server)    // ע�� IServer ���񣬻�ȡ Web ������ַ/�˿�
    {
        InitializeComponent();

        webview.Dock = DockStyle.Fill;
        webview.Source = new Uri(server.GetServerAddress());

        // ��� WebView �Ҽ��˵�����������д�
        webview.CoreWebView2InitializationCompleted += (a, c) =>
        {
            webview.CoreWebView2.ContextMenuRequested += (sender, args) =>
            {
                var newItem = webview.CoreWebView2.Environment.CreateContextMenuItem(
                    "��������д�", null, CoreWebView2ContextMenuItemKind.Command);

                newItem.CustomItemSelected += (send, ex) =>
                {
                    System.Diagnostics.Process.Start("explorer.exe", args.ContextMenuTarget.PageUri);
                };

                args.MenuItems.Insert(args.MenuItems.Count, newItem);
            };
        };
    }
}