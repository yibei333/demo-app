using System.Collections.ObjectModel;

namespace ChatServer.Services;

public abstract class BaseService<TData>
{
    List<TData> _data { get; } = [];

    protected BaseService()
    {
        _data.AddRange(SeedData());
    }

    public ReadOnlyCollection<TData> Context => new(_data);

    public virtual List<TData> SeedData() { return []; }

    public TData? Get(Func<TData, bool> predicate) => _data.FirstOrDefault(predicate);

    public IEnumerable<TData> GetMany(Func<TData, bool> predicate) => _data.Where(predicate);

    public void Add(TData data) => _data.Add(data);

    public void Remove(TData data) => _data.Remove(data);
}