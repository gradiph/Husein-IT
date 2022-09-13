using BackOfficeWeb.Models.MessageBus;

namespace BackOfficeWeb
{
    public static class MiddlewareSetup
    {
        public static WebApplication SetupMiddleware(this WebApplication app)
        {
            LogWriter.Instance.LogAsync(new ChannelViewModel(), LogType.Trace, "Setupping middlewares...");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            return app;
        }
    }
}
