using System;

namespace Lunohod.Windows
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleAppDomainCurrentDomainUnhandledException; ;
            try
            {
                using (GameEngine game = new GameEngine())
                {
                    game.Run();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        static void HandleAppDomainCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogError((Exception)e.ExceptionObject);
        }

        static void LogError(Exception ex)
        {
            //System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "lunohod_error.txt", ex.ToString());
            System.IO.File.WriteAllText("lunohod_error.txt", ex.ToString());
        }
    }
#endif
}

