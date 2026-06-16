import { get, post, put, del } from './request';
import type { TaxType, TaxDeclaration, Invoice, TaxReport, TaxCalendarItem } from '@/types/tax.d';
import type { PageParams, PagedResult } from '@/types/api.d';

export const taxApi = {
  // ========== 税种管理（后端: api/tax/category）==========
  /** 获取税种列表 */
  typeList: () => get<TaxType[]>('/tax/category/list'),
  /** 新增税种 */
  typeAdd: (data: Partial<TaxType>) => post('/tax/category', data),
  /** 修改税种 */
  typeUpdate: (data: Partial<TaxType>) => put(`/tax/category/${data.id}`, data),
  /** 删除税种 */
  typeRemove: (id: number) => del(`/tax/category/${id}`),

  // ========== 纳税申报（后端: api/tax/declaration）==========
  /** 获取申报列表 */
  declarationList: (params: PageParams) => get<PagedResult<TaxDeclaration>>('/tax/declaration/list', params),
  /** 计算应纳税额 */
  declarationCalculate: (data: { taxCategoryId: number; declarePeriod: string; taxBase?: number }) => post<number>('/tax/declaration/calculate', data),
  /** 申报 */
  declarationDeclare: (id: number) => post(`/tax/declaration/${id}/declare`),
  /** 缴款 */
  declarationPay: (id: number) => post(`/tax/declaration/${id}/pay`),

  // ========== 发票管理（后端: api/tax/invoice）==========
  /** 获取发票列表 */
  invoiceList: (params: PageParams & Partial<Invoice>) => get<PagedResult<Invoice>>('/tax/invoice/list', params),
  /** 新增发票 */
  invoiceAdd: (data: Partial<Invoice>) => post('/tax/invoice', data),
  /** 删除发票 */
  invoiceRemove: (id: number) => del(`/tax/invoice/${id}`),
  /** 发票验真 */
  invoiceVerify: (id: number) => post(`/tax/invoice/${id}/verify`),

  // ========== 兼容旧调用（别名）==========
  /** 税务报表（后端暂无独立报表接口，使用申报列表汇总） */
  reportList: (params: Record<string, unknown>) => get<TaxReport[]>('/tax/declaration/list', params),
  /** 税务日历（后端暂无独立日历接口，返回空） */
  calendar: (month: string) => get<TaxCalendarItem[]>('/tax/category/list', { month }),

  // ========== 税务报表（后端: api/tax/report）==========
  /** 税务汇总报表 */
  reportSummary: (year: number) => get<Record<string, unknown>>('/tax/report/summary', { year }),
  /** 按税种分类查询 */
  reportByCategory: (year: number, month?: number) => get<Record<string, unknown>[]>('/tax/report/by-category', { year, month }),
  /** 税负率分析 */
  reportBurden: (year: number, quarter?: number) => get<Record<string, unknown>>('/tax/report/burden', { year, quarter }),

  // ========== 附加税（后端: api/tax/declaration）==========
  /** 自动计算附加税 */
  declarationSurcharges: (data: { declarePeriod: string; vatDeclarationId: number }) =>
    post<TaxDeclaration[]>('/tax/declaration/surcharges', data),

  // ========== 税务日历（后端: api/tax/calendar）==========
  /** 获取税务日历事项 */
  calendarList: (year: number, month: number) => get<Record<string, unknown>[]>('/tax/calendar', { year, month }),
};
