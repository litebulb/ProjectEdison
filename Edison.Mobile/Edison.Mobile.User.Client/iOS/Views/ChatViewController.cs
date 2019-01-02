using System;
using System.Collections.Generic;
using CoreGraphics;
using Edison.Core.Common.Models;
using Edison.Mobile.iOS.Common.Views;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.DataSources;
using Edison.Mobile.User.Client.iOS.Shared;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class ChatViewController : BaseViewController<ChatViewModel>, IUITextViewDelegate
    {
        readonly float inputHeight = 52;

        UILabel sendMessageLabel;
        UITextView inputTextView;
        UIView borderView;
        ChatMessageTypeCollectionViewSource chatMessageTypeCollectionViewSource;
        UICollectionView messageTypeCollectionView;
        UICollectionView chatCollectionView;
        ChatCollectionViewSource chatCollectionViewSource;
        UIButton sendButton;

        NSLayoutConstraint bottomInputTextViewConstraint;

        NSObject keyboardWillShowNotificationToken;
        NSObject keyboardWillHideNotificationToken;
        NSObject keyboardDidShowNotificationToken;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            sendMessageLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.TwentyFour),
                TextColor = Constants.Color.DarkGray,
                Text = "Send Message",
            };

            View.AddSubview(sendMessageLabel);

            sendMessageLabel.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            sendMessageLabel.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, Constants.Padding).Active = true;
            sendMessageLabel.RightAnchor.ConstraintEqualTo(View.RightAnchor, -Constants.Padding).Active = true;

            sendButton = new UIButton { TranslatesAutoresizingMaskIntoConstraints = false, Enabled = false };

            sendButton.SetAttributedTitle(new NSAttributedString("Send", new UIStringAttributes
            {
                Font = Constants.Fonts.RubikMediumOfSize(Constants.Fonts.Size.Fourteen),
                ForegroundColor = Constants.Color.Blue,
            }), UIControlState.Normal);

            sendButton.SetAttributedTitle(new NSAttributedString("Send", new UIStringAttributes
            {
                Font = Constants.Fonts.RubikMediumOfSize(Constants.Fonts.Size.Fourteen),
                ForegroundColor = Constants.Color.Blue.ColorWithAlpha(0.7f),
            }), UIControlState.Highlighted);

            sendButton.SetAttributedTitle(new NSAttributedString("Send", new UIStringAttributes
            {
                Font = Constants.Fonts.RubikMediumOfSize(Constants.Fonts.Size.Fourteen),
                ForegroundColor = Constants.Color.MidGray.ColorWithAlpha(0.7f),
            }), UIControlState.Disabled);

            View.AddSubview(sendButton);
            sendButton.HeightAnchor.ConstraintEqualTo(inputHeight).Active = true;
            sendButton.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            sendButton.WidthAnchor.ConstraintEqualTo(inputHeight).Active = true;

            inputTextView = new UITextView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen),
                TextColor = Constants.Color.DarkGray,
                Delegate = this,
            };

            View.AddSubview(inputTextView);
            inputTextView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            inputTextView.RightAnchor.ConstraintEqualTo(sendButton.LeftAnchor).Active = true;
            bottomInputTextViewConstraint = inputTextView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor);
            bottomInputTextViewConstraint.Active = true;
            inputTextView.HeightAnchor.ConstraintEqualTo(inputHeight).Active = true;

            sendButton.BottomAnchor.ConstraintEqualTo(inputTextView.BottomAnchor).Active = true;

            borderView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Constants.Color.BackgroundGray,
            };

            View.AddSubview(borderView);
            borderView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            borderView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            borderView.BottomAnchor.ConstraintEqualTo(inputTextView.TopAnchor).Active = true;
            borderView.HeightAnchor.ConstraintEqualTo(1).Active = true;

            var messageTypeCollectionViewFlowLayout = new UICollectionViewFlowLayout
            {
                MinimumLineSpacing = 0,
                MinimumInteritemSpacing = 0,
                EstimatedItemSize = new CGSize(100, Constants.ChatMessageTypeHeight),
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
            };

            chatMessageTypeCollectionViewSource = new ChatMessageTypeCollectionViewSource();

            messageTypeCollectionView = new UICollectionView(CGRect.Empty, messageTypeCollectionViewFlowLayout)
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                Source = chatMessageTypeCollectionViewSource,
                AlwaysBounceHorizontal = true,
            };

            chatMessageTypeCollectionViewSource.CollectionView = new WeakReference<UICollectionView>(messageTypeCollectionView);

            messageTypeCollectionView.RegisterClassForCell(typeof(ChatMessageTypeCollectionViewCell), typeof(ChatMessageTypeCollectionViewCell).Name);

            View.AddSubview(messageTypeCollectionView);
            messageTypeCollectionView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            messageTypeCollectionView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            messageTypeCollectionView.BottomAnchor.ConstraintEqualTo(borderView.TopAnchor).Active = true;
            messageTypeCollectionView.HeightAnchor.ConstraintEqualTo(Constants.ChatMessageTypeHeight).Active = true;

            chatCollectionViewSource = new ChatCollectionViewSource();

            chatCollectionView = new UICollectionView(CGRect.Empty, new ChatCollectionViewLayout())
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Source = chatCollectionViewSource,
                BackgroundView = null,
                BackgroundColor = UIColor.Clear,
                AlwaysBounceVertical = true,
            };

            chatCollectionView.RegisterClassForCell(typeof(ChatMessageCollectionViewCell), typeof(ChatMessageCollectionViewCell).Name);

            View.AddSubview(chatCollectionView);
            chatCollectionView.TopAnchor.ConstraintEqualTo(sendMessageLabel.BottomAnchor).Active = true;
            chatCollectionView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            chatCollectionView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            chatCollectionView.BottomAnchor.ConstraintEqualTo(messageTypeCollectionView.TopAnchor).Active = true;
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();
            keyboardWillShowNotificationToken = UIKeyboard.Notifications.ObserveWillShow(HandleKeyboardWillShow);
            keyboardWillHideNotificationToken = UIKeyboard.Notifications.ObserveWillHide(HandleKeyboardWillHide);
            keyboardDidShowNotificationToken = UIKeyboard.Notifications.ObserveDidShow(HandleKeyboardDidShow);
            chatCollectionViewSource.Messages = ViewModel.ChatMessages;
            chatCollectionViewSource.Initials = ViewModel.Initials;
            chatMessageTypeCollectionViewSource.ActionPlans = ViewModel.ActionPlans;
            chatMessageTypeCollectionViewSource.OnActionPlanSelected += HandleChatMessageTypeCollectionViewSourceOnActionPlanSelected;
            ViewModel.ChatMessages.CollectionChanged += HandleChatMessagesCollectionChanged;
            ViewModel.ActionPlans.CollectionChanged += HandleActionPlansCollectionChanged;
            ViewModel.OnCurrentActionPlanChanged += HandleOnCurrentActionPlanChanged;
            sendButton.TouchUpInside += HandleSendButtonTouchUpInside;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            if (keyboardWillShowNotificationToken != null) NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardWillShowNotificationToken);
            if (keyboardWillHideNotificationToken != null) NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardWillHideNotificationToken);
            if (keyboardDidShowNotificationToken != null) NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardDidShowNotificationToken);
            chatCollectionViewSource.Messages = null;
            chatMessageTypeCollectionViewSource.OnActionPlanSelected -= HandleChatMessageTypeCollectionViewSourceOnActionPlanSelected;
            ViewModel.ChatMessages.CollectionChanged -= HandleChatMessagesCollectionChanged;
            chatMessageTypeCollectionViewSource.ActionPlans = null;
            sendButton.TouchUpInside -= HandleSendButtonTouchUpInside;
        }

        public void LaunchKeyboard()
        {
            inputTextView.BecomeFirstResponder();
        }

        public void ChatSummoned(bool launchKeyboard = false)
        {
            if (launchKeyboard)
            {
                inputTextView.BecomeFirstResponder();
            }

            ViewModel.ChatSummoned();
        }

        public void ChatDismissing()
        {
            inputTextView.ResignFirstResponder();
        }

        public void ChatDismissed()
        {
            ViewModel.ChatDismissed();
        }

        [Export("textView:shouldChangeTextInRange:replacementText:")]
        public bool ShouldChangeText(UITextView textView, NSRange range, string text)
        {
            return true;
        }

        [Export("textViewDidChange:")]
        public void Changed(UITextView textView)
        {
            sendButton.Enabled = textView.HasText;
        }

        void HandleChatMessageTypeCollectionViewSourceOnActionPlanSelected(object sender, ActionPlanSelectedEventArgs e)
        {
            ViewModel.BeginConversationWithActionPlan(e.SelectedActionPlan);
        }

        void HandleOnCurrentActionPlanChanged(object sender, ActionPlanListModel actionPlan)
        {
            chatMessageTypeCollectionViewSource.SelectedActionPlan = actionPlan;
        }

        async void HandleSendButtonTouchUpInside(object sender, EventArgs e)
        {
            if (!inputTextView.HasText) return;

            var text = inputTextView.Text;

            inputTextView.Text = "";

            var success = await ViewModel.SendMessage(text);

            if (!success)
            {
                inputTextView.Text = text;
            }
        }

        void HandleKeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            bottomInputTextViewConstraint.Constant = -e.FrameEnd.Height;

            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(e.AnimationDuration);
            UIView.SetAnimationCurve(e.AnimationCurve);
            UIView.SetAnimationBeginsFromCurrentState(true);
            View.LayoutIfNeeded();
            ScrollChatToBottom(false);
            UIView.CommitAnimations();
        }

        void HandleKeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            bottomInputTextViewConstraint.Constant = 0;

            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(e.AnimationDuration);
            UIView.SetAnimationCurve(e.AnimationCurve);
            UIView.SetAnimationBeginsFromCurrentState(true);
            View.LayoutIfNeeded();
            UIView.CommitAnimations();
        }

        void HandleKeyboardDidShow(object sender, UIKeyboardEventArgs e)
        {

        }

        void HandleActionPlansCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            messageTypeCollectionView.ReloadData();
        }

        void HandleChatMessagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                var indexPaths = new List<NSIndexPath>();
                if (e.NewItems != null)
                {
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        indexPaths.Add(NSIndexPath.FromItemSection(e.NewStartingIndex + i, 0));
                    }

                    chatCollectionView.PerformBatchUpdates(
                        () => chatCollectionView.InsertItems(indexPaths.ToArray()),
                        finished => ScrollChatToBottom()
                    );
                }
                else
                {
                    chatCollectionView.ReloadSections(NSIndexSet.FromIndex(0));
                }
            });
        }

        void ScrollChatToBottom(bool animated = true)
        {
            try
            {
                chatCollectionView.ScrollToItem(NSIndexPath.FromItemSection(ViewModel.ChatMessages.Count - 1, 0), UICollectionViewScrollPosition.Bottom, animated);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e);
            }
        }
    }
}
