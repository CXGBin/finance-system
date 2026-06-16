/** 通用 API 响应 */
export interface ApiResponse<T = unknown> {
  code: number;
  message: string;
  data: T;
}

/** 分页结果 - 与后端 PageResult 对齐 */
export interface PagedResult<T> {
  list: T[]; // JSON序列化后小写
  total: number;
  pageIndex?: number;
  pageSize?: number;
}

/** 分页请求参数 - 与后端 PageRequest 对齐 */
export interface PageParams {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: string;
}

/** 搜索表单类型（各模块可扩展） */
export type SearchParams = Record<string, unknown>;
