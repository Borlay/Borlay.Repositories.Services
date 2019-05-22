using Borlay.Serialization.Notations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Borlay.Repositories.Services
{
    public interface IIndexMap<T>
    {
        IndexLevel Level { get; }
        OrderType Order { get; }
        long GetScore(T entity);
    }

    public class IndexMapProvider<T> : IEnumerable<IIndexMap<T>>
    {
        List<IIndexMap<T>> maps = new List<IIndexMap<T>>();

        public void AddDateIndex(Func<T, DateTime> dateProvider, IndexLevel level, OrderType order)
        {
            maps.Add(new DateIndexMap<T>(dateProvider, level, order));
        }

        public void AddScoreIndex(Func<T, long> scoreProvider, IndexLevel level, OrderType order)
        {
            maps.Add(new ScoreIndexMap<T>(scoreProvider, level, order));
        }

        public void AddIndex(params IIndexMap<T>[] indexMaps)
        {
            maps.AddRange(indexMaps);
        }

        public IEnumerator<IIndexMap<T>> GetEnumerator()
        {
            return maps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return maps.GetEnumerator();
        }
    }

    public class ScoreIndexMap<T> : IIndexMap<T>
    {
        private readonly Func<T, long> scoreProvider;

        public ScoreIndexMap(Func<T, long> scoreProvider, IndexLevel level, OrderType order)
        {
            this.scoreProvider = scoreProvider;
            this.Level = level;
            this.Order = order;
        }

        public IndexLevel Level { get; }

        public OrderType Order { get; }

        public long GetScore(T entity)
        {
            return scoreProvider(entity);
        }
    }

    public class DateIndexMap<T> : IIndexMap<T>
    {
        private readonly Func<T, DateTime> dateProvider;

        public DateIndexMap(Func<T, DateTime> dateProvider, IndexLevel level, OrderType order)
        {
            this.dateProvider = dateProvider;
            this.Level = level;
            this.Order = order;
        }

        public IndexLevel Level { get; }

        public OrderType Order { get; }

        public long GetScore(T entity)
        {
            var date = dateProvider(entity);
            var offset = new DateTimeOffset(date);
            var score = offset.ToUnixTimeMilliseconds();
            return score;
        }
    }

    public interface IHasIndexMaps
    {
    }

    public interface IHasIndexMaps<T> : IHasIndexMaps
    {
        IndexMapProvider<T> IndexMaps { get; }
    }

    //[Data(2158)]
    //public enum Order
    //{
    //    Asc = 1,
    //    Desc
    //}
}
