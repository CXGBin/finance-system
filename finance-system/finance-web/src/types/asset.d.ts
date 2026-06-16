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
  assetCardId: number; // 后端 AssetCardId
  periodId: number; // 后端 PeriodId
  month: number; // 后端 Month
  depreciationAmount: number; // 后端 DepreciationAmount
  accumulatedDepreciation: number; // 后端 AccumulatedDepreciation
  netValue: number; // 后端 NetValue
}

/** 资产变动 - 与后端 AssetChange 对齐 */
export interface AssetChange {
  id: number;
  assetCardId: number; // 后端 AssetCardId
  changeType: number; // 后端 ChangeType (int)
  reason: string; // 后端 Reason
  fromDeptId?: number; // 后端 FromDeptId
  toDeptId?: number; // 后端 ToDeptId
  disposalIncome?: number; // 后端 DisposalIncome
  operatorId: number; // 后端 OperatorId
  createdTime?: string;
}

/** 资产盘点 - 与后端 AssetInventory 对齐 */
export interface AssetInventory {
  id: number;
  inventoryNo: string; // 后端 InventoryNo
  inventoryDate: string; // 后端 InventoryDate
  operatorId: number; // 后端 OperatorId
  itemsJson?: string; // 后端 ItemsJson
  status: number; // 后端 Status (0未完成 1已完成)
  createdTime?: string;
}
