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
        Task Save(ByteArray parentId, T entity);

        [Action]
        Task Save(ByteArray parentId, T[] entities);

        [Action]
        Task<T> Get(ByteArray parentId, ByteArray entityId);

        [Action]
        Task<T[]> Get(ByteArray parentId, ByteArray[] entityIds);

        [Action]
        Task<T[]> Get(ByteArray parentId, int skip, int take);

        [Action]
        Task<T[]> Get(ByteArray parentId, OrderType orderType, int skip, int take);

        [Action]
        Task<T[]> GetDistinct(ByteArray parentId, OrderType orderType, int skip, int take);

        [Action]
        Task<bool> Contains(ByteArray parentId, ByteArray entityId);

        [Action]
        Task Remove(ByteArray parentId, ByteArray entityId);

        [Action]
        Task Remove(ByteArray parentId, ByteArray[] entityIds);
    }
}
