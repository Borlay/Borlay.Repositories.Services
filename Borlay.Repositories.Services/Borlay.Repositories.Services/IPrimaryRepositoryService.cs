using Borlay.Arrays;
using Borlay.Handling.Notations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Repositories.Services
{
    [NameScope("PrimaryRepositoryService")]
    public interface IPrimaryRepositoryService<T> where T : class, IEntity
    {
        [IdAction(1)]
        Task Save(T entity);

        [IdAction(2)]
        Task<T> Get(ByteArray entityId);

        [IdAction(3)]
        Task<T[]> Get(OrderType orderType, int skip, int take);
    }
}
