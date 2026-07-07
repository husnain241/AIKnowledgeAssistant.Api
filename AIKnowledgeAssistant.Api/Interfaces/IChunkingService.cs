namespace AIKnowledgeAssistant.Api.Interfaces
{
    public interface IChunkingService
    {
        List<string> ChunkText(
            string text,
            int chunkSize = 500,
            int overlap = 50);
    }
}
