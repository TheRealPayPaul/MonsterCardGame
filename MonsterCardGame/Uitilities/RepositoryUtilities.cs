using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Uitilities
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
