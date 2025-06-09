using Domain.Entities;
using Domain.IRepository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentRepository(IMongoDatabase db)
        {
            _comments = db.GetCollection<Comment>("comments");
        }

        // ---------- single-item CRUD ----------

        public async Task<Comment?> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _comments.Find(c => c.Id == objectId)
                                  .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Comment comment) =>
            await _comments.InsertOneAsync(comment);

        public async Task UpdateAsync(Comment comment)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.Id, comment.Id);
            await _comments.ReplaceOneAsync(filter, comment);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            await _comments.DeleteOneAsync(c => c.Id == objectId);
        }

        // ---------- skip-based paging ----------

        public async Task<IEnumerable<Comment>> GetByPostIdAsync(
            string postId, int skip = 0, int limit = 10)
        {

            return await _comments.Find(c=>c.PostId==postId)
                                  .SortByDescending(c => c.CreatedAt)
                                  .Skip(skip)
                                  .Limit(limit)
                                  .ToListAsync();
        }

        // ---------- cursor-based paging (preferred) ----------

        public async Task<IEnumerable<Comment>> GetByPostIdCursorAsync(string postId, string? afterCommentId, int limit = 10)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.PostId, postId);

            if (!string.IsNullOrEmpty(afterCommentId) &&
                ObjectId.TryParse(afterCommentId, out var afterId))
            {
                // only comments with Id < cursor (i.e., older)
                var cursorFilter =
                    Builders<Comment>.Filter.Lt(c => c.Id, afterId);

                filter = Builders<Comment>.Filter.And(filter, cursorFilter);
            }

            return await _comments.Find(filter)
                                  .SortByDescending(c => c.Id)  // newest first
                                  .Limit(limit)
                                  .ToListAsync();
        }

        // ---------- reaction count ----------

        public async Task<bool> IncrementReactionCountAsync(string commentId)
        {
            var objectId = ObjectId.Parse(commentId);
            var filter = Builders<Comment>.Filter.Eq(c => c.Id, objectId);
            var update = Builders<Comment>.Update.Inc(c => c.ReactCount, 1);
            var result = await _comments.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DecrementReactionCountAsync(string commentId)
        {
            var objectId = ObjectId.Parse(commentId);
            // Only decrement if ReactCount > 0
            var filter = Builders<Comment>.Filter.And(
                Builders<Comment>.Filter.Eq(c => c.Id, objectId),
                Builders<Comment>.Filter.Gt(c => c.ReactCount, 0)
            );
            var update = Builders<Comment>.Update.Inc(c => c.ReactCount, -1);
            var result = await _comments.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // ---------- bulk delete by post ----------

        public async Task DeleteByPostIdAsync(string postId)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.PostId, postId);
            await _comments.DeleteManyAsync(filter);
        }
    }
}
