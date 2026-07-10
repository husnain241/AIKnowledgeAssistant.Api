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
        public DbSet<DocumentChunk> DocumentChunks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Document>()
                .HasMany(d => d.Chunks)
                .WithOne(c => c.Document)
                .HasForeignKey(c => c.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConversationDocument>()
    .HasKey(cd => new { cd.ConversationId, cd.DocumentId });

            modelBuilder.Entity<ConversationDocument>()
                .HasOne(cd => cd.Conversation)
                .WithMany(c => c.ConversationDocuments)
                .HasForeignKey(cd => cd.ConversationId);

            modelBuilder.Entity<ConversationDocument>()
                .HasOne(cd => cd.Document)
                .WithMany(d => d.ConversationDocuments)
                .HasForeignKey(cd => cd.DocumentId);
        }
    }

}