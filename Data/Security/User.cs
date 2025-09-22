using System.Data;
using ENT = Entities;
using ESE = Entities.Security;

namespace Data.Security
{
    public class User : Base
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(User));


        public ESE.User Search(string id)
        {
            string query = $"SELECT * FROM security.users where id='{id}'";

            ESE.User BEUser = base.SqlSearch<ESE.User>(query);

            return BEUser;
        }

        public ESE.User Search(string login, string password)
        {
            string query = $"SELECT * FROM security.users where login='{login}' and password='{password}'";

            ESE.User BEUser = base.SqlSearch<ESE.User>(query);

            return BEUser;
        }

        public bool Delete(ESE.User obj)
        {
            return base.Delete(obj);
        }

        public bool Save(ESE.User obj)
        {
            string query = "";

            if (obj.Action == ENT.Action.Insert)
                query = " INSERT INTO security.users (Id,Name,Login,Password) " +
                           " VALUES( @Id,@Name,@Login,@Password) ";
            else if (obj.Action == ENT.Action.Update)
                query = "UPDATE security.users SET " +
                           " Name=@Name,Login=@Login,Password=@Password WHERE Id = @Id ";
            else if (obj.Action == ENT.Action.Delete)
                query = "DELETE FROM security.users WHERE Id = @Id";

            try
            {
                base.dbConnection.Open();

                if (obj.Action == ENT.Action.Insert)
                    obj.Id = base.GenerateId();

                if (obj.Action != ENT.Action.None)
                {
                    base.dbCommand = base.GetQueryCommand(query);
                    base.AddInParameter(base.dbCommand, "@Id", DbType.String, obj.Id);
                    base.AddInParameter(base.dbCommand, "@Name", DbType.String, obj.Name);
                    base.AddInParameter(base.dbCommand, "@Login", DbType.String, obj.Login);
                    base.AddInParameter(base.dbCommand, "@Password", DbType.String, obj.Password);
                    base.dbCommand.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Save Error ", ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                base.DisposeCommand();
                base.DisposeConnection();
            }
            return false;
        }
    }
}