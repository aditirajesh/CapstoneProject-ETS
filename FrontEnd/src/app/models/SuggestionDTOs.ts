export interface SuggestionDto {
  id: number;
  analyserUsername: string;
  targetUsername: string;
  content: string;
  reportPeriodStart: string;
  reportPeriodEnd: string;
  isRead: boolean;
  createdAt: string;
}


export interface SuggestionAddDto {
  targetUsername: string;
  content: string;
  reportPeriodStart: string;
  reportPeriodEnd: string;
}