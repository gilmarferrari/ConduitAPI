using AspNetCoreRateLimit;

namespace Conduit.API.Extensions
{
    public static class RateLimitingConfig
    {
        public static void AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = true;

                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/v1/users/authenticate", Limit = 20, Period = "1m"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/v1/users/authenticate/session", Limit = 20, Period = "1m"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/v1/users/refresh-token", Limit = 50, Period = "1m"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/v1/users/send-code", Limit = 10, Period = "1m"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/v1/users/validate-code", Limit = 10, Period = "1m"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/v1/users/reset-password", Limit = 5, Period = "1m"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/v1/users/session-token", Limit = 20, Period = "1m"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/v1/users/two-factor-authenticator/recovery", Limit = 5, Period = "1m"
                    }
                };
            });

            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
