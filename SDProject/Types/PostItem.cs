using GrpcServer.Blog;

namespace SDProject.Types {
    public struct PostItem {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }

        public static implicit operator PostItem(Post post) {
            return new PostItem {
                Id = post.Id,
                Title = post.Title,
                Abstract = post.Abstract,
                Body = post.Body,
                ImageUrl = post.ImageUrl
            };
        }
    }
}