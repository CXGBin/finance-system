import { get, post, put, del } from './request';
import type { Subject, BalanceItem, Voucher, VoucherEntry, LedgerRecord, AccountingPeriod } from '@/types/account.d';
import type { PageParams, PagedResult } from '@/types/api.d';

// 科目管理
export const subjectApi = {
  /** 获取科目树 */
  tree: (enabledOnly?: boolean) => get<Subject[]>('/account/subject/tree', { enabledOnly }),
  /** 获取科目详情 */
  detail: (id: number) => get<Subject>(`/account/subject/${id}`),
  /** 新增科目 */
  add: (data: Partial<Subject>) => post('/account/subject', data),
  /** 修改科目 */
  update: (data: Partial<Subject>) => put(`/account/subject/${data.id}`, data),
  /** 删除科目 */
  remove: (id: number) => del(`/account/subject/${id}`),
  /** 启用/停用科目 */
  toggleStatus: (id: number, isEnabled: number) => put(`/account/subject/${id}/status`, null, { params: { isEnabled } }),
};

// 期初余额
export const balanceApi = {
  /** 查询某期间期初余额 */
  list: (periodId: number) => get<BalanceItem[]>(`/account/subject/balance/${periodId}`),
  /** 批量保存期初余额 */
  save: (data: BalanceItem[]) => post('/account/subject/balance', data),
  /** 试算平衡校验 */
  trialBalance: (periodId: number) => get<{ isBalanced: boolean; debitTotal: number; creditTotal: number }>('/account/subject/balance/trial-balance', { periodId }),
};

// 凭证
export const voucherApi = {
  /** 分页查询凭证 */
  page: (params: PageParams & Partial<Voucher>) => get<PagedResult<Voucher>>('/account/voucher/page', params),
  /** 获取凭证详情（含分录） */
  detail: (id: number) => get<Voucher>(`/account/voucher/${id}`),
  /** 新增凭证（草稿） */
  add: (data: Omit<Voucher, 'id'>) => post('/account/voucher', data),
  /** 修改凭证（仅草稿） */
  update: (data: Partial<Voucher>) => put(`/account/voucher/${data.id}`, data),
  /** 删除凭证 */
  remove: (id: number) => del(`/account/voucher/${id}`),
  /** 审核凭证 */
  audit: (id: number) => post(`/account/voucher/${id}/audit`),
  /** 反审核凭证 */
  unaudit: (id: number) => post(`/account/voucher/${id}/unaudit`),
  /** 作废凭证 */
  void: (id: number) => post(`/account/voucher/${id}/void`),
};

// 会计期间
export const periodApi = {
  /** 获取年度期间列表 */
  list: (year: number) => get<AccountingPeriod[]>('/account/period/list', { year }),
  /** 获取当前期间 */
  current: () => get<AccountingPeriod>('/account/period/current'),
  /** 初始化年度期间 */
  initYear: (year: number) => post('/account/period/init-year', null, { params: { year } }),
  /** 期末结账 */
  close: (periodId: number) => post('/account/period/close', null, { params: { periodId } }),
  /** 反结账 */
  unclose: (periodId: number) => post('/account/period/unclose', null, { params: { periodId } }),
  /** 期末损益结转 */
  profitTransfer: (periodId: number) => post('/account/period/profit-transfer', null, { params: { periodId } }),
};

// 账簿
export const ledgerApi = {
  /** 总账查询 */
  general: (params: { subjectId?: number; startPeriod: string; endPeriod: string }) =>
    get<LedgerRecord[]>('/account/ledger/general', params),
  /** 明细账查询 */
  detail: (params: { subjectId?: number; startPeriod: string; endPeriod: string }) =>
    get<LedgerRecord[]>('/account/ledger/detail', params),
  /** 日记账查询（现金/银行） */
  journal: (params: { startPeriod: string; endPeriod: string }) =>
    get<LedgerRecord[]>('/account/ledger/journal', params),
  /** 科目汇总表 */
  subjectSummary: (year: number, month: number) =>
    get<LedgerRecord[]>('/account/ledger/subject-summary', { year, month }),
};
