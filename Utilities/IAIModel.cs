using System.Threading.Tasks;

namespace ChatSystem.Utilities
{
    public interface IAIModel
    {
        string Name { get; }
        Task Initialize(string modelPath); 
        Task<string> GetResponse(string input);
    }
}