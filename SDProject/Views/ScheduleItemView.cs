using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Content.Resources;
using AndroidX.RecyclerView.Widget;
using Grpc.Core;
using SDProject.Extensions;
using SDProject.Types;
using SDProject.Utils;
using Square.Picasso;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Server = SDProject.Utils.Server;

namespace SDProject.Views {
    public class ScheduleItemView : ConstraintLayout, ICallback {
        private ImageView contentBackImageView;
        private TextView fromHourTextView;
        private TextView statusTextView;
        private TextView toHourTextView;

        private int position;

        private ScheduleItem item;

        public int Position { get => position; set => position = value; }

        public ScheduleItem Item { 
            get => item;
            set {
                item = value;
                int level = 1;
                level += (item.UserId != 0) ? 1 : 0;
                level += (item.UserId == Database.UserId) ? 1 : 0;
                fromHourTextView.Text = $"از\n{item.Start:HH:mm}";
                toHourTextView.Text = $"تا\n{item.End:HH:mm}";
                statusTextView.Text = level == 1 ? "خالی" : (level == 2 ? "پر" : "وقت شما");
                statusTextView.SetTextColor(ColorStateList.ValueOf(Color.ParseColor(Context.GetString(level == 1 ? Resource.Color.Primary : (level == 2 ? Resource.Color.Error : Resource.Color.Success)))));
                contentBackImageView.SetImageLevel(level);
            }
        }

        public ScheduleItemView(Context context) : base(context) {
            Initialize(context);
        }
        public ScheduleItemView(Context context, IAttributeSet attrs) : base(context, attrs) {
            Initialize(context, attrs);
        }

        public virtual void Initialize(Context context) {
            Inflate(Publics.ApplicationContext, Resource.Layout.list_item_schedule, this);

            contentBackImageView = FindViewById<ImageView>(Resource.Id.contentBackImageView);
            fromHourTextView = FindViewById<TextView>(Resource.Id.fromHourTextView);
            statusTextView = FindViewById<TextView>(Resource.Id.statusTextView);
            toHourTextView = FindViewById<TextView>(Resource.Id.toHourTextView);

            Typeface normalFontTypeface = ResourcesCompat.GetFont(Context, Configs.AppNormalFontRes);
            Typeface boldFontTypeface = ResourcesCompat.GetFont(Context, Configs.AppBoldFontRes);

            statusTextView.Typeface = boldFontTypeface;
            fromHourTextView.Typeface = normalFontTypeface;
            toHourTextView.Typeface = normalFontTypeface;
        }
        public virtual void Initialize(Context context, IAttributeSet attrs) {
            Initialize(context);
        }

        public void OnError(Java.Lang.Exception exception) { }

        public void OnSuccess() { }
    }

    internal class SchedulesViewAdapter : RecyclerView.Adapter {
        private readonly Context context;
        public override int ItemCount => Items.Count;
        public IList<ScheduleItem> Items;

        public SchedulesViewAdapter(Context context) {
            this.context = context;
            Items = new List<ScheduleItem>();
        }

        public SchedulesViewAdapter(Context context, IList<ScheduleItem> items) {
            this.context = context;
            Items = items.ToArray().ToList();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
            var view = new ScheduleItemView(parent.Context);

            view.Click += Schedule;

            return new ScheduleItemViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            ScheduleItemViewHolder viewHolder = holder as ScheduleItemViewHolder;
            var Item = Items[position];

            viewHolder.Position = position;
            viewHolder.Item = Item;
        }

        private void Schedule(object sender, System.EventArgs e) {
            ScheduleItemView view = sender as ScheduleItemView;
            var item = Items[view.Position];
            if (item.UserId == 0) {
                Tools.ShowMessage($"گرفتن وقت روز {item.Start:ddd dd MMM} از {item.Start:HH:mm} تا {item.End:HH:mm}{(item.Start.Day == item.End.Day ? "" : " روز بعد")}؟", Configs.DurationForever, "بله", (title, message) => {
                    try {
                        Server.Counseller.Reserve(item.CounsellerId, item.Start);
                        item.UserId = Database.UserId;
                        Items[view.Position] = item;
                        NotifyItemChanged(view.Position);
                    }
                    catch (RpcException ex) {
                        Tools.ShowError(ex.GetMessage(), Configs.ErrorDuration, (Activity)context);
                    }
                    catch {
                        Tools.ShowError("خطای داخلی برنامه", Configs.ErrorDuration, (Activity)context);
                    }
                }, (Activity)context);
            }
            else if (item.UserId == Database.UserId) {
                Tools.ShowError("شما این وقت را قبلا گرفته اید", Configs.ErrorDuration, (Activity)context);
            }
            else {
                Tools.ShowError("امکان گرفتن این وقت وجود ندارد", Configs.ErrorDuration, (Activity)context);
            }
        }

        public override void OnViewAttachedToWindow(Java.Lang.Object holder) {
            base.OnViewAttachedToWindow(holder);
            ScheduleItemViewHolder viewHolder = holder as ScheduleItemViewHolder;
            MainThread.BeginInvokeOnMainThread(() => {
                viewHolder.View.LayoutParameters.Width = ViewGroup.LayoutParams.MatchParent;
                //((ViewGroup.MarginLayoutParams)viewHolder.View.LayoutParameters).TopMargin = 0;
            });
        }
    }

    public class ScheduleItemViewHolder : RecyclerView.ViewHolder {
        public ScheduleItemView View;
        public ScheduleItem Item {
            get => View.Item;
            set => View.Item = value;
        }
        public new int Position {
            get => View.Position;
            set => View.Position = value;
        }
        public ScheduleItemViewHolder(ScheduleItemView itemView) : base(itemView) {
            View = itemView;
        }
    }

}