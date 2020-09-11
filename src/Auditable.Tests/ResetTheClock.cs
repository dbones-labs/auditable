namespace Auditable.Tests
{
    using Machine.Specifications;

    public class ResetTheClock : ICleanupAfterEveryContextInAssembly
    {
        public void AfterContextCleanup()
        {
            SystemDateTime.Reset();
        }
    }
}