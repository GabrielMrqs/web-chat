export interface Message {
  roomId?: string;
  sentAt?: Date;
  messageId?: string;
  senderId?: string;
  content: string;
} 