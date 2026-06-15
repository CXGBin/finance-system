import { get, post, put, del } from './request';
import type { BalanceSheetRow, IncomeStatementRow, CashFlowRow, SubjectBalanceRow, ReportParams } from '@/types/report.d';
import type { PageParams, PagedResult } from '@/types/api.d';

/** 报表模板 */
export interface ReportTemplate {
  id: number;
  templateName: string;
  reportType: string;
  config: Record<string, unknown>;
  createTime?: string;
  updateTime?: string;
}

/** 多期对比结果行 */
export interface CompareRow {
  itemName: string;
  periods: { period: string; amount: number }[];
}

export const reportApi = {
  // ========== 标准报表 ==========
  /** 资产负债表 */
  balanceSheet: (params: ReportParams) => get<BalanceSheetRow[]>('/report/balance-sheet', params),
  /** 利润表 */
  incomeStatement: (params: ReportParams) => get<IncomeStatementRow[]>('/report/income-statement', params),
  /** 现金流量表 */
  cashFlow: (params: ReportParams) => get<CashFlowRow[]>('/report/cash-flow', params),
  /** 科目余额表 */
  subjectBalance: (params: ReportParams & { subjectCode?: string; level?: number }) =>
    get<SubjectBalanceRow[]>('/report/subject-balance', params),

  // ========== 自定义报表（后端: api/report/custom）==========
  /** 获取模板列表 */
  templateList: (params?: PageParams) => get<PagedResult<ReportTemplate>>('/report/custom/template/list', params),
  /** 创建模板 */
  templateAdd: (data: { templateName: string; reportType: string; config: Record<string, unknown> }) => post('/report/custom/template', data),
  /** 修改模板 */
  templateUpdate: (data: { id: number; templateName?: string; config?: Record<string, unknown> }) => put(`/report/custom/template/${data.id}`, data),
  /** 删除模板 */
  templateRemove: (id: number) => del(`/report/custom/template/${id}`),
  /** 按模板生成报表 */
  templateGenerate: (id: number, period: string) => get<Record<string, unknown>>(`/report/custom/template/${id}/generate`, { period }),

  // ========== 兼容旧调用（别名）==========
  /** 自定义报表查询 */
  custom: (params: { reportId: string; [key: string]: unknown }) =>
    get<Record<string, unknown>[]>(`/report/custom/template/${params.reportId}/generate`, params),
};

// ========== 报表导出 ==========
export const reportExportApi = {
  /** 导出Excel */
  excel: (params: { reportType: string; year: number; month: number }) => get<string>('/report/export/excel', params),
  /** 导出PDF */
  pdf: (params: { reportType: string; year: number; month: number }) => get<string>('/report/export/pdf', params),
};

// ========== 多期对比 ==========
export const reportCompareApi = {
  /** 多期对比分析 */
  compare: (params: { reportType: string; periods: string[] }) => get<CompareRow[]>('/report/compare', params),
};
