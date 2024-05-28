namespace ChatServer.Services;

public abstract class BaseService<TData>
{
    static List<TData> _data { get; } = [];

    protected BaseService()
    {
        _data.AddRange(SeedData());
    }

    public virtual List<TData> SeedData() { return []; }

    public TData? Get(Func<TData, bool> predicate) => _data.FirstOrDefault(predicate);

    public IEnumerable<TData> GetMany(Func<TData, bool> predicate) => _data.Where(predicate);
}