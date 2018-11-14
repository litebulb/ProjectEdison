import { UserModel } from "./user-model";

export interface IActivityModel {
    type: string,
    channelData?: {
        data?: {
            from?: UserModel,
            reportType?: string,
            userId?: string,
        },
        baseCommand: string,
    },
    channelId?: string,
    conversation?: { id: string },
    eTag?: string,
    from: UserModel,
    id?: string,
    timestamp?: string
}

export interface MessageModel extends IActivityModel {
    type: "message",
    text?: string,
    locale?: string,
    textFormat?: "plain" | "markdown" | "xml",
    // attachmentLayout?: AttachmentLayout,
    // attachments?: Attachment[],
    entities?: any[],
    // suggestedActions?: { actions: CardAction[], to?: string[] },
    speak?: string,
    inputHint?: string,
    value?: object
}

export interface TypingModel extends IActivityModel {
    type: "typing"
}

export interface EventActivityModel extends IActivityModel {
    type: "event",
    name: string,
    value: any
}

export type ActivityModel = MessageModel | TypingModel | EventActivityModel;
