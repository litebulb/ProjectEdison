import { MessageModel } from '../../core/services/directline/models/activity-model';

export interface Chat extends MessageModel {
    read?: boolean;
}

export interface Message {
    name: string;
    text: string;
    role: string;
    self: boolean;
    read?: boolean;
}
