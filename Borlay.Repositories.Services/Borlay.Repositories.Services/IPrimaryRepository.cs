using Borlay.Arrays;
using Borlay.Handling.Notations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Repositories.Services
{
    [NameScope("PrimaryRepository")]
    public interface IPrimaryRepository<T> where T : IEntity
    {
        [IdAction(1)]
        Task Save(T entity);

        [IdAction(2)]
        Task<T> Get(ByteArray entityId);
    }

    public interface ISortedPrimaryRepository<T> : IPrimaryRepository<T> where T : IEntity
    {
        [IdAction(3)]
        Task<T[]> Get(int skip, int take);
    }
}
