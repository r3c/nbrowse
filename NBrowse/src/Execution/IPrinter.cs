namespace NBrowse.Execution
{
	public interface IPrinter
	{
		void Print<TValue>(TValue result);
	}
}