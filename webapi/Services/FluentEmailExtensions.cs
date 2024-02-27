using FluentEmail.Smtp;
using System.Net.Mail;
using System.Net;


namespace webapi.Services
{
    public static class FluentEmailExtensions
    {
        public static void AddFluentEmail(this IServiceCollection services,
            ConfigurationManager configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            var defaultFromEmail = emailSettings["DefaultFromEmail"];
            var host = emailSettings["Host"];
            var port = emailSettings.GetValue<int>("Port");
            services.AddFluentEmail(defaultFromEmail);
            var sender = services.AddSingleton<FluentEmail.Core.Interfaces.ISender>(x => new SmtpSender
            (new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("elihunt98@gmail.com", "mnnd xzfa khwb yuwq"),
                EnableSsl = true
            })); ;
            
        }
    }
}
