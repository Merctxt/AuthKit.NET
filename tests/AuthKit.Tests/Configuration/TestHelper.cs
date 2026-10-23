namespace AuthKit.Tests.Configuration;

internal static class TestHelper
{
    private const string DefaultJwtSecret = "T3h!K8m#pL2qR$vN5wX7yA0bC4dF9gH1jK3mP6rS8tU0xZ2cE5fI7lO1qT4vW8yB";

    public static (ILogger Logger, List<string> Warnings) CreateLogger()
    {
        var warnings = new List<string>();
        var mockLogger = new Mock<ILogger>();
        mockLogger
            .Setup(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()))
            .Callback(new InvocationAction(invocation =>
            {
                var state = invocation.Arguments[2];
                var formatter = invocation.Arguments[4];
                var method = formatter.GetType().GetMethod("Invoke") ?? formatter.GetType().GetMethod("Invoke`2");
                if (formatter is System.Delegate del)
                {
                    warnings.Add(del.DynamicInvoke(state, null)?.ToString() ?? string.Empty);
                }
            }));

        return (mockLogger.Object, warnings);
    }

    public static AuthKitOptions DefaultOptions(Action<AuthKitOptions>? configure = null)
    {
        var options = new AuthKitOptions
        {
            Jwt = new JwtOptions { SecretKey = DefaultJwtSecret },
        };

        configure?.Invoke(options);

        return options;
    }
}
