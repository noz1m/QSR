namespace QSR.Application.Interfaces.Integrations;

public interface ISmsProvider : IIntegrationProvider
{
    Task<SmsSubmissionResult> SendAsync(
        SmsSubmission request, CancellationToken cancellationToken = default);
}

public sealed record SmsSubmission(
    string IdempotencyKey, string PhoneNumber, string Message);

public sealed record SmsSubmissionResult(
    string ExternalId, SmsSubmissionStatus Status);

public enum SmsSubmissionStatus
{
    Accepted,
    Rejected,
    Unknown
}
