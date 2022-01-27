using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Content.Resources;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.ImageView;
using SDProject.Extensions;
using SDProject.Types;
using SDProject.Utils;
using Square.Picasso;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace SDProject.Views {
    public class PostItemView : ConstraintLayout, ICallback {
        private TextView postTitleTextView;
        private View titleAbstractBetweenSpaceView;
        private TextView postAbstractTextView;
        private ShapeableImageView postImageView;

        private int position;

        private PostItem item;

        public int Position { get => position; set => position = value; }

        public PostItem Item { 
            get => item;
            set {
                item = value;
                postTitleTextView.Visibility = item.Title.IsEmpty() ? ViewStates.Gone : ViewStates.Visible;
                postTitleTextView.Text = item.Title;
                postAbstractTextView.Visibility = item.Abstract.IsEmpty() ? ViewStates.Gone : ViewStates.Visible;
                postAbstractTextView.Text = item.Abstract;
                titleAbstractBetweenSpaceView.Visibility = (item.Title.IsEmpty() || item.Abstract.IsEmpty()) ? ViewStates.Gone : ViewStates.Visible;
                if (item.ImageUrl.IsEmpty()) {
                    postImageView.SetImageResource(Resource.Drawable.back_list_item);
                    postImageView.Visibility = ViewStates.Gone;
                }
                else {
                    Picasso.Get().Load(item.ImageUrl).Into(postImageView, this);
                    postImageView.Visibility = ViewStates.Visible;
                }
            }
        }

        public PostItemView(Context context) : base(context) {
            Initialize(context);
        }
        public PostItemView(Context context, IAttributeSet attrs) : base(context, attrs) {
            Initialize(context, attrs);
        }

        public virtual void Initialize(Context context) {
            Inflate(Publics.ApplicationContext, Resource.Layout.list_item_post, this);

            postTitleTextView = FindViewById<TextView>(Resource.Id.postTitleTextView);
            titleAbstractBetweenSpaceView = FindViewById<View>(Resource.Id.titleAbstractBetweenSpaceView);
            postAbstractTextView = FindViewById<TextView>(Resource.Id.postAbstractTextView);
            postImageView = FindViewById<ShapeableImageView>(Resource.Id.postImageView);

            Typeface normalFontTypeface = ResourcesCompat.GetFont(Context, Configs.AppNormalFontRes);
            Typeface boldFontTypeface = ResourcesCompat.GetFont(Context, Configs.AppBoldFontRes);

            postTitleTextView.Typeface = boldFontTypeface;
            postAbstractTextView.Typeface = normalFontTypeface;
        }
        public virtual void Initialize(Context context, IAttributeSet attrs) {
            Initialize(context);
        }

        public void OnError(Java.Lang.Exception exception) { }

        public void OnSuccess() { }
    }

    internal class PostsViewAdapter : RecyclerView.Adapter {
        private readonly Context context;
        public override int ItemCount => Items.Count;
        public IList<PostItem> Items;

        public PostsViewAdapter(Context context) {
            this.context = context;
            Items = new List<PostItem>();
        }

        public PostsViewAdapter(Context context, IList<PostItem> items) {
            this.context = context;
            Items = items.ToArray().ToList();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
            var view = new PostItemView(parent.Context);

            view.Click += OpenPost;

            return new PostItemViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            PostItemViewHolder viewHolder = holder as PostItemViewHolder;
            var Item = Items[position];

            viewHolder.Position = position;
            viewHolder.Item = Item;
        }

        private void OpenPost(object sender, System.EventArgs e) {
            PostItemView view = sender as PostItemView;
            Intent intent = new Intent(context, typeof(ReadingPostActivity));
            var extras = new Android.OS.Bundle();
            extras.PutInt("PostId", Items[view.Position].Id);
            intent.PutExtras(extras);
            context.StartActivity(intent);
        }

        public override void OnViewAttachedToWindow(Java.Lang.Object holder) {
            base.OnViewAttachedToWindow(holder);
            PostItemViewHolder viewHolder = holder as PostItemViewHolder;
            MainThread.BeginInvokeOnMainThread(() => {
                viewHolder.View.LayoutParameters.Width = ViewGroup.LayoutParams.MatchParent;
                if (viewHolder.Position == 0) {
                    ((ViewGroup.MarginLayoutParams)viewHolder.View.LayoutParameters).TopMargin = context.Resources.GetDimensionPixelSize(Resource.Dimension.waves_height);
                }
                else {
                    ((ViewGroup.MarginLayoutParams)viewHolder.View.LayoutParameters).TopMargin = 0;
                }
            });
        }
    }

    public class PostItemViewHolder : RecyclerView.ViewHolder {
        public PostItemView View;
        public PostItem Item {
            get => View.Item;
            set => View.Item = value;
        }
        public new int Position {
            get => View.Position;
            set => View.Position = value;
        }
        public PostItemViewHolder(PostItemView itemView) : base(itemView) {
            View = itemView;
        }
    }

}