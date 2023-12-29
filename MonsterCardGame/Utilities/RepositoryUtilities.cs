using System.Data;

namespace MonsterCardGame.Utilities
{
    internal static class RepositoryUtilities
    {
        public static void AddParameter(IDbCommand command, string parameterName, DbType dbType, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            parameter.DbType = dbType;
            command.Parameters.Add(parameter);
        }
    }
}
