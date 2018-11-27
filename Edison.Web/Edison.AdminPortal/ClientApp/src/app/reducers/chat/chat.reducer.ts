import { createEntityAdapter, EntityAdapter, EntityState, Update } from '@ngrx/entity';

import { ChatActions, ChatActionTypes } from './chat.actions';
import { Chat } from './chat.model';

export function sortByStartDate(a: Chat, b: Chat): number {
    return new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime();
}

export interface State extends EntityState<Chat> {
    // additional entities state properties
    token: string;
    expiresIn: number;
    conversationId: string;
    userContext: {
        id: string;
        name: string;
        role: "bot" | "channel" | "user";
    },
    activeUserId: string;
    activeUserName: string;
    activeConversationId: string;
}

export const adapter: EntityAdapter<Chat> = createEntityAdapter<Chat>({
    sortComparer: sortByStartDate
});

export const initialState: State = adapter.getInitialState({
    // additional entity state properties
    token: null,
    expiresIn: null,
    conversationId: null,
    userContext: null,
    activeUserId: null,
    activeUserName: null,
    activeConversationId: null,
});

export function reducer(
    state = initialState,
    action: ChatActions
): State {
    switch (action.type) {
        case ChatActionTypes.AddChat: {
            return adapter.addOne(action.payload.chat, state);
        }

        case ChatActionTypes.UpsertChat: {
            return adapter.upsertOne(action.payload.chat, state);
        }

        case ChatActionTypes.AddChats: {
            return adapter.addMany(action.payload.chats, state);
        }

        case ChatActionTypes.UpsertChats: {
            return adapter.upsertMany(action.payload.chats, state);
        }

        case ChatActionTypes.UpdateChat: {
            return adapter.updateOne(action.payload.chat, state);
        }

        case ChatActionTypes.UpdateChats: {
            return adapter.updateMany(action.payload.chats, state);
        }

        case ChatActionTypes.DeleteChat: {
            return adapter.removeOne(action.payload.id, state);
        }

        case ChatActionTypes.DeleteChats: {
            return adapter.removeMany(action.payload.ids, state);
        }

        case ChatActionTypes.LoadChats: {
            return adapter.addAll(action.payload.chats, state);
        }

        case ChatActionTypes.ClearChats: {
            return adapter.removeAll(state);
        }

        case ChatActionTypes.GetChatAuthTokenSuccess: {
            return {
                ...state,
                ...action.payload.result,
            };
        }

        case ChatActionTypes.SelectActiveUser: {
            return {
                ...state,
                activeUserId: action.payload.userId,
                activeUserName: action.payload.userName
            }
        }

        case ChatActionTypes.SelectActiveConversation: {
            return {
                ...state,
                activeConversationId: action.payload.conversationId
            }
        }

        case ChatActionTypes.EndConversation: {
            const messagesToRemove = selectAll(state)
                .filter(message => message.channelData &&
                    message.channelData.data &&
                    message.channelData.data.userId === action.payload.userId)
                .map(message => message.id);
            return adapter.removeMany(messagesToRemove, state);
        }

        case ChatActionTypes.UpdateUserReadReceipt: {
            const updatedMessages: Update<Chat>[] = selectAll(state)
                .filter(message => message.channelData.data.userId === action.payload.userId
                    && new Date(message.timestamp).getTime() < action.payload.date.getTime())
                .map(message => {
                    return {
                        id: message.id,
                        changes: {
                            ...message,
                            read: true
                        }
                    }
                });

            return adapter.updateMany(updatedMessages, state);
        }

        default: {
            return state;
        }
    }
}

export const {
    selectIds,
    selectEntities,
    selectAll,
    selectTotal,
} = adapter.getSelectors();
