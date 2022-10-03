namespace GoBack.Core.SqlServer.Options;

public class SqlServerOptions
{
    private int? _maxRetryCount;

    public TimeSpan TransactionTimeout { get; set; }
    public TimeSpan? CommandTimeout { get; set; }
    public TimeSpan? CommandBatchMaxTimeout { get; set; }
    public Func<IDisposable> Dispose { get; set; }

    public int? MaxRetryCount
    {
        get => _maxRetryCount;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "MaxRetryCount must be greater than 0.");
            }

            _maxRetryCount = value;
        }
    }
}