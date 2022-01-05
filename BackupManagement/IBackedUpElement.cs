public interface IBackedUpElement {
    string Path { get; }
    byte[] Hash { get; }
}