using Grpc.Core;
using GrpcServer.Blog;
using SDProject.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SDProject.Utils {
    internal partial class Server {
        internal class Blog {
            //private static readonly List<PostItem> _posts = new List<PostItem> {
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title ="تست دوم", Body = "این یه متن خیلی طولانی قراره باشه واسه تست متن خیلی طولانی یو نو؟ سو پلیز بر ویث می تنک یو"},
            //    new PostItem{Title = "انگیزشی", Body = "اینو ببین انگیزه بگیر 😂", ImageUrl = "https://salamdonya.com/assets/images/48/4884xv6r6.jpg"},
            //    new PostItem{Title = "انگیزشی", Body = "اینو ببین بیشتر انگیزه بگیر 😂", ImageUrl = "https://salamdonya.com/assets/images/77/7794e70xv.jpg"},
            //    new PostItem{Title = "انگیزشی", Body = "انگیزه ریخته", ImageUrl = "https://salamdonya.com/assets/images/64/64826w7au.jpg"},
            //    new PostItem{Title = "انگیزشی", Body = "و پاشیده\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nتست چند خطی", ImageUrl = "https://salamdonya.com/assets/images/67/6797qvcpp.jpg"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "10", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //    new PostItem{Title = "10", Body = "متن تست"},
            //    new PostItem{Title = "تست", Body = "متن تست"},
            //};
            //private static int j = 0;
            internal static IList<PostItem> GetPosts() {
                lock (serverLock) {
                    var client = new BlogService.BlogServiceClient(channel);
                    var response = client.FindAll(new FindAllRequest(), new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    return response.Posts.Select(item => (PostItem)item).ToList();
                }
                //int i = 0;
                //return _posts.Select(post => { post.Abstract = post.Body; post.Id = i++; return post; }).Skip(j++).Take(2).ToArray().ToList();
            }
            internal static PostItem GetPost(int id) {
                lock (serverLock) {
                    var client = new BlogService.BlogServiceClient(channel);
                    var response = client.Find(new FindRequest { PostId = id }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(Configs.ServerTimeout)));
                    return response.Post;
                }
                //int i = 0;
                //return _posts.Select(post => { post.Abstract = post.Body; post.Id = i++; return post; }).First(post => post.Id == id);
            }
        }
    }
}