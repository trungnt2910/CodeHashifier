namespace CodeHashifier.Uno.Impl
{
    public abstract class CodeHashifierEngine
    {
        public static CodeHashifierEngine Default { get; } = new DefaultCodeHashifierEngine();

        protected CodeHashifierEngine() { }

        public abstract string Hash(string source);
    }
}
