﻿using Snoozle.SqlServer.Configuration;

namespace Snoozle.SqlServer.Interfaces
{
    public interface ISqlGenerator
    {
        string SelectAll(ISqlResourceConfiguration config);

        string SelectById(ISqlResourceConfiguration config);

        string DeleteById(ISqlResourceConfiguration config);

        string Insert(ISqlResourceConfiguration config);

        string Update(ISqlResourceConfiguration config);
    }
}
