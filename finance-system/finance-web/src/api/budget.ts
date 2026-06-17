import { get, post, put, del } from './request';
import type { BudgetItem, BudgetExecution, BudgetAdjust, BudgetAlert, BudgetMonthly } from '@/types/budget.d';
import type { PageParams, PagedResult } from '@/types/api.d';

export const budgetApi = {
  // ========== 预算年度设置（后端: api/budget/setting）==========
  /** 获取预算年度列表 */
  years: (params?: { status?: number }) => get<{ id: number; year: number; status: number }[]>('/budget/setting/years', params),
  /** 创建预算年度 */
  createYear: (data: { year: number }) => post('/budget/setting/year', data),
  /** 更新年度状态 */
  updateYearStatus: (id: number, status: number) => put(`/budget/setting/year/${id}/status`, null, { params: { status } }),

  // ========== 预算科目（后端: api/budget/setting/subject）==========
  /** 获取预算科目列表 */
  subjectList: (params: { yearId: number; pageIndex?: number; pageSize?: number; sortField?: string; sortOrder?: string } & Record<string, unknown>) =>
    get<PagedResult<BudgetItem>>('/budget/setting/subject/list', params),
  /** 新增预算科目 */
  subjectAdd: (data: Omit<BudgetItem, 'id'>) => post('/budget/setting/subject', data),
  /** 修改预算科目 */
  subjectUpdate: (data: Partial<BudgetItem> & { id: number }) => put(`/budget/setting/subject/${data.id}`, data),
  /** 删除预算科目 */
  subjectRemove: (id: number) => del(`/budget/setting/subject/${id}`),

  // ========== 月度预算（后端: api/budget/plan）==========
  /** 获取月度预算 */
  planMonthly: (budgetSubjectId: number) => get<BudgetMonthly[]>(`/budget/plan/${budgetSubjectId}`),
  /** 保存月度预算 */
  planSave: (data: import('@/types/budget.d').BudgetMonthlySaveRequest) => post('/budget/plan/monthly', data),
  /** 自动平均拆分年度预算到月度 */
  planAutoSplit: (budgetSubjectId: number) => post('/budget/plan/auto-split', null, { params: { budgetSubjectId } }),

  // ========== 预算执行跟踪（后端: api/budget/execution）==========
  /** 查询预算执行情况 */
  execution: (params: { budgetYearId: number; subjectId?: number; deptId?: number; month?: number; pageIndex?: number; pageSize?: number; sortField?: string; sortOrder?: string } & Record<string, unknown>) =>
    get<PagedResult<BudgetExecution>>('/budget/execution', params),

  // ========== 预算调整（后端: api/budget/adjustment）==========
  /** 发起预算调整 */
  adjustAdd: (data: Omit<BudgetAdjust, 'id'>) => post('/budget/adjustment', data),
  /** 审批预算调整 */
  adjustApprove: (id: number, action: number) => post(`/budget/adjustment/${id}/approve`, null, { params: { action } }),

  // ========== 预算预警（后端: api/budget/alert）==========
  /** 获取预警配置 */
  alertConfig: (budgetYearId: number) => get<{ budgetYearId: number; thresholdRate: number; alertEnabled: boolean }>('/budget/alert/config', { budgetYearId }),
  /** 保存预警配置 */
  alertSaveConfig: (data: { budgetYearId: number; thresholdRate: number; alertEnabled: boolean }) => post('/budget/alert/config', data),
  /** 检查超预警科目 */
  alertCheck: (params: { budgetYearId: number; pageIndex?: number; pageSize?: number } & Record<string, unknown>) =>
    get<PagedResult<BudgetAlert>>('/budget/alert/check', params),

  // ========== 兼容旧调用（别名）==========
  /** @deprecated 使用 years */
  getSetting: () => get<{ id: number; year: number; status: number }[]>('/budget/setting/years'),
  /** @deprecated 使用 planMonthly */
  planList: (params: PageParams & Partial<BudgetItem>) => get<PagedResult<BudgetItem>>('/budget/setting/subject/list', params),
  /** @deprecated 使用 adjustAdd */
  adjustList: (params: PageParams) => get<PagedResult<BudgetAdjust>>('/budget/adjustment', params),
  /** @deprecated 使用 alertCheck */
  alertList: (params: PageParams & { budgetYearId?: number }) => get<PagedResult<BudgetAlert>>('/budget/alert/check', params),
  /** @deprecated 使用 execution */
  analysis: (params: { year: number }) => get<Record<string, unknown>>('/budget/execution', params),
  /** @deprecated 使用 alertSaveConfig */
  updateSetting: (data: Record<string, unknown>) => post('/budget/alert/config', data),

  // ========== 预算分析图表（后端: api/budget/analysis）==========
  /** 预算概览 */
  analysisOverview: (year: number) => get<Record<string, unknown>>('/budget/analysis/overview', { year }),
  /** 月度趋势 */
  analysisMonthlyTrend: (year: number) => get<Record<string, unknown>[]>('/budget/analysis/monthly-trend', { year }),
  /** 科目对比 */
  analysisSubjectCompare: (year: number) => get<Record<string, unknown>[]>('/budget/analysis/subject-compare', { year }),
  /** 费用TOP10 */
  analysisExpenseTop10: (year: number, month?: number) => get<Record<string, unknown>[]>('/budget/analysis/expense-top10', { year, month }),
};
