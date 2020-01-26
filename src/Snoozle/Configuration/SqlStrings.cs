namespace Snoozle.Configuration
{
    public class SqlStrings
    {
        public SqlStrings(string selectAll, string selectById, string deleteById, string insert, string updateById)
        {
            SelectAll = selectAll;
            SelectById = selectById;
            DeleteById = deleteById;
            Insert = insert;
            UpdateById = updateById;
        }

        public string SelectAll { get; }
        public string SelectById { get; }
        public string DeleteById { get; }
        public string Insert { get; }
        public string UpdateById { get; }
    }
}
