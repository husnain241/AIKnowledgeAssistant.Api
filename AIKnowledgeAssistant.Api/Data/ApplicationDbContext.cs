using AIKnowledgeAssistant.Api.Data;
using AIKnowledgeAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AIKnowledgeAssistantAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Document> Documents { get; set; }

    }
}