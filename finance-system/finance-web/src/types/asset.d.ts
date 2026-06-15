/** 资产分类 */
export interface AssetCategory {
  id: number;
  parentId?: number;
  name: string;
  code: string;
  depreciationMethod: 'straight' | 'declining' | 'double_declining' | 'sum_years';
  usefulLife: number;
  residualRate: number;
  children?: AssetCategory[];
}

/** 资产卡片 */
export interface AssetCard {
  id: number;
  code: string;
  name: string;
  categoryId?: number;
  categoryName?: string;
  specification?: string;
  department?: string;
  custodian?: string;
  purchaseDate: string;
  originalValue: number;
  residualValue: number;
  depreciationAmount: number;
  netValue: number;
  status: 'in_use' | 'idle' | 'scrapped' | 'transferred';
  location?: string;
  remark?: string;
}

/** 折旧记录 */
export interface DepreciationRecord {
  id: number;
  assetId: number;
  assetName?: string;
  period: string;
  originalValue: number;
  netBookValue: number;
  depreciationAmount: number;
  cumulativeDepreciation: number;
}

/** 资产变动 */
export interface AssetChange {
  id: number;
  assetId: number;
  assetName?: string;
  changeType: string;
  changeDate: string;
  description: string;
  operator: string;
}

/** 资产盘点 */
export interface AssetInventory {
  id: number;
  name: string;
  status: 'draft' | 'in_progress' | 'completed';
  createTime: string;
  totalCount: number;
  matchCount: number;
  differenceCount: number;
}
