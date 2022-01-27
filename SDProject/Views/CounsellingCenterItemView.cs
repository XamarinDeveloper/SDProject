using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
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
using static Android.Manifest;

namespace SDProject.Views {
    public class CounsellingCenterItemView : ConstraintLayout, ICallback {
        private TextView centerNameTextView;
        private TextView centerLandlineTextView;
        private TextView centerWebsiteTextView;
        private TextView centerAddressTextView;
        private int position;

        private CounsellingCenterItem item;

        public int Position { get => position; set => position = value; }

        public CounsellingCenterItem Item {
            get => item;
            set {
                item = value;
                centerNameTextView.Visibility = item.Name.IsEmpty() ? ViewStates.Gone : ViewStates.Visible;
                centerNameTextView.Text = item.Name;
                centerLandlineTextView.Visibility = item.Landline.IsEmpty() ? ViewStates.Gone : ViewStates.Visible;
                centerLandlineTextView.Text = item.Landline;
                centerWebsiteTextView.Visibility = item.Website.IsEmpty() ? ViewStates.Gone : ViewStates.Visible;
                centerWebsiteTextView.Text = item.Website;
                centerAddressTextView.Visibility = item.Address.IsEmpty() ? ViewStates.Gone : ViewStates.Visible;
                centerAddressTextView.Text = item.Address;
            }
        }

        public CounsellingCenterItemView(Context context) : base(context) {
            Initialize(context);
        }
        public CounsellingCenterItemView(Context context, IAttributeSet attrs) : base(context, attrs) {
            Initialize(context, attrs);
        }

        public virtual void Initialize(Context context) {
            Inflate(Publics.ApplicationContext, Resource.Layout.list_item_counselling_center, this);

            centerNameTextView = FindViewById<TextView>(Resource.Id.centerNameTextView);
            centerLandlineTextView = FindViewById<TextView>(Resource.Id.centerLandlineTextView);
            centerWebsiteTextView = FindViewById<TextView>(Resource.Id.centerWebsiteTextView);
            centerAddressTextView = FindViewById<TextView>(Resource.Id.centerAddressTextView);

            Typeface normalFontTypeface = ResourcesCompat.GetFont(Context, Configs.AppNormalFontRes);
            Typeface boldFontTypeface = ResourcesCompat.GetFont(Context, Configs.AppBoldFontRes);

            centerNameTextView.Typeface = boldFontTypeface;
            centerLandlineTextView.Typeface = normalFontTypeface;
            centerWebsiteTextView.Typeface = normalFontTypeface;
            centerAddressTextView.Typeface = normalFontTypeface;
        }
        public virtual void Initialize(Context context, IAttributeSet attrs) {
            Initialize(context);
        }

        public void OnError(Java.Lang.Exception exception) { }

        public void OnSuccess() { }
    }

    internal class CounsellingCentersViewAdapter : RecyclerView.Adapter {
        private readonly Context context;
        public override int ItemCount => Items.Count;
        public IList<CounsellingCenterItem> Items;

        public CounsellingCentersViewAdapter(Context context) {
            this.context = context;
            Items = new List<CounsellingCenterItem>();
        }

        public CounsellingCentersViewAdapter(Context context, IList<CounsellingCenterItem> items) {
            this.context = context;
            Items = items.ToArray().ToList();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
            var view = new CounsellingCenterItemView(parent.Context);

            view.Click += ShowCallingMessage;

            return new CounsellingCenterItemViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            CounsellingCenterItemViewHolder viewHolder = holder as CounsellingCenterItemViewHolder;
            var Item = Items[position];

            viewHolder.Position = position;
            viewHolder.Item = Item;
        }

        private void ShowCallingMessage(object sender, System.EventArgs e) {
            CounsellingCenterItemView view = sender as CounsellingCenterItemView;
            Tools.ShowMessage($"تماس با {Items[view.Position].Name}؟", Configs.DurationForever, "بله", (title, message) => {
                if (Tools.HasCallPermission()) {
                    Intent callIntent = new Intent(Intent.ActionCall);
                    callIntent.SetData(Uri.Parse("tel:" + Items[view.Position].Landline.Replace("-", "")));
                    context.StartActivity(callIntent);
                }
                else {
                    ((XActivity)context).RequestPermissions(new[] { Permission.CallPhone });
                }
            }, (Activity)context);
        }

        public override void OnViewAttachedToWindow(Java.Lang.Object holder) {
            base.OnViewAttachedToWindow(holder);
            CounsellingCenterItemViewHolder viewHolder = holder as CounsellingCenterItemViewHolder;
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

    public class CounsellingCenterItemViewHolder : RecyclerView.ViewHolder {
        public CounsellingCenterItemView View;
        public CounsellingCenterItem Item {
            get => View.Item;
            set => View.Item = value;
        }
        public new int Position {
            get => View.Position;
            set => View.Position = value;
        }
        public CounsellingCenterItemViewHolder(CounsellingCenterItemView itemView) : base(itemView) {
            View = itemView;
        }
    }
}