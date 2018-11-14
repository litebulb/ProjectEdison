export type UserRole = 'bot' | 'channel' | 'user' | 'consumer' | 'admin';

export interface UserModel {
    id: string,
    name?: string,
    iconUrl?: string,
    role?: UserRole
}
