namespace NetCoreDemoApi.Repositories.SqlLite;

public static class UserQueries
{
    private static readonly string table = "users";

    public static readonly string GetById = $"select id,name,email,password from {table} where id = @id";

    public static readonly string GetByEmail = $"select id,name,email,password from {table} where email = @email";
}
