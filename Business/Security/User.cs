using DSE = Data.Security;
using ESE = Entities.Security;

namespace Business.Security
{
    public class User : Base
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(User));

        public List<ESE.User> List()
        {
            try
            {
                DSE.User dal = new DSE.User();
                return dal.List<ESE.User>();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public ESE.User Search(string id)
        {
            try
            {
                DSE.User dal = new DSE.User();
                return dal.Search(id);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public bool Save(ESE.User user)
        {
            try
            {
                DSE.User dal = new DSE.User();
                return dal.Save(user);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public bool Delete(ESE.User user)
        {
            try
            {
                DSE.User dal = new DSE.User();
                return dal.Delete<ESE.User>(user);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public ESE.User Login(string username, string password)
        {
            try
            {
                DSE.User dal = new DSE.User();
                return dal.Search(username, password);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
    }
}