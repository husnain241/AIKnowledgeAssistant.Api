using AIKnowledgeAssistant.Api.Interfaces;

namespace AIKnowledgeAssistant.Api.Services
{
    public class ChunkingService : IChunkingService
    {
        public List<string> ChunkText(
            string text,
            int chunkSize = 500,
            int overlap = 50)
        {
            if (string.IsNullOrWhiteSpace(text))
                return [];

            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var chunks = new List<string>();

            int step = chunkSize - overlap;

            for (int i = 0; i < words.Length; i += step)
            {
                var chunkWords = words
                    .Skip(i)
                    .Take(chunkSize);

                var chunk = string.Join(" ", chunkWords);

                chunks.Add(chunk);

                if (i + chunkSize >= words.Length)
                    break;
            }

            return chunks;
        }
    }
}
