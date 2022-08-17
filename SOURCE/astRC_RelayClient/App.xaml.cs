using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Views;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : PrismApplication, IDisposable
    {
        public IMessageDialog MessageDialog { get; set; }
        private System.Threading.Mutex mutex = new System.Threading.Mutex(false, "Global\\{29E32D9A-E169-430A-A621-8DB97E44F0FA}");
        private bool alreadyShowErrorMessage;
        private bool disposedValue;

        public App()
        {
            InitializeComponent();
            Logger.Debug("-----Start-----");
            AppDomain.CurrentDomain.UnhandledException += (sender, e2) => UnhandledException((Exception)e2.ExceptionObject);
            MessageDialog = new WpfMessageDialog(this.Dispatcher, null);
        }

        private void UnhandledException(Exception e)
        {
            if (alreadyShowErrorMessage)
            {
                return;
            }
            MessageDialog.Error(RelayClient.Properties.Resources.AnUnexpectedErrorOccurred);
            Logger.Error(e);
            alreadyShowErrorMessage = true;
        }

        private bool CheckDuplicate()
        {
            try
            {
                // astRC_RelayClient.exeの多重起動を防止する。
                Logger.Debug("astRC_RelayClient.exeの多重起動防止のためのMutexを取得する。");
                return this.mutex.WaitOne(0, false);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error("多重起動を検出しました。同一PCで他のユーザーが既に本プログラムを起動していると思われます。", ex);
                return false;
            }
            catch (AbandonedMutexException e) // astRC_RelayClient.exeが異常終了後、再度起動されるときに例外が発生する。（Mutexは正しく取得できているため無視。）
            {
                Logger.Debug("異常終了検知", e);
                return true;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ReleaseMutex();
            Logger.Debug("-----End  -----");
            base.OnExit(e);
        }

        protected override Window CreateShell()
        {
            if (CheckDuplicate() == false)
            {
                MessageDialog.Warn(RelayClient.Properties.Resources.HasAlreadyBeenInRunning);
                mutex.Close();
                mutex = null;
                Shutdown();
            }
            return Container.Resolve<Views.LoginWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<Views.SwitchWindow, SwitchWindowViewModel>(nameof(SwitchWindowViewModel));
            containerRegistry.RegisterDialog<Views.ClientWindow, ClientWindowViewModel>(nameof(ClientWindowViewModel));
            containerRegistry.RegisterDialog<Views.ServerWindow, ServerWindowViewModel>(nameof(ServerWindowViewModel));
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledException(e.Exception);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ReleaseMutex();
                }

                disposedValue = true;
            }
        }

        private void ReleaseMutex()
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Close();
                mutex.Dispose();
            }
        }

        ~App()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
