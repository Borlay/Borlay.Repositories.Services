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
        Task Save(T[] entities);

        [IdAction(3)]
        Task<T> Get(ByteArray entityId);

        [IdAction(4)]
        Task<T[]> Get(ByteArray[] entityIds);

        [IdAction(5)]
        Task<T[]> Get(OrderType orderType, int skip, int take);

        [IdAction(6)]
        Task<T[]> GetDistinct(OrderType orderType, int skip, int take);

        [IdAction(7)]
        Task<bool> Contains(ByteArray entityId);

        [IdAction(8)]
        Task<bool> Remove(ByteArray entityId);

        [IdAction(9)]
        Task<bool> Remove(ByteArray[] entityIds);
    }
}
