namespace NetCoreDemoApi.Repositories.SqlLite
{
    public class ClientQueries
    {

        private static readonly string table = "clients";

        public static readonly string GetAll = $"SELECT id,name,email,deleted from {table} where deleted = 0";

        public static readonly string GetByEmail = $"select id,name,email,deleted from {table} where email=@email";

        public static readonly string GetById = $"select id,name,email,deleted from {table} where id=@id and deleted = 0";

        public static readonly string Create = $@"
                            INSERT INTO {table} (name,email) VALUES (@name,@email);
                            SELECT last_insert_rowid();";

        public static readonly string Delete = $"UPDATE {table} SET deleted = 1 WHERE id = @id";

        public static readonly string Update = $"UPDATE {table} SET name = @name, email = @email WHERE id = @id";
    }
}
