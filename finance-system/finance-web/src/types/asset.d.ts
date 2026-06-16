/** 资产分类 - 与后端 AssetCategory 对齐 */
export interface AssetCategory {
  id: number;
  parentId?: number;
  categoryCode: string; // 后端 CategoryCode
  categoryName: string; // 后端 CategoryName
  depreciationMethod: number; // 后端 DepreciationMethod (int)
  usefulLifeMonths: number; // 后端 UsefulLifeMonths
  residualRate: number; // 后端 ResidualRate
  sortOrder: number;
  isEnabled: number;
  children?: AssetCategory[];
}

/** 资产卡片 - 与后端 AssetCard 对齐 */
export interface AssetCard {
  id: number;
  assetCode: string; // 后端 AssetCode
  assetName: string; // 后端 AssetName
  categoryId?: number;
  specification?: string;
  deptId?: number;
  keeper?: string; // 后端 Keeper
  location?: string;
  originalValue: number; // 后端 OriginalValue
  residualRate: number; // 后端 ResidualRate
  residualValue?: number; // 后端 ResidualValue
  depreciationMethod: number;
  usefulLifeMonths: number;
  acquisitionDate: string; // 后端 AcquisitionDate
  accumulatedDepreciation: number; // 后端 AccumulatedDepreciation
  netValue: number; // 后端 NetValue
  status: number; // 后端 Status (int)
  remark?: string;
}

/** 折旧记录 - 与后端 AssetDepreciation 对齐 */
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

/** 资产盘点 - 与后端 AssetInventory 对齐 */
export interface AssetInventory {
  id: number;
  name: string;
  status: number; // 后端 Status (int)
  createTime?: string;
}
