using Borlay.Arrays;
using Borlay.Handling.Notations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Repositories.Services
{
    [NameScope("SecondaryRepository")]
    public interface ISecondaryRepository<T> where T : IEntity
    {
        [IdAction(1)]
        Task Save(ByteArray userId, T entity);

        [IdAction(2)]
        Task<T> Get(ByteArray userId, ByteArray entityId);
    }

    public interface ISortedSecondaryRepository<T> : ISecondaryRepository<T> where T : IEntity
    {
        [IdAction(3)]
        Task<T[]> Get(ByteArray userId, int skip, int take);
    }
}
