namespace WinFormsApp1;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        // Serve.RunNative(RunOptions.Default);    // Ĭ�� 5000 �˿ڣ��������ռ�ã��Ƽ�ʹ������ķ�ʽ
        Serve.RunNative(RunOptions.Default, Serve.IdleHost.Urls);   // ����˿�

        ApplicationConfiguration.Initialize();
        Application.Run(Native.CreateInstance<Form1>());
    }
}