using System.Collections.Generic;

using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Core.ViewModels;
using System;
using Edison.Mobile.Android.Common.Controls;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Content.Res;
using Edison.Mobile.User.Client.Core.Shared;
using Android.Support.Constraints;
using Android.Views.InputMethods;
using Edison.Mobile.User.Client.Droid.Activities;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Graphics.Drawable;
using System.Collections.ObjectModel;
using Edison.Core.Common.Models;
using Edison.Mobile.User.Client.Droid.Adapters;
using Android.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Edison.Mobile.User.Client.Droid.Fragments
{
    public class ChatFragment : BaseFragment<ChatViewModel>
    {

        private LinearLayout _quick_chat_holder;
        private ConstraintLayout _chat_layout_holder;
        private RecyclerView _eventButtons;
 //       private RecyclerView.LayoutManager _eventButtonsLayoutManager;
        private EventButtonsAdapter _eventButtonsAdapter;
        private RecyclerView _chatMessages;
 //       private RecyclerView.LayoutManager _chatMessagesLayoutManager;
        private ChatAdapter _chatAdapter;


        private List<Tuple<CircularImageButton, AppCompatTextView>> _quickButtons = new List<Tuple<CircularImageButton, AppCompatTextView>>();
        private LinearLayout _safeButtonHolder;


        private ConstraintLayout _chatInputHolder;
        private AppCompatEditText _chatMessageInput;
        private CircularImageButton _chatMessageSendButton;

        private bool _safeButtonSelected = false;
        private Color _origSafeButtonColor;
        private Color _selectedSafeButtonColor;
        private Color _origSafeIconColor;
        private Color _selectedSafeIconColor;

        private int _parentId;


        public ChatFragment(int parentId) : base()
        {
            _parentId = parentId;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(Resource.Layout.chat_fragment, container, false);

            BindViews(root);
            BindData();
            AdjustViewPositions();
            BindEvents();

            _origSafeButtonColor = new Color(ContextCompat.GetColor(Context, Resource.Color.icon_background_grey));
            _selectedSafeButtonColor = new Color(ContextCompat.GetColor(Context, Resource.Color.app_green));
            _origSafeIconColor = new Color(ContextCompat.GetColor(Context, Resource.Color.icon_blue));
            _selectedSafeIconColor = new Color(ContextCompat.GetColor(Context, Resource.Color.white));

            return root;
        }


        private void BindViews(View root)
        {
            _quick_chat_holder = root.FindViewById<LinearLayout>(Resource.Id.quick_chat_holder);
            _chat_layout_holder = root.FindViewById<ConstraintLayout>(Resource.Id.chat_layout_holder);

            _quickButtons.Add(new Tuple<CircularImageButton, AppCompatTextView>(root.FindViewById<CircularImageButton>(Resource.Id.qc_emergency), root.FindViewById<AppCompatTextView>(Resource.Id.qc_emergency_name)));
            _quickButtons.Add(new Tuple<CircularImageButton, AppCompatTextView>(root.FindViewById<CircularImageButton>(Resource.Id.qc_activity), root.FindViewById<AppCompatTextView>(Resource.Id.qc_activity_name)));
            _quickButtons.Add(new Tuple<CircularImageButton, AppCompatTextView>(root.FindViewById<CircularImageButton>(Resource.Id.qc_safe), root.FindViewById<AppCompatTextView>(Resource.Id.qc_safe_name)));
            _safeButtonHolder = root.FindViewById<LinearLayout>(Resource.Id.qc_safe_holder);
            // if the fragment is contained in an EventDetailActivity, initially make labels invisible by setting alpha to 0
            if (_parentId == Resource.Layout.event_detail_activity)
            {
                foreach (var button in _quickButtons)
                {
                    button.Item2.Alpha = 0;
                }
            }

            _chatInputHolder = root.FindViewById<ConstraintLayout>(Resource.Id.chat_input_holder);
            _chatMessageInput = root.FindViewById<AppCompatEditText>(Resource.Id.chat_input);
            _chatMessageSendButton = root.FindViewById<CircularImageButton>(Resource.Id.send_button);

            _eventButtons = root.FindViewById<RecyclerView>(Resource.Id.chat_button_list);
            _chatMessages = root.FindViewById<RecyclerView>(Resource.Id.chat_area);
        }

        private void AdjustViewPositions()
        {
            if (Constants.QuickChatIconButtonDiameterPx > -1)
            {
                foreach (var button in _quickButtons)
                {
                    var padding = _parentId == Resource.Layout.main_activity ? Constants.QuickChatIconButtonPaddingPx : Constants.QuickChatSmallIconButtonPaddingPx;
                    button.Item1.SetIconPadding(padding);
                    var size = _parentId == Resource.Layout.main_activity ? Constants.QuickChatIconButtonDiameterPx : Constants.QuickChatSmallIconButtonDiameterPx;
                    var lp = button.Item1.LayoutParameters;
                    lp.Height = size;
                    lp.Width = size;
                    button.Item1.LayoutParameters = lp;
                }
            }
        }

        private void BindEvents()
        {
            foreach (var button in _quickButtons)
            {
                button.Item1.Click += OnButtonClick;
            }
            ViewModel.ChatPromptTypes.CollectionChanged += OnChatPromptTypesCollectionChanged;
            if (Activity is MainActivity act)
            {
                act.BottomSheetBehaviour.Slide += OnSlide;
                act.KeyboardStatusChanged += OnKeyboardChange;
            }
            else if (Activity is EventDetailActivity act1)
            {
                act1.BottomSheetBehaviour.Slide += OnSlide;
                act1.KeyboardStatusChanged += OnKeyboardChange;
            }
            

            ViewModel.ActionPlans.CollectionChanged += OnEventButtonCollectionChanged;
            _eventButtonsAdapter.ItemClick += OnButtonClick;

            ViewModel.ChatMessages.CollectionChanged += OnMessagesCollectionChanged;
            _chatMessageInput.TextChanged += OnTextChanged;
            _chatMessageSendButton.Click += OnSendClicked;
        }

        private void UnbindEvents()
        {
            foreach (var button in _quickButtons)
            {
                button.Item1.Click -= OnButtonClick;
            }
            ViewModel.ChatPromptTypes.CollectionChanged -= OnChatPromptTypesCollectionChanged;
            if (Activity is MainActivity act)
            {
                act.BottomSheetBehaviour.Slide -= OnSlide;
                act.KeyboardStatusChanged -= OnKeyboardChange;
            }
            else if (Activity is EventDetailActivity act1)
            {
                act1.BottomSheetBehaviour.Slide -= OnSlide;
                act1.KeyboardStatusChanged -= OnKeyboardChange;
            }

            ViewModel.ActionPlans.CollectionChanged -= OnEventButtonCollectionChanged;
            _eventButtonsAdapter.ItemClick -= OnButtonClick;

            ViewModel.ChatMessages.CollectionChanged -= OnMessagesCollectionChanged;
        }

        private void BindData()
        {
            _eventButtonsAdapter = new EventButtonsAdapter(Context, ViewModel.ActionPlans);
            _eventButtons.SetAdapter(_eventButtonsAdapter);
            _eventButtons.HasFixedSize = true;
            if (ViewModel.CurrentActionPlan != null)
                _eventButtonsAdapter.SelectedPosition = ViewModel.ActionPlans.IndexOf(ViewModel.CurrentActionPlan);

            _chatAdapter = new ChatAdapter(Context, ViewModel.ChatMessages)
            {
                Initials = ViewModel.Initials,
                ProfileImageUri = ViewModel.ProfileImageUri,
                Email = ViewModel.Email
            };
            _chatMessages.AddItemDecoration(new SpaceItemDecoration(Activity.Resources.GetDimensionPixelSize(Resource.Dimension.chat_item_spacing)));
            _chatMessages.SetAdapter(_chatAdapter);


            if (Activity is MainActivity act)
                act.BottomSheetBehaviour.NestedScrollingViewIds.Add(Resource.Id.chat_area);
            else if (Activity is EventDetailActivity act1)
                act1.BottomSheetBehaviour.NestedScrollingViewIds.Add(Resource.Id.chat_area);
        }


        public override void OnDestroyView()
        {
            UnbindEvents();
            base.OnDestroyView();
        }



        private void OnSlide(object s, float slideOffset)
        {
            if (slideOffset >= 0 && slideOffset <= 1)
            {
                if (_parentId == Resource.Layout.main_activity)
                {
                    // Main Activity - simple linear fading based on bottom sheet position
                    _quick_chat_holder.Alpha = 1 - slideOffset;
                    _chat_layout_holder.Alpha = slideOffset;
                }
                else if (_parentId == Resource.Layout.event_detail_activity)
                {
                    // EventDetails Activity
                    // Calculate two offset thresholds on which view behaviour is based
                    var threshold2 = (float)(Constants.BottomSheetPeekHeightPx - Constants.BottomSheetSmallPeekHeightPx)/(float)Constants.AvailableDetailBottomSheetHeightPx;
                    var threshold1 = threshold2/2;
                    // Linear fading based on bottom sheet position above threshold 2 position
                    _quick_chat_holder.Alpha = slideOffset <= threshold2 ? 1 : 1 - ((slideOffset - threshold2) / (1 - threshold2));
                    _chat_layout_holder.Alpha = slideOffset <= threshold2 ? 0 : (slideOffset - threshold2) / (1 - threshold2);

                    foreach (var button in _quickButtons)
                    {
                        // QuickChatIconButtonDiameterPx
                        // QuickChatSmallIconButtonDiameterPx
                        // button icon padding based on bottom sheet position between closed and threshold 2 position
                        var paddingDelta = Constants.QuickChatIconButtonPaddingPx - Constants.QuickChatSmallIconButtonPaddingPx;
                        var padding = slideOffset >= threshold2 ? Constants.QuickChatIconButtonPaddingPx : (int)(Constants.QuickChatSmallIconButtonPaddingPx + (paddingDelta * slideOffset / threshold2));
                        button.Item1.SetIconPadding(padding);
                        // button icon size based on bottom sheet position between closed and threshold 2 position
                        var sizeDelta = Constants.QuickChatIconButtonDiameterPx - Constants.QuickChatSmallIconButtonDiameterPx;
                        var size = slideOffset >= threshold2 ? Constants.QuickChatIconButtonDiameterPx : (int)(Constants.QuickChatSmallIconButtonDiameterPx + (sizeDelta * slideOffset / threshold2));
                        var lp = button.Item1.LayoutParameters;
                        lp.Width = size;
                        lp.Height = size;
                        button.Item1.LayoutParameters = lp;

                        // button label alpha based on bottom sheet position between threshold 1 and threshold 2 positions
                        button.Item2.Alpha = slideOffset <= threshold1 ? 0 : slideOffset >= threshold2 ? 1 : (slideOffset - threshold1) / (threshold2 - threshold1);
                    }

                }



            }
        }


        private void OnChatPromptTypesCollectionChanged(object sender, EventArgs e)
        {
            if (ViewModel.ChatPromptTypes.Contains(ChatPromptType.SafetyCheck))
                _safeButtonHolder.Visibility = ViewStates.Visible;
            else
                _safeButtonHolder.Visibility = ViewStates.Gone;
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            if (sender is CircularImageButton imgButton && Activity != null)
            {

                ChatPromptType cpt = ChatPromptType.ReportActivity;
                switch ((string)imgButton.Tag)
                {
                    case "qc_safe":
                        // Change the color of the button
                        _safeButtonSelected = !_safeButtonSelected;
                        imgButton.SetBackgroundTint(_safeButtonSelected ? _selectedSafeButtonColor: _origSafeButtonColor);
                        imgButton.SetIconResource(_safeButtonSelected ? Resource.Drawable.personal_check : Resource.Drawable.user);
                        var iconCsl = ColorStateList.ValueOf(_safeButtonSelected ? _selectedSafeIconColor: _origSafeIconColor);
                        imgButton.IconTint = iconCsl;
                        cpt = ChatPromptType.SafetyCheck;
                        break;

                    case "qc_activity":
                        if (Activity is MainActivity act)
                            act.BottomSheetBehaviour.State = BottomSheetBehavior.StateExpanded;
                        else if (Activity is EventDetailActivity act1)
                            act1.BottomSheetBehaviour.State = BottomSheetBehavior.StateExpanded;
                            // inject message into chat
                            cpt = ChatPromptType.ReportActivity;
                        

                        break;

                    case "qc_emergency":
                        if (Activity is MainActivity act2)
                            act2.BottomSheetBehaviour.State = BottomSheetBehavior.StateExpanded;
                        else if (Activity is EventDetailActivity act3)
                            act3.BottomSheetBehaviour.State = BottomSheetBehavior.StateExpanded;
                        // inject message into chat
                        cpt = ChatPromptType.Emergency;

                        // set the selected button to emergency
                        var button = _eventButtonsAdapter.EventButtons.FirstOrDefault(b => b.Name.ToLowerInvariant() == "emergency");
                        var index = button == null ? -1 : _eventButtonsAdapter.EventButtons.IndexOf(button);
                        _eventButtonsAdapter.SelectedPosition = index;

                        break;

                }

                await ViewModel.ActivateChatPrompt(cpt);
            }

        }

        private async void OnButtonClick(object sender, int position)
        {
            if (position < ViewModel.ActionPlans.Count)
            {
                var actionPlan = ViewModel.ActionPlans[position];
                await ViewModel.BeginConversationWithActionPlanAsync(actionPlan);
            }

        }

        private async void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.AfterCount == 0)
                _chatMessageSendButton.Visibility = ViewStates.Invisible;
            else
                _chatMessageSendButton.Visibility = ViewStates.Visible;
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            //send the contents of the edit text
            var msg = _chatMessageInput.Text;
            // if text is empty do nothing
            if (string.IsNullOrWhiteSpace(msg)) return;

            // clear text field
            _chatMessageInput.Text = "";
            KeyboardStatusService.DismissKeyboard(Activity, _chatMessageInput);
            // send message
            var success = await ViewModel.SendMessage(msg);
            // if not sent reinstate text field contents
            if (!success)
                _chatMessageInput.Text = msg;
        }


        private int __chatHolderHeight = -1;
        private int _chatHolderHeight
        {
            get
            {
                if (__chatHolderHeight == -1)
                    __chatHolderHeight = _chat_layout_holder.Height;
                return __chatHolderHeight;
            }
        }


        private void OnKeyboardChange(KeyboardStatusChangeEventArgs e)
        {
            var keyboardStatus = e.Status;

            if (keyboardStatus == KeyboardStatus.Open)
            {
                var heightDelta = e.VisibleHeightToDecorHeightDelta;
                // need to adjust keyboard height calculation based upon the prescribed adjustment for the Activity set as  WindowSoftInputMode
                // the presence of a toolbar and.or status bar
                var keyboardHeight = heightDelta - Constants.ToolbarHeightPx - Constants.StatusBarHeightPx;

                // Disable the Bottom sheet
                if (Activity is MainActivity act)
                    act.BottomSheetBehaviour.Enabled = false;
                else if (Activity is EventDetailActivity act1)
                    act1.BottomSheetBehaviour.Enabled = false;

                // Change the height of the containing view to take into account the keyboard
                var lp = _chat_layout_holder.LayoutParameters;
                lp.Height = _chatHolderHeight - keyboardHeight;
                var lm = (LinearLayoutManager)_chatMessages.GetLayoutManager();
                Activity.RunOnUiThread(() => {
                    _chat_layout_holder.LayoutParameters = lp;
                    lm.ScrollToPositionWithOffset(_chatAdapter.ItemCount - 1, 0);
                });
            }
            else
            {
                // Enable the Bottom sheet
                if (Activity is MainActivity act)
                    act.BottomSheetBehaviour.Enabled = true;
                else if (Activity is EventDetailActivity act1)
                    act1.BottomSheetBehaviour.Enabled = true;

                // Reset the height of the containing view to take into account the keyboard
                var lp = _chat_layout_holder.LayoutParameters;
                lp.Height = _chatHolderHeight;
                var lm = (LinearLayoutManager)_chatMessages.GetLayoutManager();
                Activity.RunOnUiThread(() => {
                    _chat_layout_holder.LayoutParameters = lp;
                    lm.ScrollToPositionWithOffset(_chatAdapter.ItemCount - 1, 0);
                });
            }

        }

        private void OnEventButtonCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _eventButtonsAdapter.NotifyDataSetChanged();
        }
        private async void OnMessagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var action = e.Action;
            if (action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                int index = e.NewStartingIndex;
                Activity.RunOnUiThread(() =>
                {
                    _chatAdapter.NotifyItemRangeInserted(e.NewStartingIndex, e.NewItems.Count);
                });
            }
            else
                Activity.RunOnUiThread(() => { _chatAdapter.NotifyDataSetChanged(); });
            await UpdatePosition();
        }


        private async Task UpdatePosition()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(200);
                var lm = (LinearLayoutManager)_chatMessages.GetLayoutManager();
                Activity.RunOnUiThread(() =>
                {
                    lm.ScrollToPositionWithOffset(_chatAdapter.ItemCount - 1, 0);
                });
            }).ConfigureAwait(false);
        }



    }
}