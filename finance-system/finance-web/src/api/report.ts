import { get, post, put, del } from './request';
import type { BalanceSheetRow, IncomeStatementRow, CashFlowRow, SubjectBalanceRow, ReportParams } from '@/types/report.d';
import type { PageParams, PagedResult } from '@/types/api.d';

export const reportApi = {
  // ========== 标准报表 ==========
  /** 资产负债表 */
  balanceSheet: (params: ReportParams) => get<BalanceSheetRow[]>('/report/balance-sheet', params as any),
  /** 利润表 */
  incomeStatement: (params: ReportParams) => get<IncomeStatementRow[]>('/report/income-statement', params as any),
  /** 现金流量表 */
  cashFlow: (params: ReportParams) => get<CashFlowRow[]>('/report/cash-flow', params as any),
  /** 科目余额表 */
  subjectBalance: (params: ReportParams & { subjectCode?: string; level?: number }) =>
    get<SubjectBalanceRow[]>('/report/subject-balance', params as any),

  // ========== 自定义报表（后端: api/report/custom）==========
  /** 获取模板列表 */
  templateList: (params?: PageParams) => get<PagedResult<any>>('/report/custom/template/list', params as any),
  /** 创建模板 */
  templateAdd: (data: { templateName: string; reportType: string; config: Record<string, unknown> }) => post('/report/custom/template', data),
  /** 修改模板 */
  templateUpdate: (data: { id: number; templateName?: string; config?: Record<string, unknown> }) => put(`/report/custom/template/${data.id}`, data),
  /** 删除模板 */
  templateRemove: (id: number) => del(`/report/custom/template/${id}`),
  /** 按模板生成报表 */
  templateGenerate: (id: number, period: string) => get<any>(`/report/custom/template/${id}/generate`, { period }),

  // ========== 兼容旧调用（别名）==========
  /** 自定义报表查询 */
  custom: (params: { reportId: string; [key: string]: unknown }) =>
    get<Record<string, unknown>[]>(`/report/custom/template/${params.reportId}/generate`, params),
};

// ========== 报表导出 ==========
export const reportExportApi = {
  /** 导出Excel */
  excel: (params: { reportType: string; year: number; month: number }) => get<string>('/report/export/excel', params as any),
  /** 导出PDF */
  pdf: (params: { reportType: string; year: number; month: number }) => get<string>('/report/export/pdf', params as any),
};

// ========== 多期对比 ==========
export const reportCompareApi = {
  /** 多期对比分析 */
  compare: (params: { reportType: string; periods: string[] }) => get<any>('/report/compare', params as any),
};
