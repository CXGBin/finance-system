import { get, post, put, del } from './request';
import type { AssetCategory, AssetCard, DepreciationRecord, AssetChange, AssetInventory } from '@/types/asset.d';
import type { PageParams, PagedResult } from '@/types/api.d';

export const assetApi = {
  // ========== 分类管理（后端: api/asset/category）==========
  /** 获取分类树 */
  categoryTree: () => get<AssetCategory[]>('/asset/category/tree'),
  /** 新增分类 */
  categoryAdd: (data: Partial<AssetCategory>) => post('/asset/category', data),
  /** 修改分类 */
  categoryUpdate: (data: Partial<AssetCategory>) => put(`/asset/category/${data.id}`, data),
  /** 删除分类 */
  categoryRemove: (id: number) => del(`/asset/category/${id}`),

  // ========== 资产卡片（后端: api/asset/card）==========
  /** 分页查询资产卡片 */
  cardList: (params: PageParams & Partial<AssetCard>) => get<PagedResult<AssetCard>>('/asset/card/page', params),
  /** 获取资产详情 */
  cardDetail: (id: number) => get<AssetCard>(`/asset/card/${id}`),
  /** 新增资产 */
  cardAdd: (data: Partial<AssetCard>) => post('/asset/card', data),
  /** 修改资产 */
  cardUpdate: (data: Partial<AssetCard>) => put(`/asset/card/${data.id}`, data),
  /** 删除资产 */
  cardRemove: (id: number) => del(`/asset/card/${id}`),
  /** 资产变动 */
  cardChange: (id: number, changeType: number, data: Partial<AssetChange>) =>
    post(`/asset/card/${id}/change`, data, { params: { changeType } }),

  // ========== 折旧（后端: api/asset/depreciation）==========
  /** 计算折旧 */
  depreciationCalculate: (year: number, month: number) => get<DepreciationRecord[]>('/asset/depreciation/calculate', { year, month }),
  /** 确认折旧 */
  depreciationConfirm: (year: number, month: number) => post('/asset/depreciation/confirm', null, { params: { year, month } }),
  /** 折旧汇总 */
  depreciationSummary: (year: number) => get<DepreciationRecord[]>('/asset/depreciation/summary', { year }),

  // ========== 兼容旧调用（别名）==========
  /** @deprecated 使用 depreciationCalculate */
  depreciationList: (params: PageParams & { year?: number; month?: number }) => get<DepreciationRecord[]>('/asset/depreciation/calculate', params),
  /** @deprecated 使用 depreciationConfirm */
  depreciationRun: (period: string) => {
    const [year, month] = period.split('-').map(Number);
    return post('/asset/depreciation/confirm', null, { params: { year, month } });
  },
  /** 资产变动列表（后端暂无独立变动列表接口，使用卡片变动） */
  changeList: (params: PageParams) => get<PagedResult<AssetCard>>('/asset/card/page', params),
  /** 资产盘点（后端暂无独立盘点接口，使用卡片列表） */
  inventoryList: (params: PageParams) => get<PagedResult<AssetCard>>('/asset/card/page', params),
  /** 资产报表 */
  reportData: (params: Record<string, unknown>) => get<Record<string, unknown>>('/asset/depreciation/summary', params),
  /** 新增变动 */
  changeAdd: (data: Partial<AssetChange>) => post('/asset/card', data),
  /** 新增盘点 */
  inventoryAdd: (data: Partial<AssetInventory>) => post('/asset/card', data),

  // ========== 盘点管理（后端: api/asset/inventory）==========
  /** 分页查询盘点列表 */
  inventoryPage: (params: PageParams) => get<PagedResult<AssetInventory>>('/asset/inventory/list', params),
  /** 创建盘点单 */
  inventoryCreate: (data: Partial<AssetInventory>) => post('/asset/inventory', data),
  /** 完成盘点 */
  inventoryComplete: (id: number) => post(`/asset/inventory/${id}/complete`),

  // ========== 资产报表（后端: api/asset/report）==========
  /** 资产台账 */
  reportLedger: (params: PageParams & { categoryId?: number; status?: number }) =>
    get<PagedResult<AssetCard>>('/asset/report/ledger', params),
  /** 折旧汇总报表 */
  reportDepreciationSummary: (year: number) => get<Record<string, unknown>[]>('/asset/report/depreciation-summary', { year }),
  /** 资产价值统计 */
  reportValueStats: () => get<Record<string, unknown>>('/asset/report/value-stats'),
  /** 资产处置（报废/出售） */
  dispose: (id: number, data: Record<string, unknown>) => post<number>(`/asset/card/${id}/dispose`, data),
};
