using FluentEmail.Core;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IFluentEmailFactory _fluentEmailFactory;

    public EmailService(ILogger<EmailService> logger, IFluentEmailFactory fluentEmailFactory)
    {
        _logger = logger;
        _fluentEmailFactory = fluentEmailFactory;
    }

    public async Task Send(EmailMessageModel emailMessageModel)
    {
        try
        {
            _logger.LogInformation("Sending email");
            var result = _fluentEmailFactory.Create().To(emailMessageModel.ToAddress)
                    .Subject(emailMessageModel.Subject)
                    .Body(emailMessageModel.Body, true);
            if (emailMessageModel.AttachmentPath != null) 
            {
                string filePath = $"{emailMessageModel.AttachmentPath}";
                result.AttachFromFilename(filePath, "application/pdf", "Receipt");
            }
            await result.SendAsync();
            
        }
        catch (Exception ex) {
            throw ex;
        }
    }
}

public interface IEmailService
{
    /// <summary>
    /// Send an email.
    /// </summary>
    /// <param name="emailMessage">Message object to be sent</param>
    Task Send(EmailMessageModel emailMessage);
}

public class EmailMessageModel
{
    public string ToAddress { get; set; }
    public string Subject { get; set; }
    public string? Body { get; set; }
    public string? AttachmentPath { get; set; }
    public EmailMessageModel(string toAddress, string subject, string? body = "")
    {
        ToAddress = toAddress;
        Subject = subject;
        Body = body;
    }
}