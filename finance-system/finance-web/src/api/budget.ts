import { get, post, put, del } from './request';
import type { BudgetItem, BudgetExecution, BudgetAdjust, BudgetAlert } from '@/types/budget.d';
import type { PageParams, PagedResult } from '@/types/api.d';

export const budgetApi = {
  // ========== 预算年度设置（后端: api/budget/setting）==========
  /** 获取预算年度列表 */
  years: (params?: { status?: number }) => get<any[]>('/budget/setting/years', params as any),
  /** 创建预算年度 */
  createYear: (data: { year: number }) => post('/budget/setting/year', data),
  /** 更新年度状态 */
  updateYearStatus: (id: number, status: number) => put(`/budget/setting/year/${id}/status`, null, { params: { status } }),

  // ========== 预算科目（后端: api/budget/setting/subject）==========
  /** 获取预算科目列表 */
  subjectList: (params: { yearId: number; pageIndex?: number; pageSize?: number }) =>
    get<PagedResult<any>>('/budget/setting/subject/list', params as any),
  /** 新增预算科目 */
  subjectAdd: (data: any) => post('/budget/setting/subject', data),
  /** 修改预算科目 */
  subjectUpdate: (data: any) => put(`/budget/setting/subject/${data.id}`, data),
  /** 删除预算科目 */
  subjectRemove: (id: number) => del(`/budget/setting/subject/${id}`),

  // ========== 月度预算（后端: api/budget/plan）==========
  /** 获取月度预算 */
  planMonthly: (budgetSubjectId: number) => get<any[]>(`/budget/plan/${budgetSubjectId}`),
  /** 保存月度预算 */
  planSave: (data: any) => post('/budget/plan/monthly', data),
  /** 自动平均拆分年度预算到月度 */
  planAutoSplit: (budgetSubjectId: number) => post('/budget/plan/auto-split', null, { params: { budgetSubjectId } }),

  // ========== 预算执行跟踪（后端: api/budget/execution）==========
  /** 查询预算执行情况 */
  execution: (params: { year: number; subjectId?: number; deptId?: number }) =>
    get<BudgetExecution[]>('/budget/execution', params as any),

  // ========== 预算调整（后端: api/budget/adjustment）==========
  /** 发起预算调整 */
  adjustAdd: (data: any) => post('/budget/adjustment', data),
  /** 审批预算调整 */
  adjustApprove: (id: number, action: number) => post(`/budget/adjustment/${id}/approve`, null, { params: { action } }),

  // ========== 预算预警（后端: api/budget/alert）==========
  /** 获取预警配置 */
  alertConfig: (budgetYearId: number) => get<any>('/budget/alert/config', { budgetYearId }),
  /** 保存预警配置 */
  alertSaveConfig: (data: any) => post('/budget/alert/config', data),
  /** 检查超预警科目 */
  alertCheck: (budgetYearId: number) => get<any[]>('/budget/alert/check', { budgetYearId }),

  // ========== 兼容旧调用（别名）==========
  /** @deprecated 使用 years */
  getSetting: () => get<any>('/budget/setting/years'),
  /** @deprecated 使用 planMonthly */
  planList: (params: PageParams & Partial<BudgetItem>) => get<PagedResult<BudgetItem>>('/budget/setting/subject/list', params as any),
  /** @deprecated 使用 adjustAdd */
  adjustList: (params: PageParams) => get<PagedResult<BudgetAdjust>>('/budget/adjustment', params as any),
  /** @deprecated 使用 alertCheck */
  alertList: (params: PageParams & { budgetYearId?: number }) => get<PagedResult<BudgetAlert>>('/budget/alert/check', params as any),
  /** @deprecated 使用 execution */
  analysis: (params: { year: number }) => get<Record<string, unknown>>('/budget/execution', params),
  /** @deprecated 使用 alertSaveConfig */
  updateSetting: (data: Record<string, unknown>) => post('/budget/alert/config', data),
};
