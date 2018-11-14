import { TokenResponseModel } from "./token-response.model";

export interface StartConversationResponseModel extends TokenResponseModel {
    streamUrl: string;
}
