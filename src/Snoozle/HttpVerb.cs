using System;

namespace Snoozle
{
    [Flags]
    public enum HttpVerb
    {
        None        = 0,
        GET         = 1,
        POST        = 2,
        PUT         = 4,
        DELETE      = 8,

        All         = GET | POST | PUT | DELETE
    }
}
