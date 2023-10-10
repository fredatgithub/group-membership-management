using Entities;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IDatabaseEmailTypesRepository
    {
        Task<int?> GetEmailTypeIdByEmailTemplateName(string emailTemplateName);

    }
}       

