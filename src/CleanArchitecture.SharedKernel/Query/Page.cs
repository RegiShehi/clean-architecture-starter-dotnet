namespace CleanArchitecture.SharedKernel.Query;

public sealed record Page(int Number = 1, int Size = 10)
{
    public int Skip => Math.Max(0, (Number - 1) * Math.Max(1, Size));
    public int Take => Math.Max(1, Size);
}
