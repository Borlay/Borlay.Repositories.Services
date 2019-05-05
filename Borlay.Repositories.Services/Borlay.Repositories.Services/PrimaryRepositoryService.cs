using Borlay.Arrays;
using Borlay.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task Save(T[] entities)
        {
            using (var transaction = repository.CreateTransaction())
            {
                var buffer = new byte[1024];
                foreach (var entity in entities)
                {
                    var index = 0;
                    serializer.AddBytes(entity, buffer, ref index);
                    transaction.AppendValue(entity.Id, buffer, index);
                }

                await transaction.Commit();
            }
        }

        public async Task<T> Get(ByteArray entityId)
        {
            var bytes = repository.Get(entityId);
            var entity = Serialize(bytes);
            return entity;
        }

        public async Task<T[]> Get(ByteArray[] entityIds)
        {
            var entities = repository.Get(entityIds).Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<T[]> Get(int skip, int take)
        {
            var keys = repository.Get().Select(s => new ByteArray(s)).Skip(skip).Take(take).ToArray();
            var entities = repository.Get(keys).Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<T[]> Get(OrderType orderType, int skip, int take)
        {
            var keys = repository.Get(orderType).Select(s => new ByteArray(s)).Skip(skip).Take(take).ToArray();
            var entities = repository.Get(keys).Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<T[]> GetDistinct(OrderType orderType, int skip, int take)
        {
            var keys = repository.Get(orderType).Select(s => new ByteArray(s)).Distinct().Skip(skip).Take(take).ToArray();
            var entities = repository.Get(keys).Select(s => Serialize(s.Value)).ToArray();
            return entities;
        }

        public async Task<bool> Contains(ByteArray entityId)
        {
            return repository.Contains(entityId);
        }

        public async Task Remove(ByteArray entityId)
        {
            using (var transaction = repository.CreateTransaction())
            {
                transaction.Remove(entityId);
                await transaction.Commit();
            }
        }

        public async Task Remove(ByteArray[] entityIds)
        {
            using (var transaction = repository.CreateTransaction())
            {
                transaction.Remove(entityIds);
                await transaction.Commit();
            }
        }

        protected virtual T Serialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            var index = 0;
            var entity = serializer.GetObject(bytes, ref index);
            return (T)entity;
        }
    }
}
