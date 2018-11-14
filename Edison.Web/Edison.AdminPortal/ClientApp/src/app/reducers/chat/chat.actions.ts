import { Action } from '@ngrx/store';
import { Update } from '@ngrx/entity';
import { Chat } from './chat.model';

export enum ChatActionTypes {
    LoadChats = '[Chat] Load Chats',
    AddChat = '[Chat] Add Chat',
    UpsertChat = '[Chat] Upsert Chat',
    AddChats = '[Chat] Add Chats',
    UpsertChats = '[Chat] Upsert Chats',
    UpdateChat = '[Chat] Update Chat',
    UpdateChats = '[Chat] Update Chats',
    DeleteChat = '[Chat] Delete Chat',
    DeleteChats = '[Chat] Delete Chats',
    ClearChats = '[Chat] Clear Chats',
    GetChatAuthToken = '[Chat] Get Auth Token',
    GetChatAuthTokenSuccess = '[Chat] Get Auth Token Success',
    GetChatAuthTokenError = '[Chat] Get Auth Token Error',
    SelectActiveUser = '[Chat] Select Active User',
    SelectActiveConversation = '[Chat] Select Active Conversation',
    SendNewMessage = '[Chat] Send New Message',
    EndConversation = '[Chat] End Conversation',
    ToggleAllUsersChatWindow = '[Chat] Toggle All Users Chat Window',
    ToggleUserChatWindow = '[Chat] Toggle User Chat Window'
}

export class ToggleAllUsersChatWindow implements Action {
    readonly type = ChatActionTypes.ToggleAllUsersChatWindow;

    constructor (public payload: { open: boolean, userId?: string, userName?: string }) { }
}

export class ToggleUserChatWindow implements Action {
    readonly type = ChatActionTypes.ToggleUserChatWindow;

    constructor (public payload: { open: boolean, userId?: string, userName?: string }) { }
}

export class EndConversation implements Action {
    readonly type = ChatActionTypes.EndConversation;

    constructor (public payload: { userId: string }) { }
}

export class SendNewMessage implements Action {
    readonly type = ChatActionTypes.SendNewMessage;

    constructor (public payload: { message: string, userId?: string }) { }
}

export class SelectActiveConversation implements Action {
    readonly type = ChatActionTypes.SelectActiveConversation;

    constructor (public payload: { conversationId: string }) { }
}

export class SelectActiveUser implements Action {
    readonly type = ChatActionTypes.SelectActiveUser;

    constructor (public payload: { userId?: string, userName?: string }) { }
}

export class GetChatAuthToken implements Action {
    readonly type = ChatActionTypes.GetChatAuthToken;
}

export class GetChatAuthTokenSuccess implements Action {
    readonly type = ChatActionTypes.GetChatAuthTokenSuccess;

    constructor (public payload: { result: any }) { }
}

export class GetChatAuthTokenError implements Action {
    readonly type = ChatActionTypes.GetChatAuthTokenError;
}

export class LoadChats implements Action {
    readonly type = ChatActionTypes.LoadChats;

    constructor (public payload: { chats: Chat[] }) { }
}

export class AddChat implements Action {
    readonly type = ChatActionTypes.AddChat;

    constructor (public payload: { chat: Chat }) { }
}

export class UpsertChat implements Action {
    readonly type = ChatActionTypes.UpsertChat;

    constructor (public payload: { chat: Chat }) { }
}

export class AddChats implements Action {
    readonly type = ChatActionTypes.AddChats;

    constructor (public payload: { chats: Chat[] }) { }
}

export class UpsertChats implements Action {
    readonly type = ChatActionTypes.UpsertChats;

    constructor (public payload: { chats: Chat[] }) { }
}

export class UpdateChat implements Action {
    readonly type = ChatActionTypes.UpdateChat;

    constructor (public payload: { chat: Update<Chat> }) { }
}

export class UpdateChats implements Action {
    readonly type = ChatActionTypes.UpdateChats;

    constructor (public payload: { chats: Update<Chat>[] }) { }
}

export class DeleteChat implements Action {
    readonly type = ChatActionTypes.DeleteChat;

    constructor (public payload: { id: string }) { }
}

export class DeleteChats implements Action {
    readonly type = ChatActionTypes.DeleteChats;

    constructor (public payload: { ids: string[] }) { }
}

export class ClearChats implements Action {
    readonly type = ChatActionTypes.ClearChats;
}

export type ChatActions =
    LoadChats
    | AddChat
    | UpsertChat
    | AddChats
    | UpsertChats
    | UpdateChat
    | UpdateChats
    | DeleteChat
    | DeleteChats
    | ClearChats
    | GetChatAuthToken
    | GetChatAuthTokenSuccess
    | GetChatAuthTokenError
    | SelectActiveUser
    | SelectActiveConversation
    | EndConversation
    | ToggleAllUsersChatWindow
    | ToggleUserChatWindow;
