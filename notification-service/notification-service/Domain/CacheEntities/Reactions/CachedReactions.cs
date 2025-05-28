using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CacheEntities.Reactions
{
    /// <summary>
    /// 
    ///     كل الداتا اللي تخليني مرحش ف اي سيرفس تانيه 
    ///     +
    ///     كل الداتا اللي تربطنى ب ال كور داتابيز 
    ///     
    /// </summary>
    public class CachedReactions
    {
        public string AuthorId { get; set; } = string.Empty;
        public List<ReactionPostDetails> ReactionsOnPosts { get; set; } = [];

        public List<ReactionCommentDetails> ReactionsOnComments { get; set; } = [];

    }
}
