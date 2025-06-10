using react_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.DTO.Reaction.Request.Comment
{
    public class CreateCommentReactionRequest
    {
        public string CommentId { get; set; }
        public ReactionType ReactionType { get; set; }
    }
}
