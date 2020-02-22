using System;

namespace Snoozle
{
    /// <summary>
    /// The HTTP method verbs that Snoozle supports.
    /// </summary>
    [Flags]
    public enum HttpVerbs
    {
        None        = 0,
        GET         = 1,
        POST        = 2,
        PUT         = 4,
        DELETE      = 8,

        All         = GET | POST | PUT | DELETE
    }
}
