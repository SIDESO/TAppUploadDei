
namespace TappUploadDei
{
    //enum de estados
    public enum Status
    {
        Pending = 1,

        //estado de exito
        Success = 2,
        //estado de error
        Error = 3,

        Sending = 4
    }




    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FormSelectApp());
        }
    }
}