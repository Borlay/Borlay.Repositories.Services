using Borlay.Arrays;
using Borlay.Handling.Notations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Repositories.Services
{
    [Scope("PrimaryRepositoryService")]
    public interface IPrimaryRepositoryService<T> where T : class, IEntity
    {
        [Action]
        Task Save(T entity);

        [Action]
        Task Save(T[] entities);

        [Action]
        Task<T> Get(ByteArray entityId);

        [Action]
        Task<T[]> Get(ByteArray[] entityIds);

        [Action]
        Task<T[]> Get(int skip, int take);

        [Action]
        Task<T[]> Get(OrderType orderType, int skip, int take);

        [Action]
        Task<T[]> GetDistinct(OrderType orderType, int skip, int take);

        [Action]
        Task<bool> Contains(ByteArray entityId);

        [Action]
        Task Remove(ByteArray entityId);

        [Action]
        Task Remove(ByteArray[] entityIds);
    }
}
