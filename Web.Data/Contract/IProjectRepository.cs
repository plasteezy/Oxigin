using System.Collections.Generic;
using Global.Contracts;
using Web.Data.Model;

namespace Web.Data.Contract
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
    }

    public interface IProjectDataSetRepository : IGenericRepository<ProjectDataSet>
    {
    }

    public interface IReportRepository : IGenericRepository<SavedQuery>
    {
    }

    public interface IApiCredentialRepository : IGenericRepository<ApiCredential>
    {
    }

    public interface IUserAccountRepository
    {
        List<UserAccount> AllAccounts();

        UserAccount FindByUserId(string userId);

        UserAccount FindByEmail(string email);

        UserAccount FindByPhone(string phone);

        void UpdateLoginDate(string username);
    }
}
