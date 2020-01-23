namespace Snoozle.Configuration
{
    public class SqlStrings
    {
        public string SelectAll { get; }
        public string SelectById { get; }
        public string DeleteById { get; }

        public SqlStrings(string selectAll, string selectById, string deleteById)
        {
            SelectAll = selectAll;
            SelectById = selectById;
            DeleteById = deleteById;
        }
    }
}
