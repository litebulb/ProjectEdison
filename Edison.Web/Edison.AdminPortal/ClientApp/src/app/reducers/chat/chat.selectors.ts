import { createFeatureSelector, createSelector } from '@ngrx/store';

import { AppState } from '../';
import { Chat } from './chat.model';
import { selectAll, State } from './chat.reducer';
import { authUserSelector } from '../auth/auth.selectors';

export const chatStateSelector = createFeatureSelector<AppState, State>('chat');

export const chatTokenSelector = createSelector(
  chatStateSelector,
  authUserSelector,
  (state, user) => ({
    token: state.token,
    user,
  })
);

export const chatMessagesSelector = createSelector(
  chatStateSelector,
  state =>
    selectAll(state).filter(
      chat =>
        chat.channelData && chat.channelData.data && chat.channelData.data.from
    )
);

export const chatActiveUserIdSelector = createSelector(
  chatStateSelector,
  state => state.activeUserId
);

export const chatActiveUserSelector = createSelector(
  chatStateSelector,
  state => ({ name: state.activeUserName, userId: state.activeUserId })
);

export const chatShowUserChatSelector = createSelector(
  chatActiveUserIdSelector,
  userId => userId && userId !== '*'
);

export const chatShowAllChatSelector = createSelector(
  chatActiveUserIdSelector,
  userId => userId === '*'
);

export const chatActiveConversationIdSelector = createSelector(
  chatStateSelector,
  state => state.activeConversationId
);

export const chatActiveUsersSelector = createSelector(
  chatMessagesSelector,
  messages => {
    const users = messages
      .filter(
        chat => chat.channelData.data.from.role.toLowerCase() === 'consumer'
      )
      .map(chat => ({
        id: chat.channelData.data.from.id,
        conversationId: chat.conversation.id,
        name: chat.channelData.data.from.name,
        reportType: chat.channelData.data.reportType,
      }))
      .filter(
        (chat, index, self) => self.findIndex(c => c.id === chat.id) === index
      );

    return users;
  }
);

export const chatActiveUsersCountSelector = createSelector(
  chatActiveUsersSelector,
  users => users.length
);

const formatChatMessage = (chat: Chat, currentUserId: string) => {
  const {
    channelData: {
      data: {
        from: { id, name, role },
      },
    },
  } = chat;
  let userName = currentUserId === id ? 'YOU' : name;
  if (id !== currentUserId && role.toLowerCase() === 'admin') {
    userName = `${userName} (admin)`;
  }
  return {
    name: userName,
    text: chat.text,
    id: chat.id,
    role: role.toLowerCase(),
    self: id === currentUserId,
    read: chat.read,
  };
};

export const chatAllMessagesSelector = createSelector(
  chatMessagesSelector,
  authUserSelector,
  (messages, user) =>
    messages
      .filter(
        m =>
          (m.channelData.data.from.role.toLowerCase() === 'admin' &&
            m.channelData.data.userId === '*') ||
          m.channelData.data.from.role.toLowerCase() !== 'admin'
      )
      .map(message => formatChatMessage(message, user.profile.email))
);

export const chatActiveMessagesSelector = createSelector(
  chatMessagesSelector,
  chatActiveUserSelector,
  authUserSelector,
  (messages, { userId, name }, authUser) => {
    if (userId) {
      const firstUserMessage = messages.find(
        m => m.channelData.data.userId === userId
      );
      const userMessages = messages
        .filter(
          m =>
            m.channelData.data.userId === userId ||
            (m.channelData.data.userId === '*' &&
              new Date(m.timestamp) > new Date(firstUserMessage.timestamp))
        )
        .map(message => formatChatMessage(message, authUser.profile.email));
      return {
        userMessages,
        userId,
        name,
      };
    }

    return {
      userMessages: [],
      userId,
      name,
    };
  }
);
