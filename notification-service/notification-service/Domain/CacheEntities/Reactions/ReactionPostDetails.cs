using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CacheEntities.Reactions
{
    public class ReactionPostDetails
    {
        public UserMostUsedData User { get; set; } = null!; // User data for the reaction, including user ID, profile image URL, and username
        public string PostId { get; set; } = string.Empty;

        //this should be aggregated once every day as the content of the post may change
        public string PostContent { get; set; } = string.Empty; // Unique identifier for the reaction

        public ReactionType ReactionType { get; set; }  // List of reaction types (e.g., like, love, laugh, etc.)
        public DateTime CreatedAt { get; set; } // Timestamp of when the reaction was created

    }
}
