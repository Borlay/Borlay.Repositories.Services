using Borlay.Arrays;
using Borlay.Serialization.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Repositories.Services
{
    public class PrimaryRepositoryService<T> : IPrimaryRepositoryService<T> where T : class, IEntity
    {
        protected readonly IPrimaryRepository repository;
        protected readonly ISerializer serializer;

        public PrimaryRepositoryService(IPrimaryRepository repository, ISerializer serializer)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            this.repository = repository;
            this.serializer = serializer;
        }

        public async Task Save(T entity)
        {
            using (var transaction = repository.CreateTransaction())
            {
                var index = 0;
                var buffer = new byte[1024];
                serializer.AddBytes(entity, buffer, ref index);
                transaction.AppendValue(entity.Id, buffer, index);

                await transaction.Commit();
            }
        }

        public async Task<T> Get(ByteArray entityId)
        {
            var buffer = repository.Get(entityId);
            if (buffer == null || buffer.Length == 0)
                return null;

            var index = 0;
            var entity = serializer.GetObject(buffer, ref index);
            return (T)entity;
        }

        public async Task<T[]> Get(OrderType orderType, int skip, int take)
        {
            throw new NotSupportedException();
        }
    }
}
