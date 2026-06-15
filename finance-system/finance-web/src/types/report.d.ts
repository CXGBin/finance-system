/** 报表通用查询参数 */
export interface ReportParams {
  period: string;
  yearPeriod?: string;
  subjectCode?: string;
  level?: number;
}

/** 资产负债表行 */
export interface BalanceSheetRow {
  id?: number;
  lineNo: number;
  itemName: string;
  lineType: 'header' | 'item' | 'total';
  startBalance: number;
  endBalance: number;
  formula?: string;
  children?: BalanceSheetRow[];
}

/** 利润表行 */
export interface IncomeStatementRow {
  id?: number;
  lineNo: number;
  itemName: string;
  lineType: 'header' | 'item' | 'total';
  currentAmount: number;
  yearToDateAmount: number;
  formula?: string;
}

/** 现金流量表行 */
export interface CashFlowRow {
  id?: number;
  lineNo: number;
  itemName: string;
  lineType: 'header' | 'item' | 'total';
  currentAmount: number;
  yearToDateAmount: number;
}

/** 科目余额表行 */
export interface SubjectBalanceRow {
  subjectCode: string;
  subjectName: string;
  level: number;
  periodBeginDebit: number;
  periodBeginCredit: number;
  periodDebit: number;
  periodCredit: number;
  periodEndDebit: number;
  periodEndCredit: number;
  yearBeginDebit: number;
  yearBeginCredit: number;
  children?: SubjectBalanceRow[];
}
