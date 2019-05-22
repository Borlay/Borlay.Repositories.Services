using Borlay.Arrays;
using Borlay.Handling.Notations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Repositories.Services
{
    [Scope("SecondaryRepositoryService")]
    public interface ISecondaryRepositoryService<T> where T : class, IEntity
    {
        [Action]
        Task SaveAsync(ByteArray parentId, T entity);

        [Action]
        Task SaveAsync(ByteArray parentId, T[] entities);

        [Action]
        Task<T> GetAsync(ByteArray parentId, ByteArray entityId);

        [Action]
        Task<T[]> GetAsync(ByteArray parentId, ByteArray[] entityIds);

        [Action]
        Task<T[]> GetAsync(ByteArray parentId, bool distinct, int skip, int take);

        [Action]
        Task<T[]> GetAsync(ByteArray parentId, OrderType orderType, bool distinct, int skip, int take);

        [Action]
        Task<bool> ExistAsync(ByteArray parentId, ByteArray entityId);

        [Action]
        Task RemoveAsync(ByteArray parentId, ByteArray entityId);

        [Action]
        Task RemoveAsync(ByteArray parentId, ByteArray[] entityIds);
    }
}
