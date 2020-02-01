namespace Snoozle.SqlServer.Interfaces
{
    public interface ISqlParamaterProvider
    {
        string GenerateParameterName(string propertyName);
        string GetPrimaryKeyParameterName();
    }
}
