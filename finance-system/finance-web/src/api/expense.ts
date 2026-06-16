import { get, post, put, del } from './request';
import type { ExpenseType, ExpenseClaim, PaymentRecord, ExpenseAllocate, ExpenseStats, ExpenseLoan } from '@/types/expense.d';
import type { PageParams, PagedResult } from '@/types/api.d';

export const expenseApi = {
  // ========== 费用类型（后端: api/expense/type）==========
  /** 获取费用类型列表 */
  typeList: () => get<ExpenseType[]>('/expense/type/list'),
  /** 新增费用类型 */
  typeAdd: (data: Partial<ExpenseType>) => post('/expense/type', data),
  /** 修改费用类型 */
  typeUpdate: (data: Partial<ExpenseType>) => put(`/expense/type/${data.id}`, data),
  /** 删除费用类型 */
  typeRemove: (id: number) => del(`/expense/type/${id}`),

  // ========== 报销管理（后端: api/expense/claim）==========
  /** 获取报销列表 */
  claimList: (params: PageParams & Partial<ExpenseClaim>) => get<PagedResult<ExpenseClaim>>('/expense/claim/list', params),
  /** 获取报销详情 */
  claimDetail: (id: number) => get<ExpenseClaim>(`/expense/claim/${id}`),
  /** 新增报销 */
  claimAdd: (data: Partial<ExpenseClaim>) => post('/expense/claim', data),
  /** 修改报销 */
  claimUpdate: (data: Partial<ExpenseClaim>) => put(`/expense/claim/${data.id}`, data),
  /** 提交报销 */
  claimSubmit: (id: number) => post(`/expense/claim/${id}/submit`),
  /** 审批通过 */
  claimApprove: (id: number) => post(`/expense/claim/${id}/approve`),
  /** 审批驳回 */
  claimReject: (id: number) => post(`/expense/claim/${id}/reject`),
  /** 确认付款 */
  claimPay: (id: number) => post(`/expense/claim/${id}/pay`),

  // ========== 费用统计（后端: api/expense/statistics）==========
  /** 获取费用统计 */
  statistics: (params: Record<string, unknown>) => get<ExpenseStats[]>('/expense/statistics', params),

  // ========== 兼容旧调用（别名）==========
  /** 付款列表（后端暂无独立付款接口，使用报销列表过滤已付款） */
  paymentList: (params: PageParams) => get<PagedResult<ExpenseClaim>>('/expense/claim/list', { ...params, paymentStatus: 1 }),
  /** 付款确认 */
  paymentConfirm: (id: number) => post(`/expense/claim/${id}/pay`),
  /** 分摊列表（后端暂无独立分摊接口） */
  allocateList: (params: PageParams) => get<PagedResult<ExpenseAllocate>>('/expense/allocate', params),
  /** 新增分摊 */
  allocateAdd: (data: Partial<ExpenseAllocate>) => post('/expense/allocate', data),

  // ========== 费用分摊（后端: api/expense/allocate）==========
  /** 分页查询分摊列表 */
  allocatePage: (params: PageParams) => get<PagedResult<ExpenseAllocate>>('/expense/allocate/list', params),
  /** 创建分摊 */
  allocateCreate: (data: Partial<ExpenseAllocate>) => post('/expense/allocate', data),
};

/** 借款申请 */
export const loanApi = {
  /** 借款列表 */
  list: (params: PageParams) => get<PagedResult<ExpenseLoan>>('/expense/loan', params),
  /** 借款详情 */
  getById: (id: number) => get<ExpenseLoan>(`/expense/loan/${id}`),
  /** 创建借款 */
  create: (data: Omit<ExpenseLoan, 'id'>) => post<number>('/expense/loan', data),
  /** 审批通过 */
  approve: (id: number) => post(`/expense/loan/${id}/approve`),
  /** 审批驳回 */
  reject: (id: number) => post(`/expense/loan/${id}/reject`),
  /** 还款确认 */
  settle: (id: number, data: { settleAmount: number; settleDate: string }) => post(`/expense/loan/${id}/settle`, data),
};
