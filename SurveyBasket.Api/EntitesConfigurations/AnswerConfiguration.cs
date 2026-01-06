
namespace SurveyBasket.Api.EntitesConfigurations;

public class AnswerConfiguraion : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasIndex(a=> new {a.QuestionId, a.Content }).IsUnique();
        builder.Property(a => a.Content).HasMaxLength(1000);
        
    }
}
