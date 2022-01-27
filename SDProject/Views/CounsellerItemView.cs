using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Content.Resources;
using AndroidX.RecyclerView.Widget;
using SDProject.Extensions;
using SDProject.Types;
using SDProject.Utils;
using Square.Picasso;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace SDProject.Views {
    public class CounsellerItemView : ConstraintLayout, ICallback {
        private TextView counsellerNameTextView;
        private TextView counsellerPhoneNumberTextView;
        private TextView counsellerDescriptionTextView;
        private int position;

        private CounsellerItem item;

        public int Position { get => position; set => position = value; }

        public CounsellerItem Item {
            get => item;
            set {
                item = value;
                counsellerNameTextView.Visibility = item.Name.IsEmpty() ? ViewStates.Gone : ViewStates.Visible;
                counsellerNameTextView.Text = item.Name;
                counsellerPhoneNumberTextView.Visibility = item.PhoneNumber.IsEmpty() ? ViewStates.Gone : ViewStates.Visible;
                counsellerPhoneNumberTextView.Text = item.PhoneNumber;
                counsellerDescriptionTextView.Visibility = item.Description.IsEmpty() ? ViewStates.Gone : ViewStates.Visible;
                counsellerDescriptionTextView.Text = item.Description;
            }
        }

        public CounsellerItemView(Context context) : base(context) {
            Initialize(context);
        }
        public CounsellerItemView(Context context, IAttributeSet attrs) : base(context, attrs) {
            Initialize(context, attrs);
        }

        public virtual void Initialize(Context context) {
            Inflate(Publics.ApplicationContext, Resource.Layout.list_item_counseller, this);

            counsellerNameTextView = FindViewById<TextView>(Resource.Id.counsellerNameTextView);
            counsellerPhoneNumberTextView = FindViewById<TextView>(Resource.Id.counsellerPhoneNumberTextView);
            counsellerDescriptionTextView = FindViewById<TextView>(Resource.Id.counsellerDescriptionTextView);

            Typeface normalFontTypeface = ResourcesCompat.GetFont(Context, Configs.AppNormalFontRes);
            Typeface boldFontTypeface = ResourcesCompat.GetFont(Context, Configs.AppBoldFontRes);

            counsellerNameTextView.Typeface = boldFontTypeface;
            counsellerPhoneNumberTextView.Typeface = normalFontTypeface;
            counsellerDescriptionTextView.Typeface = normalFontTypeface;
        }
        public virtual void Initialize(Context context, IAttributeSet attrs) {
            Initialize(context);
        }

        public void OnError(Java.Lang.Exception exception) { }

        public void OnSuccess() { }
    }

    internal class CounsellersViewAdapter : RecyclerView.Adapter {
        private readonly Context context;
        public override int ItemCount => Items.Count;
        public IList<CounsellerItem> Items;

        public CounsellersViewAdapter(Context context) {
            this.context = context;
            Items = new List<CounsellerItem>();
        }

        public CounsellersViewAdapter(Context context, IList<CounsellerItem> items) {
            this.context = context;
            Items = items.ToArray().ToList();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
            var view = new CounsellerItemView(parent.Context);

            view.Click += OpenCounseller;

            return new CounsellerItemViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            CounsellerItemViewHolder viewHolder = holder as CounsellerItemViewHolder;
            var Item = Items[position];

            viewHolder.Position = position;
            viewHolder.Item = Item;
        }

        private void OpenCounseller(object sender, System.EventArgs e) {
            CounsellerItemView view = sender as CounsellerItemView;
            Intent intent = new Intent(context, typeof(OnlineCounsellerActivity));
            var extras = new Android.OS.Bundle();
            extras.PutInt("CounsellerId", Items[view.Position].Id);
            intent.PutExtras(extras);
            context.StartActivity(intent);
        }

        public override void OnViewAttachedToWindow(Java.Lang.Object holder) {
            base.OnViewAttachedToWindow(holder);
            CounsellerItemViewHolder viewHolder = holder as CounsellerItemViewHolder;
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

    public class CounsellerItemViewHolder : RecyclerView.ViewHolder {
        public CounsellerItemView View;
        public CounsellerItem Item {
            get => View.Item;
            set => View.Item = value;
        }
        public new int Position {
            get => View.Position;
            set => View.Position = value;
        }
        public CounsellerItemViewHolder(CounsellerItemView itemView) : base(itemView) {
            View = itemView;
        }
    }
}