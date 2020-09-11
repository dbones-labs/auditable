namespace Auditable.AspNetCore.Tests
{
    using System;
    using Microsoft.AspNetCore.Authentication;

    public static class Setup
    {
        public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder, Action<TestAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(DisabledAuthValues.Scheme, DisabledAuthValues.Authority, configureOptions);

        }
    }
}