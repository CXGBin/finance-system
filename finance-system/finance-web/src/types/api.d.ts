/** 通用 API 响应 */
export interface ApiResponse<T = unknown> {
  code: number;
  message: string;
  data: T;
}

/** 分页结果 */
export interface PagedResult<T> {
  list: T[];
  total: number;
  page: number;
  pageSize: number;
}

/** 分页请求参数 */
export interface PageParams {
  page: number;
  pageSize: number;
}

/** 搜索表单类型（各模块可扩展） */
export type SearchParams = Record<string, unknown>;
