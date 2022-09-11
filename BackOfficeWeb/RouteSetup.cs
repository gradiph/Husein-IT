namespace BackOfficeWeb
{
    public static class RouteSetup
    {
        public static WebApplication SetupRoutes(this WebApplication app)
        {
            LogWriter.Instance.LogAsync(new ChannelViewModel(), LogType.Trace, "Setupping routes...");

            app.MapControllerRoute(
                name: "MyArea",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            return app;
        }
    }
}
