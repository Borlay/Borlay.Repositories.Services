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
        Task SaveAsync(T entity);

        [Action]
        Task SaveAsync(T[] entities);

        [Action]
        Task<T> GetAsync(ByteArray entityId);

        [Action]
        Task<T[]> GetAsync(ByteArray[] entityIds);

        [Action]
        Task<T[]> GetAsync(bool distinct, int skip, int take);

        [Action]
        Task<T[]> GetAsync(OrderType orderType, bool distinct, int skip, int take);

        [Action]
        Task<bool> ExistAsync(ByteArray entityId);

        [Action]
        Task RemoveAsync(ByteArray entityId);

        [Action]
        Task RemoveAsync(ByteArray[] entityIds);
    }
}
