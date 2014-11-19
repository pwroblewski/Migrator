using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Migrator.Helpers;

namespace Migrator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SQLiteAsyncConnection Connection { get; set; }

        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
