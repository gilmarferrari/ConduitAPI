namespace Conduit.Application.Helpers
{
    public static class EnvironmentVariables
    {
        private static readonly Dictionary<VariableType, string> _variableNames = new Dictionary<VariableType, string>
        {
            { VariableType.Database, "CONDUIT_DATABASE_CONNECTION_STRING" },
            { VariableType.JwtSecret, "CONDUIT_JWT_SECRET" },
        };

        public static string GetEnvironmentVariable(VariableType type)
        {
            var variableName = _variableNames[type];

            try
            {
                var variable =  Environment.GetEnvironmentVariable(variableName);

                if (variable == null)
                {
                    throw new ApplicationException($"Could not retrieve the environment variable '{variableName}'");
                }

                return variable;
            }
            catch (Exception)
            {
                throw new ApplicationException($"Could not retrieve the environment variable '{variableName}'");
            }
        }
    }

    public enum VariableType
    {
        Database,
        JwtSecret,
    }
}
