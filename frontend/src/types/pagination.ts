export interface PagedResponse<T> {
  items: T[];
  nextCursor: string | null;
  hasMore: boolean;
}