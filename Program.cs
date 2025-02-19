
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

    // interface
    interface TappValidation
    {
        public void ValidateParamInitial();

        public void GetVersion();
    }


    public static class DateTimeExtensions
    {
        // Convert datetime to UNIX time
        public static string ToUnixTime(this DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            return dto.ToUnixTimeSeconds().ToString();
        }

        // Convert datetime to UNIX time including miliseconds
        public static string ToUnixTimeMilliSeconds(this DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            return dto.ToUnixTimeMilliseconds().ToString();
        }

        public static DateTime StartOfDay(this DateTime theDate)
        {
            return theDate.Date;
        }

        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }
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

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            ApplicationConfiguration.Initialize();

            var sentryOptions = new SentryOptions
            {

                // Tells which project in Sentry to send events to:
                Dsn = "https://e4cdeac9989376f915e74c403ac87363@o4505348454744064.ingest.sentry.io/4505703081443328",

                // When configuring for the first time, to see what the SDK is doing:
                Debug = true,

                // Set traces_sample_rate to 1.0 to capture 100% of transactions for tracing.
                // We recommend adjusting this value in production.
                //TracesSampleRate = 1.0,

                // Enable Global Mode since this is a client app
                IsGlobalModeEnabled = true,

                //TODO: any other options you need go here

                Release = "TAppUploadDei@1.0.4",
            };

            using (SentrySdk.Init(sentryOptions))
            {
                Application.Run(new FormSelectApp());
            }

        }



    }
}